using BepInEx;
using HarmonyLib;
using BepInEx.Configuration;
using GTFO_VR.Core;
using System.Diagnostics;
using BepInEx.IL2CPP;
using System.Collections.Generic;
using UnhollowerRuntimeLib;
using GTFO_VR.UI;
using Mathf = SteamVR_Standalone_IL2CPP.Util.Mathf;
using BepInEx.Logging;
using Logger = BepInEx.Logging.Logger;
using GTFO_VR.Detours;
using GTFO_VR.Core.PlayerBehaviours;
using GTFO_VR.Core.VR_Input;
using GTFO_VR.Core.UI;

namespace GTFO_VR_BepInEx.Core
{
    [BepInPlugin(GUID, MODNAME, VERSION)]
    public class GTFO_VR_Plugin : BasePlugin
    {

        public const string
            MODNAME = "GTFO_VR_Plugin",
            AUTHOR = "Spartan",
            GUID = "com." + AUTHOR + "." + MODNAME,
            VERSION = "0.8.1";

        private ConfigEntry<bool> configEnableVR;
        private ConfigEntry<bool> configToggleVRBySteamVR;
        private ConfigEntry<bool> configUseControllers;
        private ConfigEntry<bool> configIRLCrouch;
        private ConfigEntry<bool> configUseLeftHand;
        private ConfigEntry<int>  configLightResMode;
        private ConfigEntry<bool> configAlternateEyeRendering;
        private ConfigEntry<bool> configUseTwoHanded;
        private ConfigEntry<bool> configAlwaysDoubleHanded;
        private ConfigEntry<float> configSnapTurnAmount;
        private ConfigEntry<bool> configSmoothSnapTurn;
        private ConfigEntry<float> configWatchScaling;
        private ConfigEntry<bool> configUseNumbersForAmmoDisplay;
        private ConfigEntry<string> configWatchColorHex;
        private ConfigEntry<float> configCrouchHeight;

        public static ManualLogSource log;


        public override void Load()
        {
            log = Logger.CreateLogSource(MODNAME);
            log.LogInfo("Loading VR plugin...");
            SetupConfig();
            Harmony harmony = new Harmony("com.github.dsprtn.gtfovr");

            if (VR_Settings.enabled && SteamVRRunningCheck())
            {
                SetupClassInjections();
                TerminalInputDetours.HookAll();
                BioscannerDetours.HookAll();
                //PlayerSyncDetour.HookAll();
                harmony.PatchAll();
            }
            else
            {
                log.LogWarning("VR launch aborted, VR is disabled or SteamVR is off!");
            }
        }

       

        void SetupClassInjections()
        {
            ClassInjector.RegisterTypeInIl2Cpp<VR_Assets>();
            ClassInjector.RegisterTypeInIl2Cpp<VR_Global>();
            ClassInjector.RegisterTypeInIl2Cpp<LaserPointer>();
            ClassInjector.RegisterTypeInIl2Cpp<PlayerOrigin>();
            ClassInjector.RegisterTypeInIl2Cpp<PlayerVR>();
            ClassInjector.RegisterTypeInIl2Cpp<Snapturn>();
            ClassInjector.RegisterTypeInIl2Cpp<VR_Keyboard>();
            ClassInjector.RegisterTypeInIl2Cpp<DividedBarShaderController>();
            ClassInjector.RegisterTypeInIl2Cpp<VR_UI_Overlay>();
            ClassInjector.RegisterTypeInIl2Cpp<VRWorldSpaceUI>();
            ClassInjector.RegisterTypeInIl2Cpp<Watch>();
            ClassInjector.RegisterTypeInIl2Cpp<Controllers>();
            ClassInjector.RegisterTypeInIl2Cpp<HMD>();
            ClassInjector.RegisterTypeInIl2Cpp<SteamVR_InputHandler>();
        }

        private bool SteamVRRunningCheck()
        {
            if(!VR_Settings.toggleVRBySteamVRRunning)
            {
                return true;
            }

            List<Process> possibleVRProcesses = new List<Process>();

            possibleVRProcesses.AddRange(Process.GetProcessesByName("vrserver"));
            possibleVRProcesses.AddRange(Process.GetProcessesByName("vrcompositor"));

            log.LogDebug("VR processes found - " + possibleVRProcesses.Count);
            foreach(Process p in possibleVRProcesses)
            {
                log.LogDebug(p.ToString());
            }
            return possibleVRProcesses.Count > 0;
        }



