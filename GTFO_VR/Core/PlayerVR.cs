using GTFO_VR.Core;
using GTFO_VR.Events;
using GTFO_VR.Input;
using GTFO_VR.UI;
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

        public static PlayerAgent playerAgent;

        Watch watch;

        LaserPointer pointer;

        void Awake()
        {
            if (VRSetup)
            {
                Debug.LogWarning("Trying to create duplicate VRInit class...");
                return;
            }
            FocusStateEvents.OnFocusStateChange += FocusStateChanged;
            //Resolution hmdResolution = new Resolution();
            //hmdResolution.height = (int)SteamVR.instance.sceneHeight;
            //hmdResolution.width = (int)SteamVR.instance.sceneWidth / 2;
            //PlayerGui.OnResolutionChange(hmdResolution);
        }

        private void SpawnWatch()
        {
            if(!watch)
            {
                watch = Instantiate(VRGlobal.watchPrefab, new Vector3(0, 0, 0), Quaternion.Euler(new Vector3(0, 0, 0)), null).AddComponent<Watch>();
                watch.transform.localScale = new Vector3(1.25f, 1.25f, 1.25f);
            }
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
                if (watch)
                {
                    watch.SwitchState();
                }
               
                //UIVisible = !UIVisible;


                //VRGlobal.ClearUIRenderTex();
                //PlayerGui.SetVisible(UIVisible);
                //PlayerGui.Inventory.SetVisible(UIVisible);
                // PlayerGui.m_playerStatus.SetVisible(UIVisible);
                //PlayerGui.m_compass.SetVisible(UIVisible);
            }
            UpdateOrigin();
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

        public void UpdateOrigin()
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
            SpawnWatch();
            
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
                ClusteredRendering.Current.m_lightBufferCamera.transform.position = fpscamera.m_camera.transform.position;
                ClusteredRendering.Current.m_lightBufferCamera.transform.rotation = fpscamera.m_camera.transform.rotation;
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
            Destroy(watch);
        }

    }
}
