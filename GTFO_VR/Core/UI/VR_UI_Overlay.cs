using GTFO_VR.Core.VR_Input;
using GTFO_VR.Events;
using Il2CppInterop.Runtime.Attributes;
using System;
using UnityEngine;
using Valve.VR;
using static SteamVR_Utils;

namespace GTFO_VR.Core.UI
{
    /// <summary>
    /// Responsible for managing the UI overlay rendering for the main menu and map.
    /// </summary>
    public class VR_UI_Overlay : MonoBehaviour
    {
        public VR_UI_Overlay(IntPtr value)
: base(value) { }

        public static VR_UI_Overlay Current;

        Camera UI_Camera;
        Camera Popup_Camera;

        private ulong m_overlayHandle = OpenVR.k_ulOverlayHandleInvalid;

        private RigidTransform m_currentRigidTransform;

        private RenderTexture m_overlayTarget;

        private void Awake()
        {
            Log.Info("VR_UI_Overlay created...");
            if (Current)
            {
                Log.Error("Duplicate UI overlay handler!");
                return;
            }
            Current = this;

            Setup();
            OrientateOverlay();

            OpenVR.Compositor.FadeToColor(9999f, 0, 0, 0, 0, false);

            FocusStateEvents.OnFocusStateChange += FocusChanged;
        }

        [HideFromIl2Cpp]
        private void FocusChanged(eFocusState newState)
        {
           if(UI_Camera && Popup_Camera)
            {
                if(newState.Equals(eFocusState.GlobalPopupMessage))
                {
                    Popup_Camera.enabled = true;
                }
            }
        }

        private void Setup()
        {
            SetupOverlay();
        }

        [HideFromIl2Cpp]
        public bool GetPlayerPointingAtPositionOnScreen(out Vector2 uv)
        {
            if (Controllers.GetLocalPosition().magnitude < 0.01f)
            {
                uv = Vector2.zero;
                return false;
            }
            if (Current != null)
            {
                IntersectionResults result = new IntersectionResults();

                if (Current.ComputeIntersection(Controllers.GetLocalPosition(), Controllers.GetLocalAimForward(), ref result))
                {
                    uv = result.UVs;
                    return true;
                }
            }
            uv = Vector2.zero;
            return false;
        }

        private void Update()
        {
            if (UI_Core.m_uiPass && m_overlayTarget == null)
            {
                CreateRenderTexture();
                Popup_Camera = UI_Core.m_uiPass.transform.GetComponent<Camera>();
                UI_Camera = Popup_Camera.transform.GetComponentInChildren<UI_RenderUI>().gameObject.GetComponent<Camera>();
                return;
            }
            if (m_overlayHandle != OpenVR.k_ulOverlayHandleInvalid)
            {
                if (SteamVR_InputHandler.GetActionDown(InputAction.Crouch) || SteamVR_InputHandler.GetActionDown(InputAction.Aim))
                {
                    OrientateOverlay();
                }
                RenderUI();
            }

            // SteamVR_Render will set targetFrameRate to -1 in its Update(), resulting in unconstrainted framerate in overlay.
            // Each component sets the value when active/rendering, so we can set-and-forget.
            Application.targetFrameRate = (int)SteamVR.instance.hmd_DisplayFrequency;
        }

        internal void CreateRenderTexture()
        {
            Resolution res = SteamVR_Camera.GetResolutionForAspect(16, 9);
            m_overlayTarget = new RenderTexture(res.width, res.height, 16, RenderTextureFormat.ARGBFloat);
        }

        private void RenderUI()
        {
            if (m_overlayHandle != OpenVR.k_ulOverlayHandleInvalid)
            {
                UI_Camera.targetTexture = m_overlayTarget;
                UI_Camera.Render();
                var texture = new Texture_t
                {
                    handle = m_overlayTarget.GetNativeTexturePtr(),
                    eType = SteamVR.instance.textureType,
                    eColorSpace = EColorSpace.Auto
                };
                UI_Camera.targetTexture = null;
                OpenVR.Overlay.SetOverlayTexture(m_overlayHandle, ref texture);
            }
        }

        public struct IntersectionResults
        {
            public Vector3 point;
            public Vector3 normal;
            public Vector2 UVs;
            public float distance;
        }

