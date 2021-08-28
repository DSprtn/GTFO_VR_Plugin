using UnityEngine;
using Bhaptics.Tact;
using GTFO_VR.Events;
using System;
using System.Collections.Generic;

namespace GTFO_VR.Core.PlayerBehaviours
{
    public class BhapticsElevatorSequence : MonoBehaviour
    {
        private static readonly string VEST_ELEVATOR_RIDE_NOISE_KEY = "vest_elevator_ride_noise";
        private static readonly string VEST_ELEVATOR_RIDE_WAVE_KEY = "vest_elevator_ride_wave";
        private static readonly string VEST_ELEVATOR_DEPLOYING_KEY = "vest_elevator_deploying";
        
        private static readonly string ARMS_ELEVATOR_RIDE_WAVE_KEY = "arms_elevator_ride_wave";
        private static readonly string ARMS_ELEVATOR_DEPLOYING_KEY = "arms_elevator_deploying";

        private HapticPlayer m_hapticPlayer;

        private ElevatorState m_elevatorState = ElevatorState.None;
        private float m_currentStateStartTime;
        private Dictionary<string, float> m_nextHapticPatternTimes = new Dictionary<string, float>();
        private Vector3 m_elevatorPosition;

        private static readonly float ELEVATOR_RIDE_FEEDBACK_DURATION = 0.75f;
        private static readonly float ELEVATOR_DEPLOYING_FEEDBACK_DURATION = 1.0f;

        private static readonly float LANDED_STATE_DURATION = 1.5f;
        private static readonly float DEPLOYING_STATE_DURATION = 8f;
        private static readonly float ELEVATOR_END_PATTERN_FLOOR_DISTANCE = 100f;

        enum ElevatorState
        {
            None,
            Preparing,
            Descending,
            SlowingDown,
            Landed,
            Deploying,
        }

        public BhapticsElevatorSequence(IntPtr value) : base(value)
        {
        }

        public void Setup(HapticPlayer hapticPlayer)
        {
            m_hapticPlayer = hapticPlayer;
            
            BhapticsUtils.RegisterVestTactKey(hapticPlayer, VEST_ELEVATOR_RIDE_NOISE_KEY);
            BhapticsUtils.RegisterVestTactKey(hapticPlayer, VEST_ELEVATOR_RIDE_WAVE_KEY);
            BhapticsUtils.RegisterVestTactKey(hapticPlayer, VEST_ELEVATOR_DEPLOYING_KEY);

            BhapticsUtils.RegisterArmsTactKey(hapticPlayer, ARMS_ELEVATOR_RIDE_WAVE_KEY);
            BhapticsUtils.RegisterArmsTactKey(hapticPlayer, ARMS_ELEVATOR_DEPLOYING_KEY);

            ElevatorEvents.OnElevatorPositionChanged += OnElevatorPositionChanged;
            ElevatorEvents.OnElevatorStateChanged += OnElevatorStateChangedHaptics;
        }

        void Update()
        {
            if (m_elevatorState != ElevatorState.None)
            {
                UpdateCurrentState();
                ResubmitCompletedHapticPatterns();
            }
        }

        private void UpdateCurrentState()
        {
            float timeSinceStateStart = Time.time - m_currentStateStartTime;

            if (m_elevatorState == ElevatorState.Preparing && GetElevatorVelocity() > 0)
            {
                ChangeElevatorState(ElevatorState.Descending);
                AddNextHapticPatternTimes();
            }
            else if (m_elevatorState == ElevatorState.Descending && m_elevatorPosition.y < ELEVATOR_END_PATTERN_FLOOR_DISTANCE)
            {
                ChangeElevatorState(ElevatorState.SlowingDown);
                AddNextHapticPatternTimes();
            }
            else if (m_elevatorState == ElevatorState.SlowingDown && m_elevatorPosition.y < 2f)
            {
                ChangeElevatorState(ElevatorState.Landed);
                TurnOffHapticPatterns();
            }
            else if (m_elevatorState == ElevatorState.Landed && timeSinceStateStart >= LANDED_STATE_DURATION)
            {
                ChangeElevatorState(ElevatorState.Deploying);
                AddNextHapticPatternTimes();
            }
            else if (m_elevatorState == ElevatorState.Deploying && timeSinceStateStart >= DEPLOYING_STATE_DURATION)
            {
                ChangeElevatorState(ElevatorState.None);
                TurnOffHapticPatterns();
            }
        }

