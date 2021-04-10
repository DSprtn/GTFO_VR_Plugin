using CullingSystem;
using GTFO_VR.Core.VR_Input;
using GTFO_VR.Events;
using GTFO_VR.UI;
using GTFO_VR.Util;
using GTFO_VR_BepInEx.Core;
using Player;
using System;
using UnityEngine;
using UnityEngine.Rendering;
using Valve.VR;


namespace GTFO_VR.Core.PlayerBehaviours
{
    public class PlayerVR : MonoBehaviour
    {

        public PlayerVR(IntPtr value)
: base(value) { }

        PlayerOrigin origin;
        Snapturn snapTurn;
        Watch watch;
        LaserPointer pointer;
        VRWorldSpaceUI worldUI;

        public static bool VRPlayerIsSetup;
        public static bool LoadedAndInIngameView = false;

        public static PlayerCharacterController playerController;
        public static PlayerAgent playerAgent;
        public static FPSCamera fpsCamera;
        public static SteamVR_Camera VRCamera;
        public static CommandBuffer preRenderLights;
        public static CommandBuffer beforeForwardCmd;



        void Start()
        {
            SteamVR_Render.eyePreRenderCallback += PreRenderEye;
            if (VRPlayerIsSetup)
            {
                Debug.LogError("Trying to create duplicate VRInit class...");
                return;
            }
            PlayerLocomotionEvents.OnPlayerEnterLadder += LadderEntered;

            SteamVR_Events.NewPosesApplied.Listen(new Action(OnNewPoses));
            ClusteredRendering.Current.OnResolutionChange(new Resolution());
        }

        static bool skipLeft;

        void PreRenderEye(EVREye eye)
        {
            if (Time.frameCount % 2 == 0)
            {
                skipLeft = true;
            } else
            {
                skipLeft = false;
            }

            DoUglyCameraHack();

            
            fpsCamera.m_cullingCamera.RunVisibilityOnPreCull();
            preRenderLights.Clear();
            beforeForwardCmd.Clear();

            if (VR_Settings.alternateLightRenderingPerEye)
            {
                if(skipLeft && eye == EVREye.Eye_Left)
                {
                    return;
                }
                if(!skipLeft && eye == EVREye.Eye_Right)
                {
                    return;
                }
            }

            if (fpsCamera.m_renderUI)
            {
                UI_Core.RenderUI();
            }

            if (ScreenLiquidManager.LiquidSystem != null)
                ScreenLiquidManager.LiquidSystem.CollectCommands(preRenderLights);
            if (AirParticleSystem.AirParticleSystem.Current != null)
                AirParticleSystem.AirParticleSystem.Current.CollectCommands(preRenderLights, beforeForwardCmd);

            if (fpsCamera.m_collectCommandsClustered)
            {
                ClusteredRendering.Current.CollectCommands(preRenderLights);
            }

            if (fpsCamera.m_collectCommandsGUIX)
            {
                GUIX_Manager.Current.CollectCommands(preRenderLights);
            }

            if (MapDetails.s_isSetup && fpsCamera.m_collectCommandsMap)
            {
                MapDetails.Current.CollectCommands(preRenderLights);
            }

            Vector4 projectionParams = ClusteredRendering.GetProjectionParams(ClusteredRendering.Current.m_camera);
            Vector4 zbufferParams = ClusteredRendering.GetZBufferParams(ClusteredRendering.Current.m_camera);
            preRenderLights.SetGlobalVector(ClusteredRendering.ID_ProjectionParams, projectionParams);
            preRenderLights.SetGlobalVector(ClusteredRendering.ID_ZBufferParams, zbufferParams);

            CL_ShadowAtlas.Current.m_dynamicShadowsRenderedThisFrame = 0;
            CL_ShadowAtlas.Current.m_staticShadowsRenderedThisFrame = 0;
            CL_ShadowAtlas.Current.m_compositeShadowsRenderedThisFrame = 0;
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

                if (FocusStateEvents.currentState == eFocusState.InElevator)
                {
                    ClusteredRendering.Current.m_camera.nearClipPlane = 0.075f;
                } else
                {
                    ClusteredRendering.Current.m_camera.nearClipPlane = 0.01f;
                }

            }

