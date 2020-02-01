using Player;
using System;
using UnityEngine;
using Valve.VR;

namespace GTFO_VR
{
    public class VRInitiator : MonoBehaviour
    {
        public static GameObject hmd;

        public static bool crouchOnIRLCrouch = true;

        public static bool VR_ENABLED;

        public static TrackingType VR_TRACKING_TYPE;

        private GameObject Player;

        private SteamVR_Camera camera;

        private PlayerAgent playerAgent;

        public static FPSCamera fpscamera;

        private bool useUglyFOVHack;

        // Seems to be a pretty ok multiplier for not making your eyes hurt from looking at FP weapons
        static float itemHolderFOVMult = 1.10f;

        void Awake()
        {
            if (VR_ENABLED)
            {
                Debug.LogWarning("Trying to create duplicate VRInit class...");
                return;
            }
            Setup();
        }

        void Update()
        {
            if (!VR_ENABLED)
            {
                return;
            }
            HandleHMD();

            DoDebugOnKeyDown();
        }

        private void DoDebugOnKeyDown()
        {
            if (Input.GetKeyDown(KeyCode.F2))
            {
                DebugHelper.LogTransformHierarchy(fpscamera.transform.root);
                DebugHelper.LogTransformHierarchy(playerAgent.transform.root);
            }

            if (Input.GetKeyDown(KeyCode.F3))
            {
                DebugHelper.LogPosRotData(fpscamera.transform);
                DebugHelper.LogPosRotData(fpscamera.transform.root);
                DebugHelper.LogPosRotData(playerAgent.transform);
                DebugHelper.LogPosRotData(playerAgent.FPItemHolder.transform);
            }

        }

        private void HandleHMD()
        {
            if (useUglyFOVHack)
            {
                DoUglyFOVHack();
            }
        }

        public static Vector3 GetVRCameraEulerRotation()
        {
            Quaternion localRotation = hmd.transform.rotation;
            localRotation = Quaternion.Inverse(fpscamera.m_holder.transform.rotation) * localRotation;
            float x = localRotation.eulerAngles.x;
            float y = localRotation.eulerAngles.y;
            float z = localRotation.eulerAngles.z;
            return localRotation.eulerAngles;
        }

        private void DoUglyFOVHack()
        {
            fpscamera.m_camera.fieldOfView = SteamVR.instance.fieldOfView;
            fpscamera.m_camera.aspect = SteamVR.instance.aspect;
            fpscamera.m_itemCamera.fieldOfView = SteamVR.instance.fieldOfView * itemHolderFOVMult;
            fpscamera.m_itemCamera.aspect = SteamVR.instance.aspect;
            ClusteredRendering.Current.m_lightBufferCamera.fieldOfView = SteamVR.instance.fieldOfView;
            ClusteredRendering.Current.m_lightBufferCamera.aspect = SteamVR.instance.aspect;
        }

        private void Setup()
        {
            Debug.Log("Enabling VR");
            SetupGTFOCamera();
            SetupControllers();
            SetupHMDObject();
            gameObject.AddComponent<VRInput>();
            useUglyFOVHack = true;
            VR_ENABLED = true;
        }

        private void SetupGTFOCamera()
        {
            VR_TRACKING_TYPE = TrackingType.PositionAndRotation;
            fpscamera = UnityEngine.Object.FindObjectOfType<FPSCamera>();
            playerAgent = UnityEngine.Object.FindObjectOfType<PlayerAgent>();
            Player = playerAgent.gameObject;
            // Handle head transform manually to better fit into the game's systems and allow things like syncing lookDir online
            // This is handled in harmony injections to FPSCamera
            SteamVR_Camera.useHeadTracking = false;
            camera = fpscamera.gameObject.AddComponent<SteamVR_Camera>();
            CellSettingsApply.ApplyWorldFOV((int)SteamVR.instance.fieldOfView);
        }

        // Controllers do not work correctly at all yet, wrong transforms/parents
        private void SetupControllers()
        {
            PlayerAgent playerAgent = UnityEngine.Object.FindObjectOfType<PlayerAgent>();
            GameObject gameObject = new GameObject("LeftHand");
            GameObject gameObject2 = new GameObject("RightHand");
            gameObject.transform.SetParent(fpscamera.m_holder.transform);
            gameObject2.transform.SetParent(fpscamera.m_holder.transform);
            SteamVR_Behaviour_Pose steamVR_Behaviour_Pose = gameObject.AddComponent<SteamVR_Behaviour_Pose>();
            steamVR_Behaviour_Pose.inputSource = SteamVR_Input_Sources.LeftHand;
            steamVR_Behaviour_Pose.broadcastDeviceChanges = true;
            SteamVR_Behaviour_Pose steamVR_Behaviour_Pose2 = gameObject2.AddComponent<SteamVR_Behaviour_Pose>();
            steamVR_Behaviour_Pose2.inputSource = SteamVR_Input_Sources.RightHand;
            steamVR_Behaviour_Pose2.broadcastDeviceChanges = true;
            GameObject gameObject3 = new GameObject("LeftModel");
            SteamVR_RenderModel steamVR_RenderModel = gameObject3.AddComponent<SteamVR_RenderModel>();
            steamVR_RenderModel.updateDynamically = true;
            steamVR_RenderModel.createComponents = true;
            gameObject3.transform.SetParent(gameObject.transform);
            GameObject gameObject4 = new GameObject("RightModel");
            SteamVR_RenderModel steamVR_RenderModel2 = gameObject4.AddComponent<SteamVR_RenderModel>();
            steamVR_RenderModel2.updateDynamically = true;
            steamVR_RenderModel2.createComponents = true;
            gameObject4.transform.SetParent(gameObject2.transform);
            gameObject3.transform.localPosition = Vector3.zero;
            gameObject4.transform.localPosition = Vector3.zero;
        }

        private void SetupHMDObject()
        {
            hmd = new GameObject("HMD_ORIGIN");
            hmd.AddComponent<SteamVR_TrackedObject>().index = SteamVR_TrackedObject.EIndex.Hmd;
            UnityEngine.Object.DontDestroyOnLoad(hmd);
        }

    }
}
