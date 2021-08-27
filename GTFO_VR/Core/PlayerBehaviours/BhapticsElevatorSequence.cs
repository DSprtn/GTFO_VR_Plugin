using UnityEngine;
using Bhaptics.Tact;
using GTFO_VR.Events;
using System;

namespace GTFO_VR.Core.PlayerBehaviours
{
    public class BhapticsElevatorSequence : MonoBehaviour
    {
        private static readonly string VEST_ELEVATOR_RIDE_KEY = "vest_elevator_ride";

        private HapticPlayer m_hapticPlayer;

        private float m_nextElevatorRideHapticPatternTime;
        private float m_elevatorDescentStartTime;
        private float m_elevatorSlowingDownStartTime;

        private static readonly float ELEVATOR_RIDE_FEEDBACK_DURATION = 0.75f;
        private static readonly float SLOW_ELEVATOR_SEQUENCE_DURATION = 10.0f;
        private static readonly float INITIAL_ELEVATOR_RIDE_DELAY = 1.5f;

        public BhapticsElevatorSequence(IntPtr value) : base(value)
        {
        }

        public void Setup(HapticPlayer hapticPlayer)
        {
            m_hapticPlayer = hapticPlayer;
            
            BhapticsUtils.RegisterVestTactKey(hapticPlayer, VEST_ELEVATOR_RIDE_KEY);

            ElevatorEvents.OnElevatorRideStarted += ElevatorRideStartedHaptics;
            ElevatorEvents.OnElevatorRideStopped += ElevatorRideStoppedHaptics;
        }

        void Update()
        {
            float currentTime = Time.time;

            bool isElevatorRiding = (m_nextElevatorRideHapticPatternTime > 0);
            if (isElevatorRiding)
            {
                if (m_elevatorSlowingDownStartTime > 0 && currentTime >= m_elevatorSlowingDownStartTime + SLOW_ELEVATOR_SEQUENCE_DURATION)
                {
                    // Elevator descent ended
                    m_nextElevatorRideHapticPatternTime = 0f;
                    m_elevatorDescentStartTime = 0f;
                    m_elevatorSlowingDownStartTime = 0f;
                    m_hapticPlayer.TurnOff(VEST_ELEVATOR_RIDE_KEY);
                }
                else if (currentTime >= m_nextElevatorRideHapticPatternTime)
                {
                    float duration = GetElevatorRidePatternDuration();
                    var scaleOption = new ScaleOption(1f, duration);
                    Log.Info("Start elevator ride pattern with duration " + duration + " at " + currentTime);
                    m_hapticPlayer.SubmitRegistered(VEST_ELEVATOR_RIDE_KEY, "", scaleOption);
                    m_nextElevatorRideHapticPatternTime += ELEVATOR_RIDE_FEEDBACK_DURATION * duration;
                }
            }
        }

        private float GetElevatorRidePatternDuration()
        {
            float duration = 1f;
            float currentTime = Time.time;
            const float TIME_BEFORE_SPEED_UP = 5f;

            if (m_elevatorSlowingDownStartTime > 0f)
            {
                // Go progressively from 2.0 to 6.0 duration when elevator slows down
                float timeSinceSlowDown = currentTime - m_elevatorSlowingDownStartTime;
                duration = 1.0f + (timeSinceSlowDown / SLOW_ELEVATOR_SEQUENCE_DURATION) * 2.0f;
            }
            else if (currentTime >= m_elevatorDescentStartTime + TIME_BEFORE_SPEED_UP)
            {
                // Speed up a bit after a few seconds
                float timeSinceDescent = currentTime - m_elevatorDescentStartTime;
                const float MAX_DURATION = 1.0f;
                const float MIN_DURATION = 0.6f;
                duration = BhapticsUtils.Clamp(1 - (timeSinceDescent - TIME_BEFORE_SPEED_UP) * 0.2f, MIN_DURATION, MAX_DURATION);
            }

            return duration;
        }

        private void ElevatorRideStartedHaptics()
        {
            if (!VRConfig.configUseBhaptics.Value)
            {
                return;
            }

            m_nextElevatorRideHapticPatternTime = Time.time + INITIAL_ELEVATOR_RIDE_DELAY;
            m_elevatorDescentStartTime = Time.time;
        }

        private void ElevatorRideStoppedHaptics()
        {
            // This is called when the elevator starts slowing down, not when it fully stops
            if (!VRConfig.configUseBhaptics.Value)
            {
                return;
            }

            m_elevatorSlowingDownStartTime = Time.time;
            m_nextElevatorRideHapticPatternTime = m_elevatorSlowingDownStartTime; // start elevator ride pattern again with reduced pattern duration
            Log.Info("Elevator slows down at " + Time.time);
        }

        private void OnDestroy()
        {
            ElevatorEvents.OnElevatorRideStarted -= ElevatorRideStartedHaptics;
            ElevatorEvents.OnElevatorRideStopped -= ElevatorRideStoppedHaptics;
        }
    }
}