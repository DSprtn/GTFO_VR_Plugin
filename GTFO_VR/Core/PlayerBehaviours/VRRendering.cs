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
        private UI_Apply m_uiBlitter;
        private static bool m_skipLeftEye;

        private void Awake()
        {
            m_fpsCamera = GetComponent<FPSCamera>();
            m_uiBlitter = GetComponent<UI_Apply>();
            SteamVR_Render.eyePreRenderCallback += PrepareFrameForEye;
        }

        private void PrepareFrameForEye(EVREye eye)
        {
            if (Time.frameCount % 2 == 0)
            {
                m_skipLeftEye = true;
            }
            else
            {
                m_skipLeftEye = false;
            }

            DoUglyCameraHack();

            FixHeadAttachedFlashlightPos();

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

        private void FixHeadAttachedFlashlightPos()
        {
            if(FocusStateEvents.currentState == eFocusState.FPS && !ItemEquippableEvents.CurrentItemHasFlashlight())
            {
                Transform flashlight = m_fpsCamera.m_owner.Inventory.m_flashlight.transform;
                flashlight.position = HMD.Hmd.transform.TransformPoint(m_fpsCamera.m_owner.Inventory.m_flashlightCameraOffset + new Vector3(0,0,-.1f));
            }
        }

        private void PrepareFrame()
        {

            UI_Core.RenderUI();
            

            if (ScreenLiquidManager.LiquidSystem != null)
                ScreenLiquidManager.LiquidSystem.CollectCommands(m_fpsCamera.m_preRenderCmds);
            if (AirParticleSystem.AirParticleSystem.Current != null)
                AirParticleSystem.AirParticleSystem.Current.CollectCommands(m_fpsCamera.m_preRenderCmds, m_fpsCamera.m_beforeForwardAlpahCmds);

            if (m_fpsCamera.m_collectCommandsClustered)
            {
                ClusteredRendering.Current.CollectCommands(m_fpsCamera.m_preRenderCmds);
            }

            if (m_fpsCamera.m_collectCommandsGUIX)
            {
                GUIX_Manager.Current.CollectCommands(m_fpsCamera.m_preRenderCmds);
            }

            if (MapDetails.s_isSetup && m_fpsCamera.m_collectCommandsMap)
            {
                MapDetails.Current.CollectCommands(m_fpsCamera.m_preRenderCmds);
            }

            Vector4 projectionParams = ClusteredRendering.GetProjectionParams(ClusteredRendering.Current.m_camera);
            Vector4 zbufferParams = ClusteredRendering.GetZBufferParams(ClusteredRendering.Current.m_camera);
            m_fpsCamera.m_preRenderCmds.SetGlobalVector(ClusteredRendering.ID_ProjectionParams, projectionParams);
            m_fpsCamera.m_preRenderCmds.SetGlobalVector(ClusteredRendering.ID_ZBufferParams, zbufferParams);
        }

        // Force FOV/Aspects and position match up for all relevant game cameras
        private void DoUglyCameraHack()
        {
            if(m_uiBlitter != null)
            {
                m_uiBlitter.enabled = VRConfig.configCameraBlood.Value;
            }
            
            m_fpsCamera.PlayerMoveEnabled = true;
            m_fpsCamera.MouseLookEnabled = true;

            if (m_fpsCamera != null && m_fpsCamera.m_camera != null)
            {
                m_fpsCamera.m_camera.fieldOfView = SteamVR.instance.fieldOfView;
                m_fpsCamera.m_camera.aspect = SteamVR.instance.aspect;
                if (m_fpsCamera.m_itemCamera != null)
                {
                    m_fpsCamera.m_itemCamera.fieldOfView = SteamVR.instance.fieldOfView;
                    m_fpsCamera.m_itemCamera.aspect = SteamVR.instance.aspect;
                }
            }

            if (ClusteredRendering.Current != null && ClusteredRendering.Current.m_lightBufferCamera != null)
            {
                // ToDO - Do we need to set the light buffer camera's properties?
                ClusteredRendering.Current.m_lightBufferCamera.fieldOfView = SteamVR.instance.fieldOfView;
                ClusteredRendering.Current.m_lightBufferCamera.aspect = SteamVR.instance.aspect;
                ClusteredRendering.Current.m_lightBufferCamera.transform.position = m_fpsCamera.m_camera.transform.position;
                ClusteredRendering.Current.m_lightBufferCamera.transform.rotation = m_fpsCamera.m_camera.transform.rotation;


                ClusteredRendering.Current.m_camera.fieldOfView = SteamVR.instance.fieldOfView;
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
            SteamVR_Render.eyePreRenderCallback -= PrepareFrameForEye;
        }
    }
}