using GTFO_VR.Core.PlayerBehaviours;
using GTFO_VR.Core.UI;
using GTFO_VR.Core.VR_Input;
using GTFO_VR.Events;
using System;
using UnityEngine;
using Valve.VR;



namespace GTFO_VR.Core
{
    /// <summary>
    /// Responsible for seting up all VR related classes and handling focus state changes.
    /// </summary>
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


        void Awake()
        {
            if (!instance)
            {
                instance = this;
            }
            else
            {
                GTFO_VR_Plugin.log.LogError("Trying to create duplicate VRGlobal class");
                return;
            }
            // Prevent SteamVR from adding a tracking script automatically. We handle this manually in VR_Input.HMD
            SteamVR_Camera.useHeadTracking = false;
            SteamVR_Settings.instance.poseUpdateMode = SteamVR_UpdateModes.OnLateUpdate;

            FocusStateEvents.OnFocusStateChange += FocusChanged;
            Setup();
        }

        private void Setup()
        {
            DontDestroyOnLoad(gameObject);

            SteamVR.Initialize(false);
            WeaponArchetypeVRData.Setup();
            gameObject.AddComponent<SteamVR_InputHandler>();
            gameObject.AddComponent<HMD>();
            gameObject.AddComponent<Controllers>();
            gameObject.AddComponent<VR_Keyboard>();
            gameObject.AddComponent<VR_Assets>();
            // Delay the overlay setup so we don't 'hitch' the player's camera while everything else loads.
            Invoke(nameof(VR_Global.SetupOverlay), .5f);
        }

        void SetupOverlay()
        {
            GameObject o = new GameObject("Overlay");
            DontDestroyOnLoad(o);
            overlay = o.AddComponent<VR_UI_Overlay>();
        }

        /// <summary>
        /// Prevent reprojection when entering/exiting map or menu by clearing the associated UI render texture
        /// </summary>
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

            if (state.Equals(eFocusState.MainMenu) || state.Equals(eFocusState.Map))
            {
                HandleOutOfGameFocus();
            }
            ClearUIRenderTex();
        }

        private void HandleOutOfGameFocus()
        {
            if (!overlay)
            {
                return;
            }

            ToggleOverlay(true);
            TogglePlayerCam(false);
        }

        private void HandleIngameFocus()
        {
            if (!overlay)
            {
                return;
            }
            if (ingamePlayer == null)
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
            if (!toggle)
            {
                overlay.DestroyOverlay();
            }
            else
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
        }


        void OnDestroy()
        {
            FocusStateEvents.OnFocusStateChange -= FocusChanged;
        }
    }
}
