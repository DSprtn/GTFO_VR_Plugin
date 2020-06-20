using GTFO_VR.Events;
using GTFO_VR.Input;
using Player;
using System;
using UnityEngine;
using Valve.VR;
using Valve.VR.Extras;
using static SteamVR_Utils;

namespace GTFO_VR
{
    public class VR_UI_Overlay : MonoBehaviour
    {

        public static VR_UI_Overlay instance;

        public static bool Overlay_Active = true;

        private ulong overlayHandle = OpenVR.k_ulOverlayHandleInvalid;

        public static UI_Pass UI_ref;

        RigidTransform current;

        void Awake()
        {
            if(instance)
            {
                Debug.LogError("Duplicate UI overlay handler!");
                return;
            }
            instance = this;
            
            Setup();
            OrientateOverlay();

            OpenVR.Compositor.FadeToColor(9999f, 0, 0, 0, 0, false);
        }

        private void Setup()
        {
            SetupOverlay();

        }

        void Update()
        {
            if(overlayHandle != OpenVR.k_ulOverlayHandleInvalid)
            {
                var texture = new Texture_t
                {
                    handle = UI_ref.m_UIRenderTarget.GetNativeTexturePtr(),
                    eType = SteamVR.instance.textureType,
                    eColorSpace = EColorSpace.Auto
                };
                OpenVR.Overlay.SetOverlayTexture(overlayHandle, ref texture);

                if (VRInput.GetActionDown(InputAction.Crouch))
                {
                    OrientateOverlay();
                }
            } 
        }

        public struct IntersectionResults
        {
            public Vector3 point;
            public Vector3 normal;
            public Vector2 UVs;
            public float distance;
        }

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
            if (!overlay.ComputeOverlayIntersection(overlayHandle, ref input, ref output))
                return false;

            results.point = new Vector3(output.vPoint.v0, output.vPoint.v1, -output.vPoint.v2);
            results.normal = new Vector3(output.vNormal.v0, output.vNormal.v1, -output.vNormal.v2);
            results.UVs = new Vector2(output.vUVs.v0, output.vUVs.v1);
            results.distance = output.fDistance;
            return true;
        }

        public void OrientateOverlay()
        {
            Debug.Log("Orienting overlay...");
            Quaternion rot = Quaternion.Euler(Vector3.Project(HMD.hmd.transform.localRotation.eulerAngles, Vector3.up));
            transform.position = HMD.hmd.transform.localPosition + rot * Vector3.forward * 2.2f;
            Vector3 Pos = transform.position;
            Pos.y = HMD.hmd.transform.localPosition.y;
            

            transform.rotation = Quaternion.Euler(0f, rot.eulerAngles.y, 0f);
            transform.position = Pos;

            current = new SteamVR_Utils.RigidTransform(transform);
            var t = current.ToHmdMatrix34();
            OpenVR.Overlay.SetOverlayTransformAbsolute(overlayHandle, SteamVR.settings.trackingSpace, ref t);
        }

        public void SetupOverlay()
        {
            CVROverlay overlay = OpenVR.Overlay;
            if(overlay != null && overlayHandle == OpenVR.k_ulOverlayHandleInvalid)
            {
                overlayHandle = GetOverlayHandle("GTFO_Menu", transform, 5f);
            }
        }

        public void DestroyOverlay()
        {
            if (overlayHandle != OpenVR.k_ulOverlayHandleInvalid)
            {
                var overlay = OpenVR.Overlay;
                if (overlay != null)
                {
                    overlay.DestroyOverlay(overlayHandle);
                }

                overlayHandle = OpenVR.k_ulOverlayHandleInvalid;
            }
        }

        // From SteamVR_LoadLevel
        // Helper to create (or reuse if possible) each of our different overlay types.
        ulong GetOverlayHandle(string overlayName, Transform transform, float widthInMeters = 1.0f)
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


                current = new SteamVR_Utils.RigidTransform(transform);
                var t = current.ToHmdMatrix34();

                overlay.SetOverlayTransformAbsolute(handle, SteamVR.settings.trackingSpace, ref t);
            }

            return handle;
        }


    }
}
