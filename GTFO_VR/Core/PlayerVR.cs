using CullingSystem;
using GTFO_VR.Core;
using GTFO_VR.Events;
using GTFO_VR.Input;
using GTFO_VR.UI;
using GTFO_VR.Util;
using Player;
using System;
using UnityEngine;
using UnityEngine.Rendering;
using Valve.VR;
using Valve.VR.Extras;

namespace GTFO_VR
{
    public class PlayerVR : MonoBehaviour
    {

        PlayerOrigin origin;
        Snapturn snapTurn;
        ColisionFade collisionFader;
        Watch watch;
        LaserPointer pointer;

        public static bool VRPlayerIsSetup;
        public static bool LoadedAndInIngameView = false;

        public static PlayerCharacterController playerController;
        public static PlayerAgent playerAgent;
        public static FPSCamera fpsCamera;
        public static SteamVR_Camera VRCamera;
        public static CommandBuffer preRenderLights;

        bool frameRendered = false;

        void Start()
        {
            if (VRPlayerIsSetup)
            {
                Debug.LogError("Trying to create duplicate VRInit class...");
                return;
            }

            PlayerLocomotionEvents.OnPlayerEnterLadder += LadderEntered;
            SteamVR_Events.NewPosesApplied.AddListener(() => OnNewPoses());
            ClusteredRendering.Current.OnResolutionChange(new Resolution());
        }

        void Update()
        {
            if(!fpsCamera || !playerController)
            {
              VRPlayerIsSetup = false;
            }
            if (!VRPlayerIsSetup)
            {
                TrySetup();
            } else
            {
                HandleSnapturnInput();
                origin.UpdateOrigin();
            }
        }

        void LateUpdate()
        {
            if (!fpsCamera || !playerController)
            {
                return;
            }
            frameRendered = false;
            origin.UpdateOrigin();
        }

        /// <summary>
        /// Before we get the final image into the HMD positions and rotations need to be updated one last time to match the newest positions/rotations to greatly reduce perceivable lag/stuttering. To not get the shadow/light rendering and culling out of sync
        /// we need to make those systems render after this transform update, so we disable it in FPS_Camera and do it here instead. 
        /// </summary>
        void OnNewPoses()
        {
            if (!fpsCamera || !origin)
            {
                return;
            }

            origin.UpdateOrigin();
            UpdateVRCameraPosition();
            UpdateHeldItemPosition();
            DoUglyCameraHack();

            // Multiple pose updates happen per frame but we only need to react to the one after LateUpdate
            if (!frameRendered)
            {
                RenderLoop();
            }
        }

        void LadderEntered(LG_Ladder ladder)
        {
            snapTurn.DoSnapTurnTowards(Quaternion.LookRotation(-ladder.transform.right).eulerAngles, 5f);
            origin.CenterPlayerToOrigin();
        }

        void TrySetup()
        {
            if (FocusStateManager.CurrentState.Equals(eFocusState.InElevator) || FocusStateManager.CurrentState.Equals(eFocusState.FPS))
            {
                if (!fpsCamera)
                {
                    Debug.Log("FPSCamera lookup...");
                    if(playerAgent)
                    {
                        fpsCamera = playerAgent.FPSCamera;
                    }
                }

                if (!playerController)
                {
                    Debug.Log("PlayerController lookup...");
                    if(playerAgent)
                    {
                        playerController = playerAgent.PlayerCharacterController;
                    }
                }

                if (fpsCamera && playerController && !VRPlayerIsSetup)
                {
                    Setup();
                }
            }
        }


        private void Setup()
        {
            if (!snapTurn)
            {
                snapTurn = gameObject.AddComponent<Snapturn>();
            }
            if (!origin)
            {
                origin = gameObject.AddComponent<PlayerOrigin>();
                origin.Setup(snapTurn);
            }
            if (!collisionFader)
            {
                collisionFader = gameObject.AddComponent<ColisionFade>();
            }
            SetupLaserPointer();
            SetupVRPlayerCamera();
            SpawnWatch();
            
            
            Debug.Log("Crouching height... " + playerAgent.PlayerData.camPosCrouch);
            VRPlayerIsSetup = true;
            LoadedAndInIngameView = true;
        }

        private void UpdateVRCameraPosition()
        {
            if (VR_Settings.VR_TRACKING_TYPE.Equals(TrackingType.PositionAndRotation))
            {
                if (!FocusStateManager.CurrentState.Equals(eFocusState.InElevator))
                {
                    Vector3 camPos = HMD.GetWorldPosition();
                    fpsCamera.m_camera.transform.position = camPos;
                    C_Camera.Position = camPos;
                    C_Camera.Forward = HMD.hmd.transform.forward;

                    collisionFader.HandleCameraInCollision();
                }

            }
            fpsCamera.m_camera.transform.parent.localRotation = Quaternion.Euler(HMD.GetVRCameraEulerRotation());
        }

        public static void UpdateHeldItemPosition()
        {
            if (!VR_Settings.UseVRControllers)
            {
                return;
            }
            ItemEquippable heldItem = playerAgent.FPItemHolder.WieldedItem;
            if (heldItem != null)
            {
                heldItem.transform.position = Controllers.GetControllerPosition() + WeaponArchetypeVRData.CalculateGripOffset();
                Vector3 recoilRot = heldItem.GetRecoilRotOffset();

                if (!Utils.IsFiringFromADS())
                {
                    recoilRot.x *= 2f;
                }
                heldItem.transform.rotation = Controllers.GetControllerAimRotation() * Quaternion.Euler(recoilRot);
                heldItem.transform.position += Controllers.GetControllerAimRotation() * heldItem.GetRecoilPosOffset();
            }
        }

