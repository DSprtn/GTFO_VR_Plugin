

using GTFO_VR.Core;
using GTFO_VR.Events;
using System;
using UnityEngine;
using Valve.VR;

namespace GTFO_VR.Input
{
    public class HMD : MonoBehaviour
    {

        public HMD(IntPtr value)
: base(value) { }

        public static GameObject hmd;

        SteamVR_TrackedObject tracking;

        void Awake()
        {
            SetupHMDObject();
        }

        private void SetupHMDObject()
        {
            hmd = new GameObject("HMD_ORIGIN");
            tracking = hmd.AddComponent<SteamVR_TrackedObject>();
            tracking.index = SteamVR_TrackedObject.EIndex.Hmd;

            UnityEngine.Object.DontDestroyOnLoad(hmd);
        }

        public static void SetOrigin(Transform transform)
        {
            HMD.hmd.transform.SetParent(transform);
        }

        public static Vector3 GetVRInteractionLookDir()
        {
            if(ItemEquippableEvents.CurrentItemHasFlashlight())
            {
                return Controllers.GetAimForward();
            } else
            {
                return hmd.transform.forward;
            }
        }

        public static Vector3 GetVRInteractionFromPosition()
        {
            if (ItemEquippableEvents.CurrentItemHasFlashlight())
            {
                return Controllers.GetAimFromPos();
            }
            else
            {
                return hmd.transform.position;
            }
        }

        public static Vector3 GetWorldForward()
        {
            return hmd.transform.forward;
        }

        public static Vector3 GetFlatForwardDirection()
        {
            Vector3 dir = hmd.transform.forward;
            dir.y = 0;
            return dir.normalized;
        }

        public static float GetPlayerHeight()
        {
            if(!hmd)
            {
                return 1.8f;
            }
            return hmd.transform.localPosition.y;
        }

        public static Vector3 GetOffsetPosition(Vector3 playerPos)
        {
            return hmd.transform.position;
        }

        public static Vector3 GetWorldPosition()
        {
            return hmd.transform.position;
        }

        public static Vector3 GetVRCameraEulerRotation()
        {
            Quaternion localRotation = hmd.transform.rotation;


            if(!PlayerVR.fpsCamera || FocusStateManager.CurrentState.Equals(eFocusState.InElevator))
            {
                return localRotation.eulerAngles;
            }

            // Get local rotation for FPS Camera from world hmd rotation to keep using the game's systems and keep player rotation in multiplayer in sync
            localRotation = Quaternion.Inverse(PlayerVR.fpsCamera.m_holder.transform.rotation) * localRotation;

            return localRotation.eulerAngles;
        }

    }
}
