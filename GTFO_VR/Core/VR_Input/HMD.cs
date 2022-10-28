using GTFO_VR.Core.PlayerBehaviours;
using GTFO_VR.Events;
using System;
using UnityEngine;
using Valve.VR;

namespace GTFO_VR.Core.VR_Input
{
    /// <summary>
    /// Handles all VR camera related functions, mostly look direction and positions
    /// </summary>
    public class HMD : MonoBehaviour
    {
        public HMD(IntPtr value)
: base(value) { }

        public static GameObject Hmd;

        private SteamVR_TrackedObject m_deviceTracker;

        private void Awake()
        {
            SetupHMDObject();
        }

        private void SetupHMDObject()
        {
            Hmd = new GameObject("HMD_ORIGIN");
            m_deviceTracker = Hmd.AddComponent<SteamVR_TrackedObject>();
            m_deviceTracker.index = SteamVR_TrackedObject.EIndex.Hmd;

            DontDestroyOnLoad(Hmd);
        }

        public static void SetOrigin(Transform transform)
        {
            Hmd.transform.SetParent(transform);
        }

        internal static void OnOriginDestroyed()
        {
            Hmd.transform.SetParent(null);
            DontDestroyOnLoad(Hmd);
        }

        /// <summary>
        /// Returns the camera's forward or the controller's or weapons' if the player is
        /// holding a weapon that has a flashlight (and by an extension a lasersight)
        /// </summary>
        /// <returns></returns>
        public static Vector3 GetVRInteractionLookDir()
        {
            if (ItemEquippableEvents.CurrentItemHasFlashlight() && VRConfig.configUseControllers.Value)
            {
                return Controllers.GetAimForward();
            }
            else
            {
                return Hmd.transform.forward;
            }
        }

        /// <summary>
        /// Returns the camera's position or the controller's or weapons' if the player is
        /// holding a weapon that has a flashlight (and by extension a lasersight)
        /// </summary>
        /// <returns></returns>

        public static Vector3 GetVRInteractionFromPosition()
        {
            if (ItemEquippableEvents.CurrentItemHasFlashlight() && VRConfig.configUseControllers.Value)
            {
                return Controllers.GetAimFromPos();
            }
            else
            {
                return Hmd.transform.position;
            }
        }

        public static Vector3 GetWorldForward()
        {
            return Hmd.transform.forward;
        }

        public static Vector3 GetFlatForwardDirection()
        {
            Vector3 dir = Hmd.transform.forward;
            dir.y = 0;
            return dir.normalized;
        }

        public static float GetPlayerHeight()
        {
            if (!Hmd)
            {
                return 1.8f;
            }
            return Hmd.transform.localPosition.y;
        }

        public static Vector3 GetWorldPosition()
        {
            return Hmd.transform.position;
        }


        public static Vector3 GetVRCameraEulerRelativeToFPSCameraParent()
        {
            Quaternion localRotation = Hmd.transform.rotation;

            if (!VRPlayer.FpsCamera || FocusStateManager.CurrentState.Equals(eFocusState.InElevator))
            {
                return localRotation.eulerAngles;
            }

            // Get local rotation for FPS Camera from world hmd rotation to keep using the game's systems and keep player rotation in multiplayer in sync
            localRotation = Quaternion.Inverse(VRPlayer.FpsCamera.m_holder.transform.rotation) * localRotation;

            return localRotation.eulerAngles;
        }


    }
}