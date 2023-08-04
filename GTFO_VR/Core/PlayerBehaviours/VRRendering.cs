using GTFO_VR.Core.VR_Input;
using GTFO_VR.Events;
using System;
using UnityEngine;
using UnityEngine.Rendering;
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

        private CommandBuffer gOverwrite;
        private FPSCamera m_fpsCamera;
        private FPS_Render m_fpsRender;
        private UI_Apply m_uiBlitter;

        private Material m_occlusionMaterial;

        private void Awake()
        {
            gOverwrite = new CommandBuffer();
            m_fpsCamera = GetComponent<FPSCamera>();
            m_uiBlitter = GetComponent<UI_Apply>();
            m_fpsRender = GetComponent<FPS_Render>();

            // Disable SteamVR's mask, render our own
            SteamVR_Render.SetRenderHiddenAreaMask(false);
            m_occlusionMaterial = new Material(VRAssets.GetGTFOHiddenAreaMaskShader());
            m_fpsCamera.m_camera.AddCommandBuffer(CameraEvent.BeforeGBuffer, gOverwrite);  // Fill depth, keep stencil clear
            m_fpsCamera.m_camera.AddCommandBuffer(CameraEvent.BeforeImageEffects, gOverwrite); // Ensure masked area is black
            
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

            // Weapons with sights only render their sights otherwise. 
            Shader.DisableKeyword("FPS_RENDERING_ALLOWED");

            m_fpsCamera.m_camera.transform.parent.localRotation = Quaternion.Euler(HMD.GetVRCameraEulerRelativeToFPSCameraParent());
            m_fpsCamera.UpdateCameraRay();

            m_fpsCamera.UpdateLookatTeammates();

            VRPlayer.UpdateHeldItemTransform();

            DoUglyCameraHack();

            WeaponShellManager.EjectFPSShells();
            m_fpsCamera.m_owner.PositionHasBeenUpdated();

            GuiManager.Current.AfterCameraUpdate();
            m_fpsCamera.m_owner.Interaction.AfterCameraUpdate();
        }

        public static bool renderingFirstEye()
        {
            return SteamVR_Render.eye == SteamVR_Render.FirstEye;
        }

        private void PrepareFrameForEye(EVREye eye, SteamVR_CameraMask mask)
        {
            SkipDuplicateRenderTasks();

            DoUglyCameraHack();

            FixHeadAttachedFlashlightPos(eye);

            m_fpsCamera.m_cullingCamera.RunVisibilityOnPreCull();

            m_fpsCamera.m_preRenderCmds.Clear();
            m_fpsCamera.m_beforeForwardAlpahCmds.Clear();
            m_fpsCamera.m_preLightingCmds.Clear();

            m_fpsRender.ForceMatrixUpdate();

            gOverwrite.Clear();
            gOverwrite.DrawMesh(mask.meshFilter.mesh, mask.transform.localToWorldMatrix, m_occlusionMaterial);
           
            PrepareFrame();
        }
        private void SkipDuplicateRenderTasks()
        {
            // Shadows are rendered from the viewpoint of the light, and will be the same for both eyes.
            // UpdateCluster does something with the lights, but outputs a computer buffer that is the same for both eyes. Skip.
            ClusteredRendering.Current.UpdateShadows = renderingFirstEye();
            ClusteredRendering.Current.UpdateCluster = renderingFirstEye();

            // Mars is mostly compute shader stuff manually dispatched by EnvironmentStateManager.Update()
            // once per frame, or part of ClusteredRendering so it's already skipped.
            // Exterior skylight logic still runs, however.
            if ( ExteriorLight2.Current != null && ExteriorLight2.Current.enabled )
            {
                // Commandbuffer outputs a CascadedShadowmap that can be reused.
                // Always remove, then re-add for first first, leave out for second eye.
                ExteriorLight2.Current.m_light.RemoveCommandBuffer(LightEvent.AfterShadowMap, ExteriorLight2.Current.m_cmd);

                if (renderingFirstEye())
                {
                    // Restore shadows and CommandBuffer
                    ExteriorLight2.Current.m_light.shadows = LightShadows.Soft;
                    ExteriorLight2.Current.m_light.AddCommandBuffer(LightEvent.AfterShadowMap, ExteriorLight2.Current.m_cmd);
                }
                else
                {
                    // We are reusing the CascadedShadowmap the CommandBuffer would generate,
                    // so we don't need to it to render shadows for this eye.
                    ExteriorLight2.Current.m_light.shadows = LightShadows.None;
                }
            }

        }

        private void FixHeadAttachedFlashlightPos(EVREye eye)
        {
            if (FocusStateEvents.currentState == eFocusState.FPS)
            {
                PlayerInventoryBase inv = m_fpsCamera.m_owner.Inventory;
                if (inv.m_flashlightCLight == null || inv.m_flashlightCLight.m_unityLight == null)
                {
                    return;
                }

                if (!ItemEquippableEvents.CurrentItemHasFlashlight())
                {
                    inv.m_flashlightCLight.m_unityLight.transform.forward = HMD.GetWorldForward();
                    inv.m_flashlightCLight.m_unityLight.transform.position = HMD.Hmd.transform.TransformPoint(m_fpsCamera.m_owner.Inventory.m_flashlightCameraOffset);
                }
                else
                {
                    // No flashlight when you're downed so we need to check this or the whole render loop bugs out
                    if(inv.m_currentGearPartFlashlight == null || inv.m_currentGearPartFlashlight.m_lightAlign == null)
                    {
                        return;
                    }
                    var lightAlign = inv.m_currentGearPartFlashlight.m_lightAlign;
                    inv.m_flashlightCLight.m_unityLight.transform.forward = lightAlign.forward;
                    inv.m_flashlightCLight.m_unityLight.transform.position = lightAlign.position;
                }

                inv.m_flashlightCLight.UpdateTransformData();
            }
        }

        private void PrepareFrame()
        {
            if (AirParticleSystem.AirParticleSystem.Current != null)
                AirParticleSystem.AirParticleSystem.Current.CollectCommands(m_fpsCamera.m_preRenderCmds, m_fpsCamera.m_beforeForwardAlpahCmds);

            if (m_fpsCamera.CollectCommandsClustered)
            {
                ClusteredRendering.Current.CollectCommands(m_fpsCamera.m_preRenderCmds, m_fpsCamera.m_preLightingCmds);
            }
 
            // Generate textures that can be reused for the second eye
            if ( VRRendering.renderingFirstEye() )
            {
                // Splat/blood is rendered even if not displayed, so need to skip here too.
                if (VRConfig.configCameraBlood.Value && ScreenLiquidManager.LiquidSystem != null && m_fpsCamera.CollectCommandsScreenLiquid)
                {
                    ScreenLiquidManager.LiquidSystem.CollectCommands(m_fpsCamera.m_preRenderCmds);
                }

                // Terminal graphics
                if (m_fpsCamera.CollectCommandsGUIX && GUIX_Manager.isSetup)
                {
                    GUIX_Manager.Current.CollectCommands(m_fpsCamera.m_preRenderCmds);
                }
            }

            if (MapDetails.s_isSetup && MapDetails.s_isSetup)
            {
                MapDetails.Current.CollectCommands(m_fpsCamera.m_preRenderCmds);
            }

            Vector4 projectionParams = RenderUtils.GetProjectionParams(ClusteredRendering.Current.m_camera);
            Vector4 zbufferParams = RenderUtils.GetZBufferParams(ClusteredRendering.Current.m_camera);
            m_fpsCamera.m_preRenderCmds.SetGlobalVector(ClusteredRendering.ID_ProjectionParams, projectionParams);
            m_fpsCamera.m_preRenderCmds.SetGlobalVector(ClusteredRendering.ID_ZBufferParams, zbufferParams);

            Shader.SetGlobalMatrix("_MATRIX_V", m_fpsCamera.m_camera.worldToCameraMatrix);
            Shader.SetGlobalMatrix("_MATRIX_VP", GL.GetGPUProjectionMatrix(m_fpsCamera.m_camera.projectionMatrix, false) * m_fpsCamera.m_camera.worldToCameraMatrix);
            Shader.SetGlobalMatrix("_MATRIX_IV", this.transform.worldToLocalMatrix);
            Shader.SetGlobalInt("_FrameIndex", m_fpsCamera.m_frameIndex++);
            Shader.SetGlobalInt("_GameCameraActive", 1);
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