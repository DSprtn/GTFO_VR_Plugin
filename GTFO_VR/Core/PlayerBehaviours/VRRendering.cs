using GTFO_VR.Core.VR_Input;
using GTFO_VR.Events;
using System;
using UnityEngine;
using UnityEngine.PostProcessing;
using Valve.VR;

namespace GTFO_VR.Core.PlayerBehaviours
{
    /// <summary>
    /// Responsible for getting juicy VR visuals into your HMD of choice
    /// </summary>
    public class VRRendering : MonoBehaviour
    {
        public VRRendering(IntPtr value) : base(value)
        {
        }

        private FPSCamera m_fpsCamera;
        private FPS_Render m_fpsRender;
        private UI_Apply m_uiBlitter;
        private static bool m_skipLeftEye;

        private void Awake()
        {
            m_fpsCamera = GetComponent<FPSCamera>();
            m_uiBlitter = GetComponent<UI_Apply>();
            m_fpsRender = GetComponent<FPS_Render>();
            SteamVR_Render.preRenderBothEyesCallback += PreRenderUpdate;
            SteamVR_Render.eyePreRenderCallback += PrepareFrameForEye;
        }

        private void PreRenderUpdate()
        {
            if (m_fpsCamera.m_owner == null || m_fpsCamera.m_holder == null)
                return;
            if (!m_fpsCamera.m_cameraTransition.IsActive())
            {
                m_fpsCamera.RotationUpdate();
                if (!m_fpsCamera.m_holder.m_connectedToPlayer)
                    m_fpsCamera.m_owner.transform.rotation = Quaternion.Euler(0.0f, m_fpsCamera.Rotation.eulerAngles.y, 0.0f);
            }
            if (!FocusStateManager.CurrentState.Equals(eFocusState.InElevator))
            {
                m_fpsCamera.transform.position = HMD.GetWorldPosition();
            }

            m_fpsCamera.m_camera.transform.parent.localRotation = Quaternion.Euler(HMD.GetVRCameraEulerRelativeToFPSCameraParent());
            m_fpsCamera.UpdateCameraRay();

            m_fpsCamera.UpdateLookatTeammates();

            VRPlayer.UpdateHeldItemTransform();

            DoUglyCameraHack();

            WeaponShellManager.EjectFPSShells();
            m_fpsCamera.m_owner.PositionHasBeenUpdated();

            GuiManager.Current.AfterCameraUpdate();
            m_fpsCamera.m_owner.Interaction.AfterCameraUpdate();

            m_fpsRender.ForceMatrixUpdate();
        }

        private void PrepareFrameForEye(EVREye eye)
        {
            m_skipLeftEye = Time.frameCount % 2 == 0;

            DoUglyCameraHack();

            FixHeadAttachedFlashlightPos(eye);

            m_fpsCamera.m_cullingCamera.RunVisibilityOnPreCull();
            m_fpsCamera.m_preRenderCmds.Clear();
            m_fpsCamera.m_beforeForwardAlpahCmds.Clear();

            if (VRConfig.configAlternateEyeRendering.Value)
            {
                if (m_skipLeftEye && eye == EVREye.Eye_Left)
                {
                    return;
                }
                if (!m_skipLeftEye && eye == EVREye.Eye_Right)
                {
                    return;
                }
            }

            PrepareFrame();
        }

        private void FixHeadAttachedFlashlightPos(EVREye eye)
        {
            if (FocusStateEvents.currentState == eFocusState.FPS)
            {
                var inv = m_fpsCamera.m_owner.Inventory;
                if (inv.m_flashlightCLight == null)
                {
                    return;
                }

                if (!ItemEquippableEvents.CurrentItemHasFlashlight())
                {
                    inv.m_flashlightCLight.m_unityLight.transform.position = HMD.Hmd.transform.TransformPoint(m_fpsCamera.m_owner.Inventory.m_flashlightCameraOffset);
                }
                else
                {
                    var lightAlign = inv.m_currentGearPartFlashlight.m_lightAlign;
                    inv.m_flashlightCLight.m_unityLight.transform.forward = lightAlign.forward;
                    inv.m_flashlightCLight.m_unityLight.transform.position = lightAlign.position;
                }

                inv.m_flashlightCLight.UpdateTransformData();
            }
        }

