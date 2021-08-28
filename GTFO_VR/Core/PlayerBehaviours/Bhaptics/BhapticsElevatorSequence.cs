using UnityEngine;
using Bhaptics.Tact;
using GTFO_VR.Events;
using System;
using System.Collections.Generic;

namespace GTFO_VR.Core.PlayerBehaviours
{
    public class BhapticsElevatorSequence : MonoBehaviour
    {
        private static readonly string VEST_ELEVATOR_RIDE_KEY = "vest_elevator_ride";
        private static readonly string VEST_ELEVATOR_RIDE_END_KEY = "vest_elevator_ride_end";
        private static readonly string VEST_ELEVATOR_DEPLOYING_KEY = "vest_elevator_deploying";
        
        private static readonly string ARMS_ELEVATOR_RIDE_KEY = "arms_elevator_ride";
        private static readonly string ARMS_ELEVATOR_DEPLOYING_KEY = "arms_elevator_deploying";

        private HapticPlayer m_hapticPlayer;

        private ElevatorState m_elevatorState = ElevatorState.None;
        private float m_currentStateStartTime;
        private float m_nextHapticPatternTime;
        private Vector3 m_elevatorPosition;

        private static readonly float ELEVATOR_RIDE_FEEDBACK_DURATION = 0.75f;
        private static readonly float ELEVATOR_DEPLOYING_FEEDBACK_DURATION = 1.0f;

        private static readonly float LANDED_STATE_DURATION = 1.5f;
        private static readonly float DEPLOYING_STATE_DURATION = 8f;
        private static readonly float ELEVATOR_END_PATTERN_FLOOR_DISTANCE = 60f;

        enum ElevatorState
        {
            None,
            Preparing,
            Descending,
            Landed,
            Deploying,
        }

        public BhapticsElevatorSequence(IntPtr value) : base(value)
        {
        }

        public void Setup(HapticPlayer hapticPlayer)
        {
            m_hapticPlayer = hapticPlayer;
            
            BhapticsUtils.RegisterVestTactKey(hapticPlayer, VEST_ELEVATOR_RIDE_KEY);
            BhapticsUtils.RegisterVestTactKey(hapticPlayer, VEST_ELEVATOR_RIDE_END_KEY);
            BhapticsUtils.RegisterVestTactKey(hapticPlayer, VEST_ELEVATOR_DEPLOYING_KEY);

            BhapticsUtils.RegisterArmsTactKey(hapticPlayer, ARMS_ELEVATOR_RIDE_KEY);
            BhapticsUtils.RegisterArmsTactKey(hapticPlayer, ARMS_ELEVATOR_DEPLOYING_KEY);

            ElevatorEvents.OnElevatorPositionChanged += OnElevatorPositionChanged;
            ElevatorEvents.OnElevatorStateChanged += OnElevatorStateChangedHaptics;
        }

        void Update()
        {
            float currentTime = Time.time;

            if (m_elevatorState != ElevatorState.None)
            {
                UpdateCurrentState();

                if (m_nextHapticPatternTime > 0 && currentTime >= m_nextHapticPatternTime)
                {
                    List<string> hapticPatternKeys = GetHapticPatternKeys(m_elevatorState);

                    foreach (string hapticPatternKey in hapticPatternKeys)
                    {
                        float hapticPatternDuration = GetFeedbackDuration(hapticPatternKey);
                        float durationScale = GetElevatorRidePatternDurationScale();
                        var scaleOption = new ScaleOption(1f, durationScale);
                        Log.Info("Start elevator haptic pattern " + hapticPatternKey + " with duration " + hapticPatternDuration + " and scale " + durationScale + " at " + currentTime);
                        m_hapticPlayer.SubmitRegistered(hapticPatternKey, "", scaleOption);
                        m_nextHapticPatternTime += hapticPatternDuration * durationScale;
                    }
                }
            }
        }

        private void UpdateCurrentState()
        {
            float timeSinceStateStart = Time.time - m_currentStateStartTime;

            if (m_elevatorState == ElevatorState.Preparing && GetElevatorVelocity() > 0)
            {
                ChangeElevatorState(ElevatorState.Descending);
                m_nextHapticPatternTime = Time.time;
            }
            else if (m_elevatorState == ElevatorState.Descending && m_elevatorPosition.y < 2f)
            {
                ChangeElevatorState(ElevatorState.Landed);
                m_nextHapticPatternTime = 0f;
                m_hapticPlayer.TurnOff(VEST_ELEVATOR_RIDE_END_KEY);
            }
            else if (m_elevatorState == ElevatorState.Landed && timeSinceStateStart >= LANDED_STATE_DURATION)
            {
                ChangeElevatorState(ElevatorState.Deploying);
                m_nextHapticPatternTime = Time.time;
            }
            
            else if (m_elevatorState == ElevatorState.Deploying && timeSinceStateStart >= DEPLOYING_STATE_DURATION)
            {
                ChangeElevatorState(ElevatorState.None);
                m_nextHapticPatternTime = 0f;
                m_hapticPlayer.TurnOff(VEST_ELEVATOR_DEPLOYING_KEY);
            }
        }

        private float GetElevatorVelocity()
        {
            return ElevatorRide.CurrentVelocity;
        }

        private List<string> GetHapticPatternKeys(ElevatorState elevatorState)
        {
            var keys = new List<string>();

            switch (elevatorState)
            {
                case ElevatorState.Descending:
                    if (m_elevatorPosition.y > ELEVATOR_END_PATTERN_FLOOR_DISTANCE)
                    {
                        keys.Add(VEST_ELEVATOR_RIDE_KEY);
                        keys.Add(ARMS_ELEVATOR_RIDE_KEY);
                    }
                    else
                    {
                        keys.Add(VEST_ELEVATOR_RIDE_END_KEY);
                    }
                    break;
                case ElevatorState.Deploying:
                    keys.Add(VEST_ELEVATOR_DEPLOYING_KEY);
                    keys.Add(ARMS_ELEVATOR_DEPLOYING_KEY);
                    break;
                case ElevatorState.None:
                case ElevatorState.Preparing:
                case ElevatorState.Landed:
                    break;
            }

            return keys;
        }

        private float GetFeedbackDuration(string patternKey)
        {
            if (patternKey == VEST_ELEVATOR_RIDE_KEY || patternKey == VEST_ELEVATOR_RIDE_END_KEY || patternKey == ARMS_ELEVATOR_RIDE_KEY)
            {
                return ELEVATOR_RIDE_FEEDBACK_DURATION;
            }
            else if (patternKey == VEST_ELEVATOR_DEPLOYING_KEY || patternKey == ARMS_ELEVATOR_DEPLOYING_KEY)
            {
                return ELEVATOR_DEPLOYING_FEEDBACK_DURATION;
            }

            return 0;
        }

        private float GetElevatorRidePatternDurationScale()
        {
            float scale = 1f;
            float currentTime = Time.time;

            const float MAX_ELEVATOR_VELOCITY = 400f;

            if (m_elevatorState == ElevatorState.Descending)
            {
                const float MIN_PATTERN_SCALE = 0.5f;
                const float MAX_PATTERN_SCALE = 1.3f;
                scale = MAX_PATTERN_SCALE - ((GetElevatorVelocity() / MAX_ELEVATOR_VELOCITY) * (MAX_PATTERN_SCALE - MIN_PATTERN_SCALE));
            }

            return scale;
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
    }
}