        [HideFromIl2Cpp]
        public bool ComputeIntersection(Vector3 source, Vector3 direction, ref IntersectionResults results)
        {
            var overlay = OpenVR.Overlay;
            if (overlay == null)
                return false;

            var input = new VROverlayIntersectionParams_t();
            input.eOrigin = SteamVR.settings.trackingSpace;
            input.vSource.v0 = source.x;
            input.vSource.v1 = source.y;
            input.vSource.v2 = -source.z;
            input.vDirection.v0 = direction.x;
            input.vDirection.v1 = direction.y;
            input.vDirection.v2 = -direction.z;

            var output = new VROverlayIntersectionResults_t();
            if (!overlay.ComputeOverlayIntersection(m_overlayHandle, ref input, ref output))
                return false;

            results.point = new Vector3(output.vPoint.v0, output.vPoint.v1, -output.vPoint.v2);
            results.normal = new Vector3(output.vNormal.v0, output.vNormal.v1, -output.vNormal.v2);
            results.UVs = new Vector2(output.vUVs.v0, output.vUVs.v1);
            results.distance = output.fDistance;
            return true;
        }

        public void OrientateOverlay()
        {
            Log.Debug("Orienting overlay...");
            Quaternion rot = Quaternion.Euler(Vector3.Project(HMD.Hmd.transform.localRotation.eulerAngles, Vector3.up));
            transform.position = HMD.Hmd.transform.localPosition + rot * Vector3.forward * 2.3f;
            Vector3 Pos = transform.position;
            Pos.y = HMD.Hmd.transform.localPosition.y;

            transform.rotation = Quaternion.Euler(0f, rot.eulerAngles.y, 0f);
            transform.position = Pos;
            m_currentRigidTransform = new RigidTransform(transform);
            var t = m_currentRigidTransform.ToHmdMatrix34();
            OpenVR.Overlay.SetOverlayTransformAbsolute(m_overlayHandle, SteamVR.settings.trackingSpace, ref t);
        }

        public void SetupOverlay()
        {
            CVROverlay overlay = OpenVR.Overlay;
            if (overlay != null && m_overlayHandle == OpenVR.k_ulOverlayHandleInvalid)
            {
                m_overlayHandle = GetOverlayHandle("GTFO_Menu", transform, 5f);
            }
        }

        public void DestroyOverlay()
        {
            if (m_overlayHandle != OpenVR.k_ulOverlayHandleInvalid)
            {
                var overlay = OpenVR.Overlay;
                if (overlay != null)
                {
                    overlay.DestroyOverlay(m_overlayHandle);
                }

                m_overlayHandle = OpenVR.k_ulOverlayHandleInvalid;
            }
        }

        [HideFromIl2Cpp]
        // From SteamVR_LoadLevel
        // Helper to create (or reuse, if possible) each of our different overlay types.
        private ulong GetOverlayHandle(string overlayName, Transform transform, float widthInMeters = 1.0f)
        {
            ulong handle = OpenVR.k_ulOverlayHandleInvalid;

            var overlay = OpenVR.Overlay;
            if (overlay == null)
                return handle;

            var key = SteamVR_Overlay.key + "." + overlayName;

            var error = overlay.FindOverlay(key, ref handle);
            if (error != EVROverlayError.None)
                error = overlay.CreateOverlay(key, overlayName, ref handle);
            if (error == EVROverlayError.None)
            {
                overlay.ShowOverlay(handle);
                overlay.SetOverlayAlpha(handle, 1f);
                overlay.SetOverlayWidthInMeters(handle, widthInMeters);
                overlay.SetOverlayCurvature(handle, 0.3f);

                // Enables laser but disables all input for some reason? More research needed
                //overlay.SetOverlayInputMethod(handle, VROverlayInputMethod.Mouse);
                //overlay.SetOverlayFlag(handle, VROverlayFlags.MakeOverlaysInteractiveIfVisible, true);

                // D3D textures are upside-down in Unity to match OpenGL.
                if (SteamVR.instance.textureType == ETextureType.DirectX)
                {
                    var textureBounds = new VRTextureBounds_t
                    {
                        uMin = 0,
                        vMin = 1,
                        uMax = 1,
                        vMax = 0
                    };
                    overlay.SetOverlayTextureBounds(handle, ref textureBounds);
                }

                m_currentRigidTransform = new RigidTransform(transform);
                var t = m_currentRigidTransform.ToHmdMatrix34();

                overlay.SetOverlayTransformAbsolute(handle, SteamVR.settings.trackingSpace, ref t);
            }

            return handle;
        }
    }
}