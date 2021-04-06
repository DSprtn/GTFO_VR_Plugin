using GTFO_VR.Core;
using GTFO_VR.Events;
using GTFO_VR.Input;
using GTFO_VR_BepInEx.Core;
using Player;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnhollowerBaseLib.Attributes;
using UnityEngine;
using Valve.VR;

namespace GTFO_VR.Core
{
    public class PlayerOrigin : MonoBehaviour
    {
        public PlayerOrigin(IntPtr value)
       : base(value) { }


        public static GameObject origin;

        public Snapturn snapTurn;

        public static Vector3 offsetFromPlayerToHMD = Vector3.zero;


        void Awake()
        {
            GTFO_VR_Plugin.log.LogInfo("Origin created");
            Snapturn.OnSnapTurn += UpdateOrigin;
        }
        public void Setup(Snapturn snapturn)
        {
            this.snapTurn = snapturn;
            FocusStateEvents.OnFocusStateChange += FocusStateChanged;
            SetupOrigin();
            SetInitialSnapTurn();
        }
        [HideFromIl2Cpp]
        private void FocusStateChanged(eFocusState newState)
        {
            if (FocusStateEvents.lastState.Equals(eFocusState.InElevator) && newState.Equals(eFocusState.FPS))
            {
                CenterPlayerToOrigin();
            }

            if (newState.Equals(eFocusState.InElevator))
            {
                SetInitialSnapTurn();
            }
        }

        private void SetInitialSnapTurn()
        {
            offsetFromPlayerToHMD = Vector3.zero;
            snapTurn.snapTurnRotation = Quaternion.Euler(new Vector3(0, -HMD.hmd.transform.localRotation.eulerAngles.y, 0f));
            UpdateOrigin();
        }

        public void UpdateOrigin()
        {
            if (origin == null || PlayerVR.playerController == null)
            {
                return;
            }
            Vector3 newPosition = PlayerVR.playerController.SmoothPosition;
            
            origin.transform.position = newPosition - offsetFromPlayerToHMD;
            origin.transform.rotation = snapTurn.snapTurnRotation;
            origin.transform.position -= CalculateCrouchOffset();

        }

        public static Vector3 GetUnadjustedPosition()
        {
            return PlayerVR.playerController.SmoothPosition;
        }

        [HideFromIl2Cpp]
        Vector3 CalculateCrouchOffset()
        {
            if (PlayerVR.playerAgent && PlayerVR.playerAgent.Locomotion.m_currentStateEnum.Equals(PlayerLocomotion.PLOC_State.Crouch))
            {

                float goalCrouchHeight = VRInput.IRLCrouchBorder;

                float diff = Mathf.Max(0f, HMD.GetPlayerHeight() - goalCrouchHeight);
                return new Vector3(0, diff, 0);
            }
            return Vector3.zero;
        }


        private void SetupOrigin()
        {
            if (origin)
            {
                return;
            }
            GTFO_VR_Plugin.log.LogInfo("Creating origin GO");
            origin = new GameObject("Origin");
            Controllers.SetOrigin(origin.transform);
            HMD.SetOrigin(origin.transform);
            DontDestroyOnLoad(origin);
        }

        public void CenterPlayerToOrigin()
        {
            Vector3 pos = HMD.hmd.transform.localPosition;
            pos.y = 0f;
            pos = snapTurn.snapTurnRotation * pos;
            offsetFromPlayerToHMD = pos;

            GTFO_VR_Plugin.log.LogDebug("Centering player... new offset = " + offsetFromPlayerToHMD);
        }

        void OnDestroy()
        {
            FocusStateEvents.OnFocusStateChange -= FocusStateChanged;
        }
    }
}