        private void PrepareFrame()
        {
            UI_Core.RenderUI();

            if (ScreenLiquidManager.LiquidSystem != null)
                ScreenLiquidManager.LiquidSystem.CollectCommands(m_fpsCamera.m_preRenderCmds);
            if (AirParticleSystem.AirParticleSystem.Current != null)
                AirParticleSystem.AirParticleSystem.Current.CollectCommands(m_fpsCamera.m_preRenderCmds, m_fpsCamera.m_beforeForwardAlpahCmds);

            if (m_fpsCamera.CollectCommandsClustered)
            {
                ClusteredRendering.Current.CollectCommands(m_fpsCamera.m_preRenderCmds);
            }

            if (m_fpsCamera.CollectCommandsGUIX && GUIX_Manager.isSetup)
            {
                GUIX_Manager.Current.CollectCommands(m_fpsCamera.m_preRenderCmds);
            }

            if (MapDetails.s_isSetup && MapDetails.s_isSetup)
            {
                MapDetails.Current.CollectCommands(m_fpsCamera.m_preRenderCmds);
            }

            Vector4 projectionParams = ClusteredRendering.GetProjectionParams(ClusteredRendering.Current.m_camera);
            Vector4 zbufferParams = ClusteredRendering.GetZBufferParams(ClusteredRendering.Current.m_camera);
            m_fpsCamera.m_preRenderCmds.SetGlobalVector(ClusteredRendering.ID_ProjectionParams, projectionParams);
            m_fpsCamera.m_preRenderCmds.SetGlobalVector(ClusteredRendering.ID_ZBufferParams, zbufferParams);

            Shader.SetGlobalMatrix("_MATRIX_V", m_fpsCamera.m_camera.worldToCameraMatrix);
            Shader.SetGlobalMatrix("_MATRIX_VP", GL.GetGPUProjectionMatrix(m_fpsCamera.m_camera.projectionMatrix, false) * m_fpsCamera.m_camera.worldToCameraMatrix);
            Shader.SetGlobalMatrix("_MATRIX_IV", this.transform.worldToLocalMatrix);
            Shader.SetGlobalInt("_FrameIndex", m_fpsCamera.m_frameIndex++);
        }

        // Force FOV/Aspects and position match up for all relevant game cameras
        private void DoUglyCameraHack()
        {
            float FOV = SteamVR.instance.fieldOfView;
            if (m_uiBlitter != null)
            {
                m_uiBlitter.enabled = VRConfig.configCameraBlood.Value;
            }

            m_fpsCamera.PlayerMoveEnabled = true;
            m_fpsCamera.MouseLookEnabled = true;

            if (m_fpsCamera != null && m_fpsCamera.m_camera != null)
            {
                m_fpsCamera.m_camera.fieldOfView = FOV;
                m_fpsCamera.m_camera.aspect = SteamVR.instance.aspect;
                if (m_fpsCamera.m_itemCamera != null)
                {
                    m_fpsCamera.m_itemCamera.fieldOfView = FOV;
                    m_fpsCamera.m_itemCamera.aspect = SteamVR.instance.aspect;
                }
            }

            if (ClusteredRendering.Current != null && ClusteredRendering.Current.m_lightBufferCamera != null)
            {
                // ToDO - Do we need to set the light buffer camera's properties?
                ClusteredRendering.Current.m_lightBufferCamera.fieldOfView = FOV;
                ClusteredRendering.Current.m_lightBufferCamera.aspect = SteamVR.instance.aspect;
                ClusteredRendering.Current.m_lightBufferCamera.transform.position = m_fpsCamera.m_camera.transform.position;
                ClusteredRendering.Current.m_lightBufferCamera.transform.rotation = m_fpsCamera.m_camera.transform.rotation;


                ClusteredRendering.Current.m_camera.fieldOfView = FOV;
                ClusteredRendering.Current.m_camera.aspect = SteamVR.instance.aspect;
                ClusteredRendering.Current.m_camera.transform.position = m_fpsCamera.m_camera.transform.position;
                ClusteredRendering.Current.m_camera.transform.rotation = m_fpsCamera.m_camera.transform.rotation;

                // This is a small hack to compensate for high FOVs and view distances in clustered rendering in the elevator
                // Not setting this results in buggy lighting in the elevator.
                if (FocusStateEvents.currentState == eFocusState.InElevator)
                {
                    ClusteredRendering.Current.m_camera.nearClipPlane = 0.075f;
                }
                else
                {
                    ClusteredRendering.Current.m_camera.nearClipPlane = 0.01f;
                }
            }
            // Make sure camera is disabled at all times because rendering should only be called from within SteamVR's rendering system
            m_fpsCamera.m_camera.enabled = false;
            m_fpsCamera.m_itemCamera.enabled = false;
        }

        private void OnDestroy()
        {
            SteamVR_Render.preRenderBothEyesCallback -= PreRenderUpdate;
            SteamVR_Render.eyePreRenderCallback -= PrepareFrameForEye;
        }
    }
}