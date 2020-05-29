using GTFO_VR.Events;
using GTFO_VR.Input;
using Player;
using System;
using UnityEngine;
using Valve.VR;
using Valve.VR.Extras;

namespace GTFO_VR
{
    public class VR_UI_Overlay : MonoBehaviour
    {

        public static VR_UI_Overlay instance;

        public static bool Overlay_Active = true;

        private ulong overlayHandle = OpenVR.k_ulOverlayHandleInvalid;

        public static UI_Pass UI_ref;

        void Awake()
        {
            if(instance)
            {
                Debug.LogError("Duplicate UI overlay handler!");
                return;
            }
            instance = this;
            OrientateOverlay();
            Setup();

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

        public void OrientateOverlay()
        {
            Quaternion rot = Quaternion.Euler(Vector3.Project(HMD.hmd.transform.rotation.eulerAngles, Vector3.up));
            transform.position = HMD.hmd.transform.localPosition + rot * Vector3.forward * 3.5f;
            Vector3 Pos = transform.position;
            Pos.y = HMD.hmd.transform.position.y;
            transform.rotation = Quaternion.LookRotation(HMD.hmd.transform.forward);//Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(-HMD.hmd.transform.forward), 0.2f);

            transform.rotation = Quaternion.Euler(0f, transform.rotation.eulerAngles.y, 0f);
            transform.position = Pos;


                
            //var offset = new SteamVR_Utils.RigidTransform(HMD.hmd.transform, transform);

            //var t = offset.ToHmdMatrix34();

            var t = new SteamVR_Utils.RigidTransform(transform).ToHmdMatrix34();
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


                var t = new SteamVR_Utils.RigidTransform(transform).ToHmdMatrix34();

                overlay.SetOverlayTransformAbsolute(handle, SteamVR.settings.trackingSpace, ref t);
                    
                
            }

            return handle;
        }


    }
}