            foreach (SteamVR_Camera cam in SteamVR_Render.instance.cameras)
            {
                cam.camera.enabled = false;
            }
        }

        void Update()
        {
            if (!fpsCamera || !playerController)
            {
                VRPlayerIsSetup = false;
            }
            if (!VRPlayerIsSetup)
            {
                TrySetup();
            }
            else
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

        }



        void LadderEntered(LG_Ladder ladder)
        {
            GTFO_VR_Plugin.log.LogDebug("Ladder forward " + ladder.transform.forward);
            GTFO_VR_Plugin.log.LogDebug("Ladder right " + ladder.transform.right);
            GTFO_VR_Plugin.log.LogDebug("Ladder up " + ladder.transform.up);

            snapTurn.DoSnapTurnTowards(Quaternion.LookRotation(ladder.transform.forward).eulerAngles, 10f);
            origin.CenterPlayerToOrigin();
        }

        void TrySetup()
        {
            if (FocusStateManager.CurrentState.Equals(eFocusState.InElevator) || FocusStateManager.CurrentState.Equals(eFocusState.FPS))
            {
                if (!fpsCamera)
                {
                    GTFO_VR_Plugin.log.LogDebug("FPSCamera lookup...");
                    if (playerAgent)
                    {
                        fpsCamera = playerAgent.FPSCamera;
                    }
                }

                if (!playerController)
                {
                    GTFO_VR_Plugin.log.LogDebug("PlayerController lookup...");
                    if (playerAgent)
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
            if (!worldUI)
            {
                worldUI = gameObject.AddComponent<VRWorldSpaceUI>();
            }
            SetupLaserPointer();
            SetupVRPlayerCamera();
            SpawnWatch();


            GTFO_VR_Plugin.log.LogDebug("Crouching height... " + playerAgent.PlayerData.camPosCrouch);
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
                    
                    fpsCamera.transform.position = HMD.GetWorldPosition();
                    C_Camera.Position = camPos;
                    C_Camera.Forward = HMD.hmd.transform.forward;
                    fpsCamera.UpdateCameraRay();
                    CollisionFade.HandleCameraInCollision();
                }
            }
            fpsCamera.m_camera.transform.parent.localRotation = Quaternion.Euler(HMD.GetVRCameraEulerRotation());
        }

        public static void UpdateHeldItemPosition()
        {
            if (!VR_Settings.useVRControllers)
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
                heldItem.transform.rotation = Controllers.GetControllerAimRotation();
                heldItem.transform.localRotation *= Quaternion.Euler(recoilRot) * WeaponArchetypeVRData.GetVRWeaponData(heldItem).rotationOffset;
                heldItem.transform.position += Controllers.GetControllerAimRotation() * heldItem.GetRecoilPosOffset();
            }
        }

        void HandleSnapturnInput()
        {
            if (SteamVR_InputHandler.GetSnapTurningLeft())
            {
                snapTurn.DoSnapTurn(-VR_Settings.snapTurnAmount);
                origin.CenterPlayerToOrigin();
            }

            if (SteamVR_InputHandler.GetSnapTurningRight())
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
            pointer.color = ColorExt.OrangeBright();
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
                watch = Instantiate(VR_Assets.watchPrefab, Vector3.zero, Quaternion.identity, null).AddComponent<Watch>();
                Vector3 watchScale = new Vector3(1.25f, 1.25f, 1.25f);
                watchScale *= VR_Settings.watchScale;
                watch.transform.localScale = watchScale;
            }
        }

        public static float VRDetectionMod(Vector3 dir, float distance, float m_flashLightRange, float m_flashlight_spotAngle)
        {
            if (distance > m_flashLightRange)
            {
                return 0.0f;
            }
            Vector3 VRLookDir = fpsCamera.Forward;
            if (ItemEquippableEvents.CurrentItemHasFlashlight() && VR_Settings.useVRControllers)
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
            SteamVR_Render.eyePreRenderCallback -= PreRenderEye;
            PlayerLocomotionEvents.OnPlayerEnterLadder -= LadderEntered;
            if (pointer)
            {
                Destroy(pointer.gameObject);
            }
            if (watch)
            {
                Destroy(watch.gameObject);
            }
            SteamVR_Events.NewPosesApplied.Remove(new Action(OnNewPoses));
        }

    }
}