        void HandleSnapturnInput()
        {
            if (VRInput.GetSnapTurningLeft())
            {
                snapTurn.DoSnapTurn(-VR_Settings.snapTurnAmount);
                origin.CenterPlayerToOrigin();
            }

            if (VRInput.GetSnapTurningRight())
            {
                snapTurn.DoSnapTurn(VR_Settings.snapTurnAmount);
                origin.CenterPlayerToOrigin();
            }
            
        }

        void SetupLaserPointer()
        {
            if (pointer)
            {
                return;
            }
            GameObject laserPointer = new GameObject("LaserPointer");
            pointer = laserPointer.AddComponent<LaserPointer>();
            pointer.color = Color.red;
        }

        void SetupVRPlayerCamera()
        {
            if (VRCamera)
            {
                return;
            }
            VRCamera = fpsCamera.gameObject.AddComponent<SteamVR_Camera>();
            fpsCamera.gameObject.AddComponent<SteamVR_Fade>();
        }

        void SpawnWatch()
        {
            if (!watch)
            {
                watch = Instantiate(VR_Assets.watchPrefab, new Vector3(0, 0, 0), Quaternion.Euler(new Vector3(0, 0, 0)), null).AddComponent<Watch>();
                Vector3 watchScale = new Vector3(1.25f, 1.25f, 1.25f);
                watchScale *= VR_Settings.watchScale;
                watch.transform.localScale = watchScale;
            }
        }

        // Force FOV/Aspects and position match up for all relevant game cameras
        void DoUglyCameraHack()
        {
            fpsCamera.PlayerMoveEnabled = true;
            fpsCamera.MouseLookEnabled = true;

            if (fpsCamera != null && fpsCamera.m_camera != null)
            {
                fpsCamera.m_camera.fieldOfView = SteamVR.instance.fieldOfView;
                fpsCamera.m_camera.aspect = SteamVR.instance.aspect;
                if (fpsCamera.m_itemCamera != null)
                {
                    fpsCamera.m_itemCamera.fieldOfView = SteamVR.instance.fieldOfView;
                    fpsCamera.m_itemCamera.aspect = SteamVR.instance.aspect;
                }
            }

            if (ClusteredRendering.Current != null && ClusteredRendering.Current.m_lightBufferCamera != null)
            {

                ClusteredRendering.Current.m_lightBufferCamera.fieldOfView = SteamVR.instance.fieldOfView;
                ClusteredRendering.Current.m_lightBufferCamera.aspect = SteamVR.instance.aspect;
                ClusteredRendering.Current.m_lightBufferCamera.transform.position = fpsCamera.m_camera.transform.position;
                ClusteredRendering.Current.m_lightBufferCamera.transform.rotation = fpsCamera.m_camera.transform.rotation;


                ClusteredRendering.Current.m_camera.fieldOfView = SteamVR.instance.fieldOfView;
                ClusteredRendering.Current.m_camera.aspect = SteamVR.instance.aspect;
                ClusteredRendering.Current.m_camera.transform.position = fpsCamera.m_camera.transform.position;
                ClusteredRendering.Current.m_camera.transform.rotation = fpsCamera.m_camera.transform.rotation;
            }

            foreach (SteamVR_Camera cam in SteamVR_Render.instance.cameras)
            {
                cam.camera.enabled = false;
            }
        }

        
        void RenderLoop()
        {
            C_Camera.Current.RunVisibilityOnPreCull();

            preRenderLights.Clear();


            if (fpsCamera.debugRenderUI)
            {
                UI_Core.RenderUI();
            }

            if (fpsCamera.debugRenderClustered)
            {
                ClusteredRendering.Current.CollectCommands(preRenderLights);
            }

            if (fpsCamera.debugRenderGUIX)
            {
                GUIX_Manager.Current.CollectCommands(preRenderLights);
            }

            if (MapDetails.s_isSetup)
            {
                MapDetails.Current.CollectCommands(preRenderLights);
            }

            Vector4 projectionParams = ClusteredRendering.GetProjectionParams(ClusteredRendering.Current.m_camera);
            Vector4 zbufferParams = ClusteredRendering.GetZBufferParams(ClusteredRendering.Current.m_camera);
            preRenderLights.SetGlobalVector(ClusteredRendering.ID_ProjectionParams, projectionParams);
            preRenderLights.SetGlobalVector(ClusteredRendering.ID_ZBufferParams, zbufferParams);

            frameRendered = true;
        }

        public static float VRDetectionMod(Vector3 dir, float distance, float m_flashLightRange, float m_flashlight_spotAngle)
        {
            if (distance > m_flashLightRange)
            {
                return 0.0f;
            }
            Vector3 VRLookDir = fpsCamera.Forward;
            if (ItemEquippableEvents.CurrentItemHasFlashlight() && VR_Settings.UseVRControllers)
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

        void OnDestroy()
        {
            PlayerLocomotionEvents.OnPlayerEnterLadder -= LadderEntered;
            Destroy(pointer.gameObject);
            Destroy(watch);
            SteamVR_Events.NewPosesApplied.RemoveListener(() => OnNewPoses());
        }

    }
}
