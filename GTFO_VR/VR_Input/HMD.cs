

using UnityEngine;
using Valve.VR;

namespace GTFO_VR.Input
{
    public class HMD : MonoBehaviourExtended
    {
        public static GameObject hmd;

        public static Vector3 position;

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

        public static Vector3 GetVRCameraEulerRotation()
        {
            Quaternion localRotation = hmd.transform.rotation;
            // TODO Snaprot
            // TODO Incorporate origin into transform code
            //localRotation *= snapTurnRot;

            // Get local rotation for FPS Camera from world hmd rotation to keep using the game's systems and keep player rotation in multiplayer in sync
            if (PlayerVR.LoadedAndInGame && PlayerVR.fpscamera)
            {
                localRotation = Quaternion.Inverse(PlayerVR.fpscamera.m_holder.transform.rotation) * localRotation;
            }
            return localRotation.eulerAngles;
        }
    }
}
