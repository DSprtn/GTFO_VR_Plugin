using GTFO_VR.Core;
using GTFO_VR.Events;
using GTFO_VR.Input;
using GTFO_VR.Util;
using GTFO_VR_BepInEx.Core;
using Player;
using SteamVR_Standalone_IL2CPP.Util;
using System;
using System.Text;
using UnhollowerRuntimeLib;
using UnityEngine;
using Valve.VR;



namespace GTFO_VR
{
    public class VR_Global : MonoBehaviour
    {

        public VR_Global(IntPtr value)
: base(value) { }

        public static VR_Global instance;

        public static bool VR_ENABLED;

        public static bool Overlay_Active = true;

        static PlayerVR ingamePlayer;

        static VR_UI_Overlay overlay;

        public static bool hackingToolRenderingOverriden;

        public static Resolution VR_Resolution;


        void Awake()
        {
            if (!instance)
            {
                instance = this;
            } else
            {
                GTFO_VR_Plugin.log.LogError("Trying to create duplicate VRGlobal class");
                return;
            }
            // Prevent SteamVR from adding a tracking script automatically. We handle this manually in HMD
            SteamVR_Camera.useHeadTracking = false;
           
            FocusStateEvents.OnFocusStateChange += FocusChanged;
            Setup();
            SteamVR_Settings.instance.poseUpdateMode = SteamVR_UpdateModes.OnLateUpdate;
        }




        public static bool GetPlayerPointingAtPositionOnScreen(out Vector2 uv)
        {
            if(Controllers.GetLocalPosition().magnitude < 0.01f)
            {
                uv = Vector2.zero;
                return false;
            }
            if(overlay)
            {
                VR_UI_Overlay.IntersectionResults result = new VR_UI_Overlay.IntersectionResults();

                if(overlay.ComputeIntersection(Controllers.GetLocalPosition(), Controllers.GetLocalAimForward(), ref result))
                {
                    uv = result.UVs;
                    return true;
                }
            }
            uv = Vector2.zero;
            return false;
        }

        private void Setup()
        {
            SteamVR.Initialize(false);
            WeaponArchetypeVRData.Setup();
            gameObject.AddComponent<VRInput>();
            gameObject.AddComponent<HMD>();
            gameObject.AddComponent<Controllers>();
            gameObject.AddComponent<VR_Keyboard>();
            gameObject.AddComponent<VR_Assets>();
            VR_Resolution = new Resolution
            {
                height = (int)SteamVR.instance.sceneHeight,
                width = (int)SteamVR.instance.sceneWidth
            };
            Invoke(nameof(VR_Global.SetupOverlay), .25f);
            DontDestroyOnLoad(gameObject);
        }

        void SetupOverlay()
        {
            GameObject o = new GameObject("Overlay");
            DontDestroyOnLoad(o);
            overlay = o.AddComponent<VR_UI_Overlay>();
        }

        public static void ClearUIRenderTex()
        {

            RenderTexture rt = RenderTexture.active;
            RenderTexture.active = UI_Core.UIPassHUD.Camera.targetTexture;
            GL.Clear(true, true, Color.clear);
            RenderTexture.active = rt;
        }

        public void FocusChanged(eFocusState state)
        {
            if (state.Equals(eFocusState.FPS) || state.Equals(eFocusState.InElevator))
            {
                HandleIngameFocus();
            }

            if(state.Equals(eFocusState.MainMenu) || state.Equals(eFocusState.Map)) {
                HandleOutOfGameFocus();
            }
            ClearUIRenderTex();
        }

        private void HandleOutOfGameFocus()
        {
            if(!overlay)
            {
                return;
            }

            ToggleOverlay(true);
            TogglePlayerCam(false);
        }

        private void HandleIngameFocus()
        {
            if(!overlay)
            {
                return;
            }
            if(ingamePlayer == null)
            {
                GTFO_VR_Plugin.log.LogInfo("Creating VR Player...");

                ingamePlayer = new GameObject("VR_Player").AddComponent<PlayerVR>();
            }
           
            ToggleOverlay(false);
            TogglePlayerCam(true);
            ClusteredRendering.Current.OnResolutionChange(new Resolution());
        }

        void ToggleOverlay(bool toggle)
        {
            if(!toggle)
            {
                overlay.DestroyOverlay();
            } else
            {
                overlay.SetupOverlay();
            }
            
            overlay.gameObject.SetActive(toggle);
            overlay.OrientateOverlay();
        }

        void TogglePlayerCam(bool toggle)
        {
            PlayerVR.LoadedAndInIngameView = toggle;
            SteamVR_Render.pauseRendering = !toggle;
            Invoke(nameof(VR_Global.DisableUnneccessaryCams), .1f);

        }

        void DisableUnneccessaryCams()
        {
            if(PlayerVR.VRCamera && PlayerVR.VRCamera.head)
            {
                foreach (Camera cam in PlayerVR.VRCamera.transform.root.GetComponentsInChildren<Camera>())
                {
                    cam.enabled = false;
                }
            }
        }

        void OnDestroy()
        {
            FocusStateEvents.OnFocusStateChange -= FocusChanged;
        }
    }
}
