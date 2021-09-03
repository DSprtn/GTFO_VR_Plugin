using UnityEngine;
using Bhaptics.Tact;
using GTFO_VR.Events;
using System;
using System.Collections.Generic;
using Player;

namespace GTFO_VR.Core.PlayerBehaviours
{
    public class BhapticsElevatorSequence : MonoBehaviour
    {
        private static readonly string VEST_ELEVATOR_RIDE_NOISE_KEY = "vest_elevator_ride_noise";
        private static readonly string VEST_ELEVATOR_RIDE_WAVE_KEY = "vest_elevator_ride_wave";
        private static readonly string VEST_ELEVATOR_DEPLOYING_KEY = "vest_elevator_deploying";
        private static readonly string VEST_ELEVATOR_DOOR_PRE_OPENING_KEY = "vest_elevator_door_pre_opening";
        private static readonly string VEST_ELEVATOR_DOOR_OPENING_KEY = "vest_elevator_door_opening";

        private static readonly string ARMS_ELEVATOR_RIDE_WAVE_KEY = "arms_elevator_ride_wave";
        private static readonly string ARMS_ELEVATOR_DEPLOYING_KEY = "arms_elevator_deploying";

        private PlayerAgent m_player;
        private HapticPlayer m_hapticPlayer;

        private int m_movingFramesCount;
        private Vector3 m_lastPlayerPosition;
        private ElevatorState m_elevatorState = ElevatorState.None;
        private float m_currentStateStartTime;
        private Dictionary<string, float> m_nextHapticPatternTimes = new Dictionary<string, float>();
        private Vector3 m_elevatorPosition;

        private static readonly float ELEVATOR_RIDE_FEEDBACK_DURATION = 0.75f;
        private static readonly float ELEVATOR_DEPLOYING_FEEDBACK_DURATION = 1.0f;

        private static readonly float PENDING_DOOR_PRE_OPENING_DURATION = 3.5f;
        private static readonly float PENDING_DOOR_OPENING_DURATION = 3f;
        private static readonly float PENDING_TOP_DEPLOYING_DURATION = 4f;
        private static readonly float TOP_DEPLOYING_STATE_DURATION = 5f;
        private static readonly float ELEVATOR_END_PATTERN_FLOOR_DISTANCE = 100f;

        enum ElevatorState
        {
            None,
            SceneLoaded,
            FirstMovement,
            PendingCageRotating,
            CageRotating,
            PendingDoorPreOpening,
            PendingDoorOpening,
            PendingTopDeploying,
            TopDeploying,
            Preparing,
            FirstDescentPattern,
            Descending,
            SlowingDown,
            Landed,
            Deploying,
        }

        public BhapticsElevatorSequence(IntPtr value) : base(value)
        {
        }

        public void Setup(PlayerAgent player, HapticPlayer hapticPlayer)
        {
            m_player = player;
            m_hapticPlayer = hapticPlayer;

            m_movingFramesCount = 0;
            m_lastPlayerPosition = m_player.transform.position;
            m_elevatorState = ElevatorState.None;
            m_currentStateStartTime = 0;
            m_nextHapticPatternTimes.Clear();
            m_elevatorPosition = Vector3.zero;

            BhapticsUtils.RegisterVestTactKey(hapticPlayer, VEST_ELEVATOR_RIDE_NOISE_KEY);
            BhapticsUtils.RegisterVestTactKey(hapticPlayer, VEST_ELEVATOR_RIDE_WAVE_KEY);
            BhapticsUtils.RegisterVestTactKey(hapticPlayer, VEST_ELEVATOR_DEPLOYING_KEY);
            BhapticsUtils.RegisterVestTactKey(hapticPlayer, VEST_ELEVATOR_DOOR_PRE_OPENING_KEY);
            BhapticsUtils.RegisterVestTactKey(hapticPlayer, VEST_ELEVATOR_DOOR_OPENING_KEY);

            BhapticsUtils.RegisterArmsTactKey(hapticPlayer, ARMS_ELEVATOR_RIDE_WAVE_KEY);
            BhapticsUtils.RegisterArmsTactKey(hapticPlayer, ARMS_ELEVATOR_DEPLOYING_KEY);

            ElevatorEvents.OnElevatorPositionChanged += OnElevatorPositionChanged;
            ElevatorEvents.OnPreReleaseSequenceStarted += OnPreReleaseSequenceStarted;
            ElevatorEvents.OnPreReleaseSequenceSkipped += OnPreReleaseSequenceSkipped;
        }