        private void ResubmitCompletedHapticPatterns()
        {
            var m_nextHapticPatternTimesClone = new Dictionary<string, float>(m_nextHapticPatternTimes); // to avoid modifying original dictionary while looping on it

            foreach (KeyValuePair<string, float> pair in m_nextHapticPatternTimesClone)
            {
                string patternKey = pair.Key;
                float nextHapticPatternTime = pair.Value;

                if (Time.time >= nextHapticPatternTime && nextHapticPatternTime > 0)
                {
                    FeedbackDetails feedback = GetElevatorStateFeedback(m_elevatorState, patternKey);

                    if (feedback != null)
                    {
                        var scaleOption = new ScaleOption(1f, feedback.FeedbackDurationScale);
                        m_hapticPlayer.SubmitRegistered(feedback.PatternKey, feedback.PatternKey, scaleOption);
                        m_nextHapticPatternTimes[patternKey] += feedback.FeedbackDuration * feedback.FeedbackDurationScale;
                    }
                    else
                    {
                        m_nextHapticPatternTimes[patternKey] = 0;
                    }
                }
            }
        }

        private float GetElevatorVelocity()
        {
            return ElevatorRide.CurrentVelocity;
        }

        private void AddNextHapticPatternTimes()
        {
            foreach (FeedbackDetails feedback in GetElevatorStateFeedbacks(m_elevatorState))
            {
                m_nextHapticPatternTimes[feedback.PatternKey] = Time.time;
            }
        }

        private void TurnOffHapticPatterns()
        {
            foreach (string patternKey in m_nextHapticPatternTimes.Keys)
            {
                m_hapticPlayer.TurnOff(patternKey);
            }

            m_nextHapticPatternTimes.Clear();
        }

        private List<FeedbackDetails> GetElevatorStateFeedbacks(ElevatorState elevatorState)
        {
            var result = new List<FeedbackDetails>();

            switch (elevatorState)
            {
                case ElevatorState.Descending:
                    {
                        float duration = ELEVATOR_RIDE_FEEDBACK_DURATION;
                        float durationScale = GetElevatorRideDurationScale();
                        result.Add(new FeedbackDetails(VEST_ELEVATOR_RIDE_NOISE_KEY, duration, durationScale));
                        result.Add(new FeedbackDetails(VEST_ELEVATOR_RIDE_WAVE_KEY, duration, durationScale));
                        result.Add(new FeedbackDetails(ARMS_ELEVATOR_RIDE_WAVE_KEY, duration, durationScale));
                    }
                    break;
                case ElevatorState.SlowingDown:
                    {
                        float durationScale = GetElevatorRideDurationScale();
                        result.Add(new FeedbackDetails(VEST_ELEVATOR_RIDE_NOISE_KEY, ELEVATOR_RIDE_FEEDBACK_DURATION, durationScale));
                    }
                    break;
                case ElevatorState.Deploying:
                    {
                        float feedbackDuration = ELEVATOR_DEPLOYING_FEEDBACK_DURATION;
                        float durationScale = 1f;
                        result.Add(new FeedbackDetails(VEST_ELEVATOR_DEPLOYING_KEY, feedbackDuration, durationScale));
                        result.Add(new FeedbackDetails(ARMS_ELEVATOR_DEPLOYING_KEY, feedbackDuration, durationScale));
                    }
                    break;
            }

            return result;
        }

        private float GetElevatorRideDurationScale()
        {
            const float MAX_ELEVATOR_VELOCITY = 400f;
            const float MIN_PATTERN_SCALE = 0.5f;
            const float MAX_PATTERN_SCALE = 1.3f;
            return MAX_PATTERN_SCALE - ((GetElevatorVelocity() / MAX_ELEVATOR_VELOCITY) * (MAX_PATTERN_SCALE - MIN_PATTERN_SCALE));
        }

        private FeedbackDetails GetElevatorStateFeedback(ElevatorState elevatorState, string patternKey)
        {
            foreach (FeedbackDetails feedback in GetElevatorStateFeedbacks(elevatorState))
            {
                if (feedback.PatternKey == patternKey)
                {
                    return feedback;
                }
            }

            return null;
        }

        private void OnElevatorStateChangedHaptics(ElevatorRideState elevatorRideState)
        {
            if (!VRConfig.configUseBhaptics.Value)
            {
                return;
            }

            if (elevatorRideState == ElevatorRideState.Start)
            {
                ChangeElevatorState(ElevatorState.Preparing);
            }
        }

        private void OnElevatorPositionChanged(Vector3 position)
        {
            m_elevatorPosition = position;
        }

        private void ChangeElevatorState(ElevatorState elevatorState)
        {
            m_elevatorState = elevatorState;
            m_currentStateStartTime = Time.time;
        }

        private void OnDestroy()
        {
            ElevatorEvents.OnElevatorPositionChanged -= OnElevatorPositionChanged;
            ElevatorEvents.OnElevatorStateChanged -= OnElevatorStateChangedHaptics;
        }

        class FeedbackDetails
        {
            public string PatternKey { get; }
            public float FeedbackDuration { get; }
            public float FeedbackDurationScale { get; }

            public FeedbackDetails(string patternKey, float feedbackDuration, float feedbackDurationScale)
            {
                PatternKey = patternKey;
                FeedbackDuration = feedbackDuration;
                FeedbackDurationScale = feedbackDurationScale;
            }
        }
    }
}