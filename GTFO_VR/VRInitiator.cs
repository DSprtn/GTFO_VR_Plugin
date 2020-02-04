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

        private SteamVR_Camera camera;

        public static PlayerAgent playerAgent;

        public static FPSCamera fpscamera;

        private bool useUglyFOVHack;

        // Seems to be a pretty ok multiplier for not making your eyes hurt from looking at FP weapons
        static float itemHolderFOVMult = 1.10f;


        public static GameObject leftController;
        public static GameObject rightController;
        GameObject origin;

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
            UpdateOrigin();
            DoDebugOnKeyDown();
        }

        private void UpdateOrigin()
        {
            if(!origin)
            {
                return;
            }
            origin.transform.position = playerAgent.PlayerCharacterController.SmoothPosition;
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
                DebugHelper.LogPosRotData(leftController.transform);
                DebugHelper.LogPosRotData(rightController.transform);
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
            origin = new GameObject("Origin");
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
            // Handle head transform manually to better fit into the game's systems and allow things like syncing lookDir online
            // This is handled in harmony injections to FPSCamera
            SteamVR_Camera.useHeadTracking = false;
            camera = fpscamera.gameObject.AddComponent<SteamVR_Camera>();
            CellSettingsApply.ApplyWorldFOV((int)SteamVR.instance.fieldOfView);
        }

        private void SetupControllers()
        {
            leftController = SetupController(SteamVR_Input_Sources.LeftHand);
            rightController = SetupController(SteamVR_Input_Sources.RightHand);
            leftController.name = "LeftController";
            rightController.name = "RightController";
        }
            
        GameObject SetupController(SteamVR_Input_Sources source)
        {
            GameObject controller = new GameObject("Controller");
            SteamVR_Behaviour_Pose steamVR_Behaviour_Pose = controller.AddComponent<SteamVR_Behaviour_Pose>();
            steamVR_Behaviour_Pose.inputSource = source;
            steamVR_Behaviour_Pose.broadcastDeviceChanges = true;


            // Controller rendering - Unneeded at this version
            //GameObject model = new GameObject("Model");
            //SteamVR_RenderModel steamVR_RenderModel = model.AddComponent<SteamVR_RenderModel>();
            //steamVR_RenderModel.updateDynamically = true;
            //steamVR_RenderModel.createComponents = true;
            //model.transform.SetParent(controller.transform);
            //model.transform.localPosition = Vector3.zero;

            controller.transform.SetParent(origin.transform);
            return controller;
        }

        private void SetupHMDObject()
        {
            hmd = new GameObject("HMD_ORIGIN");
            hmd.AddComponent<SteamVR_TrackedObject>().index = SteamVR_TrackedObject.EIndex.Hmd;
            UnityEngine.Object.DontDestroyOnLoad(hmd);
        }

    }
}
