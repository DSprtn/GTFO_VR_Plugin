using GTFO_VR.Core.VR_Input;
using GTFO_VR.Events;
using Player;
using System;
using UnityEngine;

namespace GTFO_VR.Core.PlayerBehaviours
{
    /// <summary>
    /// Responsible for managing the player's position and rotation within the playspace and within GTFO's systems.
    /// </summary>
    public class PlayerOrigin : MonoBehaviour
    {
        public PlayerOrigin(IntPtr value)
       : base(value) { }

        public static event Action OnOriginShift;

        private PlayerAgent m_agent;

        private Vector3 m_offsetFromPlayerToHMD = Vector3.zero;

        Quaternion PlayerRotationOffset = Quaternion.identity;

        private void Awake()
        {
            Log.Info("Origin created");
        }

        public void Setup(PlayerAgent agent)
        {
            m_agent = agent;
            FocusStateEvents.OnFocusStateChange += FocusStateChanged;
            Snapturn.OnSnapTurn += HandleOriginShift;
            SetupOrigin();
            SetInitialRotationOffset();
        }

        private void SetupOrigin()
        {
            Controllers.SetOrigin(transform);
            HMD.SetOrigin(transform);
            DontDestroyOnLoad(gameObject);
            Log.Info("Origin created and set");
        }

        private void Update()
        {
            UpdateOrigin();
        }

        private void LateUpdate()
        {
            UpdateOrigin();
        }

        private void FocusStateChanged(eFocusState newState)
        {
            if (FocusStateEvents.lastState.Equals(eFocusState.InElevator) && newState.Equals(eFocusState.FPS))
            {
                CenterPlayerToOrigin();
            }

            if (newState.Equals(eFocusState.InElevator))
            {
                SetInitialRotationOffset();
            }
        }

        private void HandleOriginShift()
        {
            UpdateOrigin();
            CenterPlayerToOrigin();
            OnOriginShift?.Invoke();
        }

        public void RotatePlayer(Quaternion rotation)
        {
            PlayerRotationOffset *= rotation;
        }

        private void SetInitialRotationOffset()
        {
            m_offsetFromPlayerToHMD = Vector3.zero;
            PlayerRotationOffset = Quaternion.Euler(new Vector3(0, -HMD.Hmd.transform.localRotation.eulerAngles.y, 0f));
            UpdateOrigin();
        }

        public void UpdateOrigin()
        {
            if (m_agent.PlayerCharacterController == null)
            {
                return;
            }
            Vector3 newPosition = m_agent.PlayerCharacterController.SmoothPosition + new Vector3(0, VRConfig.configFloorOffset.Value / 100f, 0);

            transform.position = newPosition - m_offsetFromPlayerToHMD;
            transform.rotation = PlayerRotationOffset;
            transform.position -= CalculateCrouchOffset();
        }

        private Vector3 CalculateCrouchOffset()
        {
            if (m_agent && m_agent.Locomotion.m_currentStateEnum.Equals(PlayerLocomotion.PLOC_State.Crouch))
            {
                float goalCrouchHeight = VRConfig.configCrouchHeight.Value / 100f;

                float diff = Mathf.Max(0f, HMD.GetPlayerHeight() - goalCrouchHeight);
                return new Vector3(0, diff, 0);
            }
            return Vector3.zero;
        }

        public void CenterPlayerToOrigin()
        {
            Vector3 pos = HMD.Hmd.transform.localPosition;
            pos.y = 0f;
            pos = PlayerRotationOffset * pos;
            m_offsetFromPlayerToHMD = pos;
        }

        private void OnDestroy()
        {
            Controllers.OnOriginDestroyed();
            HMD.OnOriginDestroyed();
            FocusStateEvents.OnFocusStateChange -= FocusStateChanged;
            Snapturn.OnSnapTurn += HandleOriginShift;
        }
    }
}