        void FixedUpdate()
        {
            if (m_elevatorState != ElevatorState.None)
            {
                UpdateCurrentState();
                ResubmitCompletedHapticPatterns();
                
                if (m_lastPlayerPosition != m_player.transform.position)
                {
                    m_movingFramesCount++;
                }
                else
                {
                    m_movingFramesCount = 0;
                }

                m_lastPlayerPosition = m_player.transform.position;
            }
        }

        private bool HasJustStartedMoving()
        {
            const int MIN_MOVING_FRAMES_BEFORE_START_MOVING = 5; // To fix cases where the player moves just one frame, but is not really moving around
            return m_movingFramesCount == MIN_MOVING_FRAMES_BEFORE_START_MOVING;
        }

        private bool HasJustStoppedMoving()
        {
            return m_movingFramesCount > 0 && m_lastPlayerPosition == m_player.transform.position;
        }

        private void UpdateCurrentState()
        {
            float timeSinceStateStart = Time.time - m_currentStateStartTime;

            if (m_elevatorState == ElevatorState.SceneLoaded && HasJustStartedMoving())
            {
                ChangeElevatorState(ElevatorState.FirstMovement);
                AddNextHapticPatternTimes();
            }
            else if (m_elevatorState == ElevatorState.FirstMovement && HasJustStoppedMoving())
            {
                ChangeElevatorState(ElevatorState.PendingCageRotating);
                TurnOffHapticPatterns();
            }
            else if (m_elevatorState == ElevatorState.PendingCageRotating && HasJustStartedMoving())
            {
                ChangeElevatorState(ElevatorState.CageRotating);
                AddNextHapticPatternTimes();
            }
            else if (m_elevatorState == ElevatorState.CageRotating && HasJustStoppedMoving())
            {
                ChangeElevatorState(ElevatorState.PendingDoorPreOpening);
                TurnOffHapticPatterns();
            }
            else if (m_elevatorState == ElevatorState.PendingDoorPreOpening && timeSinceStateStart >= PENDING_DOOR_PRE_OPENING_DURATION) // timing-based since player moves/stops multiple times in a row starting from here
            {
                m_hapticPlayer.SubmitRegistered(VEST_ELEVATOR_DOOR_PRE_OPENING_KEY);
                ChangeElevatorState(ElevatorState.PendingDoorOpening);
            }
            else if (m_elevatorState == ElevatorState.PendingDoorOpening && timeSinceStateStart >= PENDING_DOOR_OPENING_DURATION)
            {
                m_hapticPlayer.SubmitRegistered(VEST_ELEVATOR_DOOR_OPENING_KEY);
                ChangeElevatorState(ElevatorState.PendingTopDeploying);
            }
            else if (m_elevatorState == ElevatorState.PendingTopDeploying && timeSinceStateStart >= PENDING_TOP_DEPLOYING_DURATION)
            {
                ChangeElevatorState(ElevatorState.TopDeploying);
                AddNextHapticPatternTimes();
            }
            else if (m_elevatorState == ElevatorState.TopDeploying && timeSinceStateStart >= TOP_DEPLOYING_STATE_DURATION)
            {
                ChangeElevatorState(ElevatorState.Preparing);
                TurnOffHapticPatterns();
            }
            else if (m_elevatorState == ElevatorState.Preparing && GetElevatorVelocity() > 0)
            {
                ChangeElevatorState(ElevatorState.FirstDescentPattern);
                AddNextHapticPatternTimes(); // play ride pattern
            }
            else if (m_elevatorState == ElevatorState.FirstDescentPattern && timeSinceStateStart >= ELEVATOR_RIDE_FEEDBACK_DURATION)
            {
                // When ride pattern has ended, start wave pattern as well
                ChangeElevatorState(ElevatorState.Descending);
                AddNextHapticPatternTimes();
            }
            else if (m_elevatorState == ElevatorState.Descending && m_elevatorPosition.y < ELEVATOR_END_PATTERN_FLOOR_DISTANCE)
            {
                ChangeElevatorState(ElevatorState.SlowingDown);
                AddNextHapticPatternTimes();
            }
            else if (m_elevatorState == ElevatorState.SlowingDown && m_elevatorPosition.y <= 0.1f)
            {
                ChangeElevatorState(ElevatorState.Landed);
                TurnOffHapticPatterns();
            }
            else if (m_elevatorState == ElevatorState.Landed && HasJustStartedMoving())
            {
                ChangeElevatorState(ElevatorState.Deploying);
                AddNextHapticPatternTimes();
            }
            else if (m_elevatorState == ElevatorState.Deploying && HasJustStoppedMoving())
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
                        var scaleOption = new ScaleOption(feedback.FeedbackIntensity, feedback.FeedbackDurationScale);
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
            m_hapticPlayer.TurnOff(VEST_ELEVATOR_DOOR_OPENING_KEY);
        }

