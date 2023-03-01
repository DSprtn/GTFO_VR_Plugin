﻿using UnityEngine;
using Bhaptics.Tact;
using System.Collections.Generic;

namespace GTFO_VR.Core.PlayerBehaviours.BodyHaptics.Bhaptics
{
    public class BhapticsElevatorSequence : ElevatorSequenceAgent
    {
        private static readonly string VEST_ELEVATOR_RIDE_NOISE_KEY = "vest_elevator_ride_noise";
        private static readonly string VEST_ELEVATOR_RIDE_WAVE_KEY = "vest_elevator_ride_wave";
        private static readonly string VEST_ELEVATOR_DEPLOYING_KEY = "vest_elevator_deploying";
        private static readonly string VEST_ELEVATOR_DOOR_OPENING_KEY = "vest_elevator_door_opening";

        private static readonly string ARMS_ELEVATOR_RIDE_WAVE_KEY = "arms_elevator_ride_wave";
        private static readonly string ARMS_ELEVATOR_DEPLOYING_KEY = "arms_elevator_deploying";

        private HapticPlayer m_hapticPlayer;

        private ElevatorState m_elevatorState = ElevatorState.None;
        private Dictionary<string, float> m_nextHapticPatternTimes = new Dictionary<string, float>();
        private bool m_isInElevator = true;

        private static readonly float ELEVATOR_DEPLOYING_FEEDBACK_DURATION = 1.0f;

        public void Setup(HapticPlayer hapticPlayer)
        {
            m_hapticPlayer = hapticPlayer;

            m_elevatorState = ElevatorState.None;
            m_nextHapticPatternTimes.Clear();

            BhapticsUtils.RegisterVestTactKey(hapticPlayer, VEST_ELEVATOR_RIDE_NOISE_KEY);
            BhapticsUtils.RegisterVestTactKey(hapticPlayer, VEST_ELEVATOR_RIDE_WAVE_KEY);
            BhapticsUtils.RegisterVestTactKey(hapticPlayer, VEST_ELEVATOR_DEPLOYING_KEY);
            BhapticsUtils.RegisterVestTactKey(hapticPlayer, VEST_ELEVATOR_DOOR_OPENING_KEY);

            BhapticsUtils.RegisterArmsTactKey(hapticPlayer, ARMS_ELEVATOR_RIDE_WAVE_KEY);
            BhapticsUtils.RegisterArmsTactKey(hapticPlayer, ARMS_ELEVATOR_DEPLOYING_KEY);
        }

        public void Update()
        {
            if (m_elevatorState != ElevatorState.None && m_isInElevator)
            {
                ResubmitCompletedHapticPatterns();
            }
        }

        public void ElevatorStateChanged(ElevatorState elevatorState)
        {
            m_elevatorState = elevatorState;

            if (elevatorState == ElevatorState.FirstMovement
                || elevatorState == ElevatorState.CageRotating
                || elevatorState == ElevatorState.TopDeploying
                || elevatorState == ElevatorState.FirstDescentPattern
                || elevatorState == ElevatorState.Descending
                || elevatorState == ElevatorState.SlowingDown
                || elevatorState == ElevatorState.Deploying)
            {
                AddNextHapticPatternTimes();
            }
            else if (elevatorState == ElevatorState.PendingCageRotating
                     || elevatorState == ElevatorState.PendingDoorOpening
                     || elevatorState == ElevatorState.Preparing
                     || elevatorState == ElevatorState.Landed
                     || elevatorState == ElevatorState.None)
            {
                TurnOffHapticPatterns();
            }
            else if (elevatorState == ElevatorState.PendingTopDeploying)
            {
                m_hapticPlayer.SubmitRegistered(VEST_ELEVATOR_DOOR_OPENING_KEY);
            }
        }

        public void SetIsInElevator(bool inElevator)
        {
            m_isInElevator = inElevator;
        }

        private void ResubmitCompletedHapticPatterns()
        {
            if (!VRConfig.configUseBhaptics.Value)
            {
                return;
            }

            var nextHapticPatternTimesClone = new Dictionary<string, float>(m_nextHapticPatternTimes); // to avoid modifying original dictionary while looping on it

            foreach (KeyValuePair<string, float> pair in nextHapticPatternTimesClone)
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

            if (!m_isInElevator)
            {
                return result;
            }

            switch (elevatorState)
            {
                case ElevatorState.FirstDescentPattern:
                case ElevatorState.SlowingDown:
                    {
                        float durationScale = BodyHapticsUtils.GetElevatorRideDurationScale();
                        result.Add(new FeedbackDetails(VEST_ELEVATOR_RIDE_NOISE_KEY, BodyHapticsUtils.ELEVATOR_RIDE_FEEDBACK_DURATION, durationScale));
                    }
                    break;
                case ElevatorState.Descending:
                    {
                        float duration = BodyHapticsUtils.ELEVATOR_RIDE_FEEDBACK_DURATION;
                        float durationScale = BodyHapticsUtils.GetElevatorRideDurationScale();
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