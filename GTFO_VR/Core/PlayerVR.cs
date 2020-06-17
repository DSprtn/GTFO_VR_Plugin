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

        public static bool VRSetup;

        public static GameObject origin;

        public static PlayerCharacterController playerController;

        public static FPSCamera fpscamera;

        public static PlayerGuiLayer PlayerGui;

        public static bool LoadedAndInGame = false;

        public static SteamVR_Camera VRCamera;

        public static bool UIVisible = true;

        public static PlayerAgent playerAgent;

        Watch watch;

        LaserPointer pointer;

        public static CommandBuffer preRenderLights;

        static bool frameRendered = false;

        public static Quaternion snapTurnRotation = Quaternion.identity;

        float snapTurnTime = 0f;

        float snapTurnDelay = 0.25f;

        public static Vector3 offsetFromPlayerToHMD = Vector3.zero;

        static bool headInCollision;
        static bool controllerHeadToHMDHeadBlocked;

        void Awake()
        {
            if (VRSetup)
            {
                Debug.LogWarning("Trying to create duplicate VRInit class...");
                return;
            }

            FocusStateEvents.OnFocusStateChange += FocusStateChanged;
            SteamVR_Events.NewPosesApplied.AddListener(() => OnNewPoses());
            ClusteredRendering.Current.OnResolutionChange(new Resolution());

        }

        /// <summary>
        /// Before we get the final image into the HMD positions and rotations need to be updated one last time to match the newest positions/rotations to greatly reduce perceivable lag/stuttering. To not get the shadow/light rendering and culling out of sync
        /// we need to make those systems render after this transform update, so we disable it in FPS_Camera and do it here instead. 
        /// </summary>
        // TODO - Move to a more logical place
        void OnNewPoses()
        {

            if (!playerController || !fpscamera)
            {
                return;
            }

            UpdateOrigin();
            if (VRSettings.VR_TRACKING_TYPE.Equals(TrackingType.PositionAndRotation))
            {
                if (!FocusStateManager.CurrentState.Equals(eFocusState.InElevator))
                {
                    Vector3 camPos = HMD.GetWorldPosition();
                    fpscamera.m_camera.transform.position = camPos;
                    C_Camera.Position = camPos;
                    C_Camera.Forward = HMD.hmd.transform.forward;

                    CheckCameraIsInCollision();
                }

            }
            fpscamera.m_camera.transform.parent.localRotation = Quaternion.Euler(HMD.GetVRCameraEulerRotation());

            UpdateHeldItemPosition();

            DoUglyCameraHack();

            // Multiple pose updates happen per frame but we only need to react to the one after LateUpdate
            if (!frameRendered)
            {
                RenderLoop();
            }
        }

        private void CheckCameraIsInCollision()
        {
            if (Physics.OverlapBox(HMD.GetWorldPosition() + (HMD.GetWorldForward() * 0.05f), new Vector3(0.03f, 0.03f, 0.03f), HMD.hmd.transform.rotation, LayerManager.MASK_TENTACLE_BLOCKERS).Length > 0)
            {
                headInCollision = true;
            }
            else
            {
                headInCollision = false;
            }

            Vector3 centerPlayerHeadPos = fpscamera.GetCamerposForPlayerPos(playerController.SmoothPosition);

            if (Physics.Linecast(centerPlayerHeadPos, HMD.GetWorldPosition(), LayerManager.MASK_TENTACLE_BLOCKERS))
            {
                controllerHeadToHMDHeadBlocked = true;
            }
            else
            {
                controllerHeadToHMDHeadBlocked = false;
            }

            if (controllerHeadToHMDHeadBlocked || headInCollision)
            {
                SteamVR_Fade.Start(Color.black, 0.2f, true);
            }
            else
            {
                SteamVR_Fade.Start(Color.clear, 0.2f, true);
            }

        }

        public static void UpdateHeldItemPosition()
        {
            ItemEquippable heldItem = playerAgent.FPItemHolder.WieldedItem;
            if (heldItem != null)
            {
                heldItem.transform.position = Controllers.GetControllerPosition() + WeaponArchetypeVRData.CalculateGripOffset();
                Vector3 recoilRot = heldItem.GetRecoilRotOffset();

                if (!Util.Utils.IsFiringFromADS())
                {
                    recoilRot.x *= 2f;
                }
                heldItem.transform.rotation = Controllers.GetControllerAimRotation() * Quaternion.Euler(heldItem.GetRecoilRotOffset());
                heldItem.transform.position += Controllers.GetControllerAimRotation() * heldItem.GetRecoilPosOffset();
            }
        }

        private static void RenderLoop()
        {
            C_Camera.Current.RunVisibilityOnPreCull();

            preRenderLights.Clear();


            if (fpscamera.debugRenderUI)
            {
                UI_Core.RenderUI();
            }

            if (fpscamera.debugRenderClustered)
            {
                ClusteredRendering.Current.CollectCommands(preRenderLights);
            }

            if (fpscamera.debugRenderGUIX)
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

        public void FocusStateChanged(eFocusState newState)
        {
            if (FocusStateEvents.lastState.Equals(eFocusState.InElevator) && newState.Equals(eFocusState.FPS))
            {
                CenterPlayerToOrigin();
            }
        }

        void Update()
        {

            if (!fpscamera)
            {
                VRSetup = false;
            }
            // Ugly, but works for now, assumes player is always created in elevator 
            if (!VRSetup && FocusStateManager.CurrentState.Equals(eFocusState.InElevator))
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

                if (fpscamera && playerController && !VRSetup)
                {
                    Setup();
                }
            }
            else
            {
                HandleSnapturnInput();
                UpdateOrigin();
            }
        }

        void HandleSnapturnInput()
        {
            if (VRInput.GetSnapTurningLeft())
            {
                DoSnapTurn(-VRSettings.snapTurnAmount);
            }

            if (VRInput.GetSnapTurningRight())
            {
                DoSnapTurn(VRSettings.snapTurnAmount);
            }

        }

        void DoSnapTurn(float angle)
        {
            if (VRSettings.useSmoothTurn)
            {
                snapTurnRotation *= Quaternion.Euler(new Vector3(0, angle * Time.deltaTime * 2f, 0f));
                CenterPlayerToOrigin();
            }
            else
            {
                if (snapTurnTime + snapTurnDelay < Time.time)
                {
                    SteamVR_Fade.Start(Color.black, 0f, true);
                    SteamVR_Fade.Start(Color.clear, 0.2f, true);
                    snapTurnRotation *= Quaternion.Euler(new Vector3(0, angle, 0f));
                    CenterPlayerToOrigin();
                    snapTurnTime = Time.time;
                }
            }
        }

        void LateUpdate()
        {
            if (!VRSetup)
            {
                return;
            }
            frameRendered = false;

            if (VRInput.GetActionDown(InputAction.Aim))
            {
                if (watch)
                {
                    watch.SwitchState();
                }
            }
            UpdateOrigin();
        }

        public static float VRDetectionMod(Vector3 dir, float distance, float m_flashLightRange, float m_flashlight_spotAngle)
        {
            if (distance > m_flashLightRange)
            {
                return 0.0f;
            }
            Vector3 VRLookDir = fpscamera.Forward;
            if (ItemEquippableEvents.CurrentItemHasFlashlight() && VRSettings.UseVRControllers)
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

        void UpdateOrigin()
        {
            if (origin == null || playerController == null)
            {
                return;
            }

            origin.transform.position = playerController.SmoothPosition - offsetFromPlayerToHMD;
            origin.transform.rotation = snapTurnRotation;
            origin.transform.position -= CalculateCrouchOffset();
        }

        Vector3 CalculateCrouchOffset()
        {
            if (PlayerVR.playerAgent.Locomotion.m_currentStateEnum.Equals(Player.PlayerLocomotion.PLOC_State.Crouch))
            {
                float goalCrouchHeight = VRInput.IRLCrouchBorder;

                float diff = Mathf.Max(0f, HMD.GetPlayerHeight() - goalCrouchHeight);
                return new Vector3(0, diff, 0);
            }
            return Vector3.zero;
        }

        private void Setup()
        {
            Debug.Log("Enabling VR player");
            SetupOrigin();
            SetupLaserPointer();
            SetupVRPlayerCamera();
            SpawnWatch();
            SetInitialSnapTurn();
            Debug.Log("Crouching height... " + playerAgent.PlayerData.camPosCrouch);
            VRSetup = true;
            LoadedAndInGame = true;
        }

        private void SetInitialSnapTurn()
        {
            offsetFromPlayerToHMD = Vector3.zero;
            snapTurnRotation = Quaternion.Euler(new Vector3(0, -HMD.hmd.transform.localRotation.eulerAngles.y, 0f));
            Debug.Log("Setting snap turn rot to " + snapTurnRotation.eulerAngles);
            UpdateOrigin();
        }

        private void SetupOrigin()
        {
            if (origin)
            {
                return;
            }
            origin = new GameObject("Origin");
            Controllers.SetOrigin(origin.transform);
            HMD.SetOrigin(origin.transform);
            DontDestroyOnLoad(origin);
        }

        private void SetupLaserPointer()
        {
            if (pointer)
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
            fpscamera.gameObject.AddComponent<SteamVR_Fade>();
        }

        private void SpawnWatch()
        {
            if (!watch)
            {
                watch = Instantiate(VRGlobal.watchPrefab, new Vector3(0, 0, 0), Quaternion.Euler(new Vector3(0, 0, 0)), null).AddComponent<Watch>();
                watch.transform.localScale = new Vector3(1.25f, 1.25f, 1.25f);
            }
        }

        // Force FOV/Aspects and position match up for all relevant game cameras
        void DoUglyCameraHack()
        {
            fpscamera.PlayerMoveEnabled = true;
            fpscamera.MouseLookEnabled = true;

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


                ClusteredRendering.Current.m_camera.fieldOfView = SteamVR.instance.fieldOfView;
                ClusteredRendering.Current.m_camera.aspect = SteamVR.instance.aspect;
                ClusteredRendering.Current.m_camera.transform.position = fpscamera.m_camera.transform.position;
                ClusteredRendering.Current.m_camera.transform.rotation = fpscamera.m_camera.transform.rotation;
            }

            foreach (SteamVR_Camera cam in SteamVR_Render.instance.cameras)
            {
                cam.camera.enabled = false;
            }
        }

        private static void CenterPlayerToOrigin()
        {
            Vector3 pos = HMD.hmd.transform.localPosition;
            pos.y = 0f;
            pos = snapTurnRotation * pos;
            offsetFromPlayerToHMD = pos;

            Debug.Log("Centering player... new offset = " + offsetFromPlayerToHMD);
        }

        void OnDestroy()
        {
            FocusStateEvents.OnFocusStateChange -= FocusStateChanged;
            Destroy(pointer.gameObject);
            Destroy(watch);
            SteamVR_Events.NewPosesApplied.RemoveListener(() => OnNewPoses());
        }

    }
}