        private List<FeedbackDetails> GetElevatorStateFeedbacks(ElevatorState elevatorState)
        {
            var result = new List<FeedbackDetails>();

            switch (elevatorState)
            {
                case ElevatorState.FirstDescentPattern:
                case ElevatorState.SlowingDown:
                    {
                        float durationScale = GetElevatorRideDurationScale();
                        result.Add(new FeedbackDetails(VEST_ELEVATOR_RIDE_NOISE_KEY, ELEVATOR_RIDE_FEEDBACK_DURATION, durationScale));
                    }
                    break;
                case ElevatorState.Descending:
                    {
                        float duration = ELEVATOR_RIDE_FEEDBACK_DURATION;
                        float durationScale = GetElevatorRideDurationScale();
                        result.Add(new FeedbackDetails(VEST_ELEVATOR_RIDE_NOISE_KEY, duration, durationScale));
                        result.Add(new FeedbackDetails(VEST_ELEVATOR_RIDE_WAVE_KEY, duration, durationScale));
                        result.Add(new FeedbackDetails(ARMS_ELEVATOR_RIDE_WAVE_KEY, duration, durationScale));
                    }
                    break;
                
                case ElevatorState.FirstMovement:
                case ElevatorState.CageRotating:
                    {
                        float feedbackDuration = ELEVATOR_DEPLOYING_FEEDBACK_DURATION;
                        float durationScale = 1f;
                        float intensity = 0.3f;
                        result.Add(new FeedbackDetails(VEST_ELEVATOR_DEPLOYING_KEY, feedbackDuration, durationScale, intensity));
                        result.Add(new FeedbackDetails(ARMS_ELEVATOR_DEPLOYING_KEY, feedbackDuration, durationScale, intensity));
                    }
                    break;
                case ElevatorState.TopDeploying:
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
            const float MIN_PATTERN_SCALE = 0.6f;
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

        private void OnPreReleaseSequenceStarted()
        {
            if (!VRConfig.configUseBhaptics.Value)
            {
                return;
            }

            ChangeElevatorState(ElevatorState.SceneLoaded);
            AddNextHapticPatternTimes();
        }

        private void OnPreReleaseSequenceSkipped()
        {
            ChangeElevatorState(ElevatorState.Preparing);
            TurnOffHapticPatterns();
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
            ElevatorEvents.OnPreReleaseSequenceStarted -= OnPreReleaseSequenceStarted;
            ElevatorEvents.OnPreReleaseSequenceSkipped -= OnPreReleaseSequenceSkipped;
        }

        class FeedbackDetails
        {
            public string PatternKey { get; }
            public float FeedbackDuration { get; }
            public float FeedbackDurationScale { get; }
            public float FeedbackIntensity { get; }

            public FeedbackDetails(string patternKey, float feedbackDuration, float feedbackDurationScale, float feedbackIntensity = 1f)
            {
                PatternKey = patternKey;
                FeedbackDuration = feedbackDuration;
                FeedbackDurationScale = feedbackDurationScale;
                FeedbackIntensity = feedbackIntensity;
            }
        }
    }
}