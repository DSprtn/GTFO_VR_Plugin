using GTFO_VR.Core.PlayerBehaviours;
using GTFO_VR.Core.PlayerBehaviours.ForceTube;
using GTFO_VR.Core.PlayerBehaviours.BodyHaptics;
using GTFO_VR.Core.UI;
using GTFO_VR.Core.VR_Input;
using GTFO_VR.Events;
using Player;
using System;
using UnityEngine;
using Valve.VR;

namespace GTFO_VR.Core
{
    /// <summary>
    /// Responsible for seting up all VR related classes and handling focus state changes.
    /// </summary>
    public class VRSystems : MonoBehaviour
    {
        public VRSystems(IntPtr value)
: base(value) { }

        public static VRSystems Current;
        private static VR_UI_Overlay m_overlay;

        private VRPlayer m_player;

        private static FPSCamera m_currentFPSCameraRef;
        private static LocalPlayerAgent m_currentPlayerAgentRef;

        private void Awake()
        {
            if (Current)
            {
                Log.Error("Trying to create duplicate VRSystems class!");
                return;
            }
            Current = this;

#if DEBUG_GTFO_VR
            Log.Warning("Running a dev build!");
#endif 

            // Prevent SteamVR from adding a tracking script automatically. We handle this manually in VR_Input.HMD
            SteamVR_Camera.useHeadTracking = false;
            SteamVR_Settings.instance.poseUpdateMode = SteamVR_UpdateModes.OnLateUpdate;


            Setup();
            SteamVR_Camera.sceneResolutionScaleMultiplier = VRConfig.configRenderResolutionMultiplier.Value;
            VRConfig.configRenderResolutionMultiplier.SettingChanged += VRResolutionChanged;
            FocusStateEvents.OnFocusStateChange += FocusChanged;
            GuiManager.ForceOnResolutionChange();
        }


#if DEBUG_GTFO_VR
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.F11))
            {
                WeaponArchetypeVRData.LoadHapticsData();
            }
        }
#endif
        /// <summary>
        /// Some events like the checkpoint respawn are hard to handle in events. That's why we make do with this hack.
        /// </summary>
        /// <param name="__instance"></param>
        internal static void Heartbeat(LocalPlayerAgent __instance)
        {
            if(Current.m_player == null)
            {
                if(FocusStateEvents.currentState.Equals(eFocusState.FPS))
                {
                    m_currentPlayerAgentRef = __instance;
                    m_currentFPSCameraRef = __instance.FPSCamera;
                    Current.AppendVRComponents();
                    Current.HandleIngameFocus();
                }
            }
        }

        private void VRResolutionChanged(object sender, EventArgs e)
        {
            SteamVR_Camera.sceneResolutionScaleMultiplier = VRConfig.configRenderResolutionMultiplier.Value;
            if(ClusteredRendering.Current)
            {
                ClusteredRendering.Current.OnResolutionChange(new Resolution());
            }
        }

        private void Setup()
        {
            DontDestroyOnLoad(gameObject);
            if(!SteamVR.active)
            {
                Log.Info("Starting SteamVR...");
                SteamVR.Initialize(false);
            }
            var res = SteamVR_Camera.GetSceneResolution();
            Log.Info($"SteamVR Setup - HMD Res: {res.width}x{res.height}");
            WeaponArchetypeVRData.Setup();
            SteamVR_InputHandler.Setup();
            ForceTube.Setup();
            BodyHapticsIntegrator.Initialize();
            gameObject.AddComponent<HMD>();
            gameObject.AddComponent<Controllers>();
            gameObject.AddComponent<VRKeyboard>();
            gameObject.AddComponent<VRAssets>();
            // Delay the overlay setup so we don't 'hitch' the player's camera while everything else loads.
            Invoke(nameof(VRSystems.SetupOverlay), .5f);
        }

        public static void OnPlayerSpawned(FPSCamera fpsCamera, LocalPlayerAgent playerAgent)
        {
            if(!playerAgent.IsLocallyOwned)
            {
                Log.Error("Got playerAgent spawn but we aren't the owner!?");
            }
            Log.Info("Player and fpscamera have been spawned, references have been set...");
            m_currentFPSCameraRef = fpsCamera;
            m_currentPlayerAgentRef = playerAgent;

            // Normally we add VR components after the player has been spawned
            // If a player rejoins he is destroyed in the elevator and respawned, so we need to check if this happens and add VR components if needed
            if(FocusStateEvents.IsInGame())
            {
                Log.Debug("Player spawned while an in-game state was active");
                Current.HandleIngameFocus();
            }
        }

        private void SetupOverlay()
        {
            GameObject o = new GameObject("Overlay");
            DontDestroyOnLoad(o);
            m_overlay = o.AddComponent<VR_UI_Overlay>();
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
                ForceApplyVRControlSettings();
                HandleIngameFocus();
            }

            if (state.Equals(eFocusState.MainMenu) || state.Equals(eFocusState.Map) || state.Equals(eFocusState.GlobalPopupMessage))
            {
                HandleOutOfGameFocus();
            }

            ClearUIRenderTex();
        }

        private static void ForceApplyVRControlSettings()
        {
            // VR bindings require these to be disabled.
            // It is overriden if player accesses settings, so refresh when returning to FPS.
            CellSettingsManager.SetBoolValue(eCellSettingID.Gameplay_HoldToShowComsList, false);
            CellSettingsManager.SetBoolValue(eCellSettingID.Gameplay_CrouchToggle, false);
        }

        private void HandleOutOfGameFocus()
        {
            if (!m_overlay)
            {
                return;
            }
            Log.Debug("Focus change - Enabling overlay/Disabling player");
            ToggleOverlay(true);
            TogglePlayerCam(false);
        }

        private void HandleIngameFocus()
        {
            if (!m_overlay)
            {
                return;
            }

            try
            {
                if(!m_player)
                {
                    AppendVRComponents();
                }
            } catch(UnhollowerBaseLib.ObjectCollectedException e)
            {
                Log.Warning("Got GC'D object, falling back..." + e.ToString());
                m_player = null;
                AppendVRComponents();
            }

            ToggleOverlay(false);
            TogglePlayerCam(true);
        }

        private void AppendVRComponents()
        {
            if (m_currentFPSCameraRef == null || m_currentPlayerAgentRef == null)
            {
                Log.Warning("Tried to spawn player but FPS camera or playeragent ref was null, this should never happen! Falling back to FindObjectOfType");
                m_currentFPSCameraRef = FindObjectOfType<FPSCamera>();
                m_currentPlayerAgentRef = m_currentFPSCameraRef.m_owner;
            }

            m_player = m_currentFPSCameraRef.gameObject.AddComponent<VRPlayer>();
            m_player.Setup(m_currentFPSCameraRef, m_currentPlayerAgentRef);
        }

        private void ToggleOverlay(bool toggle)
        {
            if (!toggle)
            {
                m_overlay.DestroyOverlay();
            }
            else
            {
                m_overlay.SetupOverlay();
            }

            m_overlay.gameObject.SetActive(toggle);
            m_overlay.OrientateOverlay();
        }

        private void TogglePlayerCam(bool toggle)
        {
            if (VRConfig.configOculusCrashWorkaround.Value)
            {
                SteamVR_Render.pauseRendering = false;
                return;
            } 
            SteamVR_Render.pauseRendering = !toggle;
        }

        private void OnDestroy()
        {
            VRConfig.configRenderResolutionMultiplier.SettingChanged -= VRResolutionChanged;
            FocusStateEvents.OnFocusStateChange -= FocusChanged;
            BodyHapticsIntegrator.Destroy();
        }
    }
}