        private void SetupConfig()
        {
            configEnableVR = Config.Bind("Startup", "Run VR plugin?", true, "If true, game will start in VR");
            configToggleVRBySteamVR = Config.Bind("Startup", "Start in pancake if SteamVR is off?", true, "If true, will start the game in pancake mode if SteamVR is not detected");

            configUseControllers = Config.Bind("Input", "Use VR Controllers?", true, "If true, will use VR controllers. You can play with a gamepad and head aiming if you set this to false");
            configIRLCrouch = Config.Bind("Input", "Crouch in-game when you crouch IRL?", true, "If true, when crouching down below a certain threshold IRL, the in-game character will also crouch");
            configUseLeftHand = Config.Bind("Input", "Use left hand as main hand?", false, "If true, all items will appear in the left hand");
            configLightResMode = Config.Bind("Experimental performance tweaks", "Light render resolution tweak - the lower resolution the greater the performance gain!", 1, "0 = Native HMD resolution 1 = 1920x1080, 2 = 1024x768 (Small artifacting, big performance increase), \n 3=640x480 (medium artifacting on lights, great performance increase)");
            configUseTwoHanded = Config.Bind("Input", "Use both hands to aim?", true, "If true, two-handed weapons will be allowed to be aimed with both hands.");
            configAlwaysDoubleHanded = Config.Bind("Input", "Always use double handed aiming? (Where it applies)", false, "If true, double handed weapons will always use double handed aiming (RECOMMENDED FOR GUN STOCK USERS)");
            configSnapTurnAmount = Config.Bind("Input", "Snap turn angle", 60f, "The amount of degrees to turn on a snap turn (or turn per half a second if smooth turn is enabled)");
            configSmoothSnapTurn = Config.Bind("Input", "Use smooth turning?", false, "If true, will use smooth turn instead of snap turn");
            configWatchScaling = Config.Bind("Misc", "Watch scale multiplier", 1.00f, "Size of the watch in-game will be multiplied by this value down to half of its default size or up to double (0.5 or 2.0)");
            configUseNumbersForAmmoDisplay = Config.Bind("Misc", "Use numbers for ammo display?", false, "If true, current ammo and max ammo will be displayed as numbers on the watch");
            configWatchColorHex = Config.Bind("Misc", "Hex color to use for watch", "#ffffff", "Google hexcolor and paste whatever color you want here");
            configCrouchHeight = Config.Bind("Input", "Crouch height in meters", 1.15f, "In-game character will be crouching if your head is lower than this height above the playspace (clamped to 1-1.35m)");
            configAlternateEyeRendering = Config.Bind("Experimental performance tweaks", "Alternate light and shadow rendering per frame per eye", false, "If true will alternate between eyes when drawing lights and shadows each frame, \n might look really janky so only use this if you absolutely want to play this in VR but don't have the rig for it!");


            log.LogDebug("VR enabled?" + configEnableVR.Value);
            log.LogDebug("Toggle VR by SteamVR running?" + configToggleVRBySteamVR.Value);
            log.LogDebug("Use VR Controllers? : " + configUseControllers.Value);
            log.LogDebug("Crouch on IRL crouch? : " + configIRLCrouch.Value);
            log.LogDebug("Use left hand as main hand? : " + configUseLeftHand.Value);
            log.LogDebug("Light resolution mode: " + configLightResMode.Value.ToString());
            log.LogDebug("Use two handed aiming: " + configUseTwoHanded.Value);
            log.LogDebug("Start with double handed aiming: " + configAlwaysDoubleHanded.Value);
            log.LogDebug("Snapturn amount: " + configSnapTurnAmount.Value);
            log.LogDebug("Use smooth turn?: " + configSmoothSnapTurn.Value);
            log.LogDebug("Watch size multiplier: " + configWatchScaling.Value);
            log.LogDebug("Use numbers for number display?: " + configUseNumbersForAmmoDisplay.Value);
            log.LogDebug("Watch color - " + configWatchColorHex.Value);
            log.LogDebug("Crouching height - " + configCrouchHeight.Value);
            log.LogDebug("Alternate eye rendering? - " + configAlternateEyeRendering.Value);

            VR_Settings.useVRControllers = configUseControllers.Value;
            VR_Settings.crouchOnIRLCrouch = configIRLCrouch.Value;
            VR_Settings.lightRenderMode = configLightResMode.Value;
            VR_Settings.twoHandedAimingEnabled = configUseTwoHanded.Value;
            VR_Settings.alwaysDoubleHanded = configAlwaysDoubleHanded.Value;
            VR_Settings.snapTurnAmount = configSnapTurnAmount.Value;
            VR_Settings.useSmoothTurn = configSmoothSnapTurn.Value;
            VR_Settings.watchScale = Mathf.Clamp(configWatchScaling.Value, 0.5f, 2f);
            VR_Settings.toggleVRBySteamVRRunning = configToggleVRBySteamVR.Value;
            VR_Settings.useNumbersForAmmoDisplay = configUseNumbersForAmmoDisplay.Value;
            VR_Settings.watchColor = ColorExt.Hex(configWatchColorHex.Value);
            VR_Settings.IRLCrouchBorder = Mathf.Clamp(configCrouchHeight.Value, 1f, 1.35f);
            VR_Settings.alternateLightRenderingPerEye = configAlternateEyeRendering.Value;

            if (configUseLeftHand.Value)
            {
                VR_Settings.mainHand = HandType.Left;
            }
        }

    }
}
