using System;
using Bhaptics.Tact;
using GTFO_VR.Core.PlayerBehaviours.BodyHaptics.Bhaptics;
using GTFO_VR.Core.PlayerBehaviours.BodyHaptics.Shockwave;
using GTFO_VR.Events;
using Player;
using UnityEngine;

namespace GTFO_VR.Core.PlayerBehaviours.BodyHaptics
{
    public class ElevatorSequenceIntegrator : MonoBehaviour
    {
        private LocalPlayerAgent m_player;

        private BhapticsElevatorSequence m_bhapticsSequence;
        private ShockwaveElevatorSequence m_shockwaveSequence;

        private ElevatorState m_elevatorState = ElevatorState.None;
        private int m_movingFramesCount;
        private Vector3 m_lastPlayerPosition;
        private float m_currentStateStartTime;
        private Vector3 m_elevatorPosition;

        private static readonly float PENDING_DOOR_PRE_OPENING_DURATION = 3.5f;
        private static readonly float PENDING_TOP_DEPLOYING_DURATION = 7f;
        private static readonly float TOP_DEPLOYING_STATE_DURATION = 5f;
        private static readonly float ELEVATOR_END_PATTERN_FLOOR_DISTANCE = 100f;

        public ElevatorSequenceIntegrator(IntPtr value) : base(value)
        {
        }

        public void Setup(LocalPlayerAgent player, HapticPlayer hapticPlayer)
        {
            m_player = player;
            m_lastPlayerPosition = m_player.transform.position;
            m_elevatorState = ElevatorState.None;
            m_elevatorPosition = Vector3.zero;

            m_bhapticsSequence = new BhapticsElevatorSequence();
            m_bhapticsSequence.Setup(hapticPlayer);

            m_shockwaveSequence = new ShockwaveElevatorSequence();
            m_shockwaveSequence.Setup();
        }

        private void Awake()
        {
            ElevatorEvents.OnElevatorPositionChanged += OnElevatorPositionChanged;
            ElevatorEvents.OnPreReleaseSequenceStarted += OnPreReleaseSequenceStarted;
            ElevatorEvents.OnPreReleaseSequenceSkipped += OnPreReleaseSequenceSkipped;
            PlayerLocomotionEvents.OnStateChange += OnPlayerLocomotionStateChanged;
        }

        private void OnDestroy()
        {
            ElevatorEvents.OnElevatorPositionChanged -= OnElevatorPositionChanged;
            ElevatorEvents.OnPreReleaseSequenceStarted -= OnPreReleaseSequenceStarted;
            ElevatorEvents.OnPreReleaseSequenceSkipped -= OnPreReleaseSequenceSkipped;
            PlayerLocomotionEvents.OnStateChange -= OnPlayerLocomotionStateChanged;
        }

        private ElevatorSequenceAgent[] GetAgents()
        {
            return new ElevatorSequenceAgent[] { m_bhapticsSequence, m_shockwaveSequence };
        }

        private void FixedUpdate()
        {
            if (m_elevatorState != ElevatorState.None)
            {
                UpdateCurrentState();

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

            foreach (ElevatorSequenceAgent agent in GetAgents())
            {
                agent.Update();
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
            }
            else if (m_elevatorState == ElevatorState.FirstMovement && HasJustStoppedMoving())
            {
                ChangeElevatorState(ElevatorState.PendingCageRotating);
            }
            else if (m_elevatorState == ElevatorState.PendingCageRotating && HasJustStartedMoving())
            {
                ChangeElevatorState(ElevatorState.CageRotating);
            }
            else if (m_elevatorState == ElevatorState.CageRotating && HasJustStoppedMoving())
            {
                ChangeElevatorState(ElevatorState.PendingDoorOpening);
            }
            else if (m_elevatorState == ElevatorState.PendingDoorOpening && timeSinceStateStart >= PENDING_DOOR_PRE_OPENING_DURATION)
            {
                ChangeElevatorState(ElevatorState.PendingTopDeploying);
            }
            else if (m_elevatorState == ElevatorState.PendingTopDeploying && timeSinceStateStart >= PENDING_TOP_DEPLOYING_DURATION)
            {
                ChangeElevatorState(ElevatorState.TopDeploying);
            }
            else if (m_elevatorState == ElevatorState.TopDeploying && timeSinceStateStart >= TOP_DEPLOYING_STATE_DURATION)
            {
                ChangeElevatorState(ElevatorState.Preparing);
            }
            else if (m_elevatorState == ElevatorState.Preparing && ElevatorRide.CurrentVelocity > 0)
            {
                ChangeElevatorState(ElevatorState.FirstDescentPattern); // play ride pattern
            }
            else if (m_elevatorState == ElevatorState.FirstDescentPattern && timeSinceStateStart >= BodyHapticsUtils.ELEVATOR_RIDE_FEEDBACK_DURATION)
            {
                ChangeElevatorState(ElevatorState.Descending); // When ride pattern has ended, start wave pattern as well
            }
            else if (m_elevatorState == ElevatorState.Descending && m_elevatorPosition.y < ELEVATOR_END_PATTERN_FLOOR_DISTANCE)
            {
                ChangeElevatorState(ElevatorState.SlowingDown);
            }
            else if (m_elevatorState == ElevatorState.SlowingDown && m_elevatorPosition.y <= 0.1f)
            {
                ChangeElevatorState(ElevatorState.Landed);
            }
            else if (m_elevatorState == ElevatorState.Landed && HasJustStartedMoving())
            {
                ChangeElevatorState(ElevatorState.Deploying);
            }
            else if (m_elevatorState == ElevatorState.Deploying && HasJustStoppedMoving())
            {
                ChangeElevatorState(ElevatorState.None);
            }
        }

        private void OnPreReleaseSequenceStarted()
        {
            if (!VRConfig.configUseBhaptics.Value)
            {
                return;
            }

            ChangeElevatorState(ElevatorState.SceneLoaded);
        }

        private void OnPreReleaseSequenceSkipped()
        {
            ChangeElevatorState(ElevatorState.Preparing);
        }

        private void OnPlayerLocomotionStateChanged(PlayerLocomotion.PLOC_State state)
        {
            foreach (ElevatorSequenceAgent agent in GetAgents())
            {
                agent.SetIsInElevator(state == PlayerLocomotion.PLOC_State.InElevator);
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

            foreach (ElevatorSequenceAgent agent in GetAgents())
            {
                agent.ElevatorStateChanged(elevatorState);
            }
        }
    }
}