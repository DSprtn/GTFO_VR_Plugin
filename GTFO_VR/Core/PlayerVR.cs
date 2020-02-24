using GTFO_VR.Core;
using GTFO_VR.Events;
using GTFO_VR.Input;
using Player;
using System;
using UnityEngine;
using Valve.VR;
using Valve.VR.Extras;

namespace GTFO_VR
{
    public class PlayerVR : MonoBehaviour
    {

        public static bool VRSetup;

        public static GameObject origin;

        public static PlayerCharacterController playerController;

        public static FPSCamera fpscamera;

        public static PlayerGuiLayer PlayerGui;

        public static bool LoadedAndInGame = false;

        public static SteamVR_Camera VRCamera;

        static Quaternion snapTurnRot = Quaternion.identity;

        public static bool UIVisible = true;

        LaserPointer pointer;

        public static PlayerAgent playerAgent;

        void Awake()
        {
            if (VRSetup)
            {
                Debug.LogWarning("Trying to create duplicate VRInit class...");
                return;
            }
            FocusStateEvents.OnFocusStateChange += FocusStateChanged;
            Resolution res = new Resolution();
            res.width = 1024;
            res.height = 768;
            ClusteredRendering.Current.OnResolutionChange(res);
        }

        public void FocusStateChanged(eFocusState newState)
        {
            // Do nothing yet
        }

        void Update()
        {
            if(!fpscamera)
            {
                VRSetup = false;
            }
            // Ugly, but works for now, assumes player is always created in elevator 
            if(!VRSetup && FocusStateManager.CurrentState.Equals(eFocusState.InElevator))
            {
                if (!fpscamera)
                {
                    Debug.LogWarning("Performing performance heavy lookup for fpscamera...");
                    fpscamera = FindObjectOfType<FPSCamera>();
                }

                if (!playerController)
                {
                    Debug.LogWarning("Performing performance heavy lookup for playerController...");
                    playerController = FindObjectOfType<PlayerCharacterController>();
                }

                if (fpscamera && playerController && !VRSetup )
                {
                    Setup();
                }
            } else
            {
                UpdateOrigin();
            }
        }

        void LateUpdate()
        {
            if (!VRSetup)
            {
                return;
            }

            DoUglyCameraHack();
           
            if(VRInput.GetActionDown(InputAction.Aim))
            {
                UIVisible = !UIVisible;


                VRGlobal.ClearUIRenderTex();
                //PlayerGui.SetVisible(UIVisible);
                //PlayerGui.Inventory.SetVisible(UIVisible);
                // PlayerGui.m_playerStatus.SetVisible(UIVisible);
                //PlayerGui.m_compass.SetVisible(UIVisible);
            }
        }

        

        public static float VRDetectionMod(Vector3 dir, float distance, float m_flashLightRange, float m_flashlight_spotAngle)
        {
            if (distance > m_flashLightRange) {
                return 0.0f;
            }
            Vector3 VRLookDir = fpscamera.Forward; 
            if(ItemEquippableEvents.CurrentItemHasFlashlight() && VRSettings.UseVRControllers)
            {
                VRLookDir = Controllers.GetAimForward();
            }
            float angleDiff = Vector3.Angle(dir, -VRLookDir);
            float spotlightAngleSize = m_flashlight_spotAngle * 0.5f;
            if (angleDiff > spotlightAngleSize)
                return 0.0f;
            float distanceMultiplier = 1.0f - distance / m_flashLightRange;
            return Mathf.Min((1.0f - angleDiff / spotlightAngleSize) * distanceMultiplier, 0.2f);
        }

        public static void SetPlayerGUIInstance(PlayerGuiLayer gui)
        {
            PlayerGui = gui;
        }

        private void UpdateOrigin()
        {
            if(origin == null || playerController == null)
            {
                return;
            }
            origin.transform.position = playerController.SmoothPosition;          
        }

        private void Setup()
        {
            Debug.Log("Enabling VR player");
            SetupOrigin();
            SetupLaserPointer();
            SetupVRPlayerCamera();
            VRSetup = true;
            LoadedAndInGame = true;
        }

        private void SetupOrigin()
        {
            if(origin)
            {
                return;
            }
            origin = new GameObject("Origin");
            Controllers.SetOrigin(origin.transform);
            DontDestroyOnLoad(origin);
        }

        private void SetupLaserPointer()
        {
            if(pointer)
            {
                return;
            }
            GameObject laserPointer = new GameObject("LaserPointer");
            pointer = laserPointer.AddComponent<LaserPointer>();
            pointer.color = Color.red;
        }

        private void SetupVRPlayerCamera()
        {
            if (VRCamera)
            {
                return;
            }
            // Handle head transform manually to better fit into the game's systems and allow things like syncing lookDir online
            // This is handled in harmony injections to FPSCamera
            VRCamera = fpscamera.gameObject.AddComponent<SteamVR_Camera>();
        }

        private void DoUglyCameraHack()
        {
            if (fpscamera != null && fpscamera.m_camera != null)
            {
                fpscamera.m_camera.fieldOfView = SteamVR.instance.fieldOfView;
                fpscamera.m_camera.aspect = SteamVR.instance.aspect;
                if (fpscamera.m_itemCamera != null)
                {
                    fpscamera.m_itemCamera.fieldOfView = SteamVR.instance.fieldOfView;
                    fpscamera.m_itemCamera.aspect = SteamVR.instance.aspect;
                }
            }

            if (ClusteredRendering.Current != null && ClusteredRendering.Current.m_lightBufferCamera != null)
            {

                ClusteredRendering.Current.m_lightBufferCamera.fieldOfView = SteamVR.instance.fieldOfView;
                ClusteredRendering.Current.m_lightBufferCamera.aspect = SteamVR.instance.aspect;
            }

            foreach(SteamVR_Camera cam in SteamVR_Render.instance.cameras)
            {
                cam.camera.enabled = false;
            }
        }

        void OnDestroy()
        {
            FocusStateEvents.OnFocusStateChange -= FocusStateChanged;
            Destroy(pointer.gameObject);
        }

    }
}
