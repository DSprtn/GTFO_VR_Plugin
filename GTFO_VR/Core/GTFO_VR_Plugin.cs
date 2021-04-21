using BepInEx;
using BepInEx.Configuration;
using BepInEx.IL2CPP;
using GTFO_VR.Core.PlayerBehaviours;
using GTFO_VR.Core.UI;
using GTFO_VR.Core.VR_Input;
using GTFO_VR.Detours;
using GTFO_VR.UI;
using HarmonyLib;
using System.Collections.Generic;
using System.Diagnostics;
using UnhollowerRuntimeLib;
using Mathf = SteamVR_Standalone_IL2CPP.Util.Mathf;

namespace GTFO_VR.Core
{
    /// <summary>
    /// Main entry point of the mod. Responsible for managing the config and running all patches if the mod is enabled.
    /// </summary>
    [BepInPlugin(GUID, MODNAME, VERSION)]
    public class GTFO_VR_Plugin : BasePlugin
    {
        public const string
            MODNAME = "GTFO_VR_Plugin",
            AUTHOR = "Spartan",
            GUID = "com." + AUTHOR + "." + MODNAME,
            VERSION = "0.8.4.0";

        public static GTFO_VR_Plugin Current;

        public override void Load()
        {
            Current = this;

            Core.Log.Setup(BepInEx.Logging.Logger.CreateLogSource(MODNAME));
            Core.Log.Info("Loading VR plugin...");
            SetupConfig();

            if (VRSettings.VREnabled && SteamVRRunningCheck())
            {
                InjectVR();
            }
            else
            {
                Log.LogWarning("VR launch aborted, VR is disabled or SteamVR is off!");
            }
        }

        private void InjectVR()
        {
            Harmony harmony = new Harmony("com.github.dsprtn.gtfovr");
            SetupIL2CPPClassInjections();
            TerminalInputDetours.HookAll();
            BioscannerDetours.HookAll();
            harmony.PatchAll();
        }

        private void SetupIL2CPPClassInjections()
        {
            ClassInjector.RegisterTypeInIl2Cpp<VRAssets>();
            ClassInjector.RegisterTypeInIl2Cpp<VRSystems>();
            ClassInjector.RegisterTypeInIl2Cpp<VRRendering>();
            ClassInjector.RegisterTypeInIl2Cpp<CollisionFade>();
            ClassInjector.RegisterTypeInIl2Cpp<LaserPointer>();
            ClassInjector.RegisterTypeInIl2Cpp<PlayerOrigin>();
            ClassInjector.RegisterTypeInIl2Cpp<VRPlayer>();
            ClassInjector.RegisterTypeInIl2Cpp<Snapturn>();
            ClassInjector.RegisterTypeInIl2Cpp<VRKeyboard>();
            ClassInjector.RegisterTypeInIl2Cpp<DividedBarShaderController>();
            ClassInjector.RegisterTypeInIl2Cpp<VR_UI_Overlay>();
            ClassInjector.RegisterTypeInIl2Cpp<VRWorldSpaceUI>();
            ClassInjector.RegisterTypeInIl2Cpp<Watch>();
            ClassInjector.RegisterTypeInIl2Cpp<Controllers>();
            ClassInjector.RegisterTypeInIl2Cpp<HMD>();
        }

        private bool SteamVRRunningCheck()
        {
            if (!VRSettings.toggleVRBySteamVRRunning)
            {
                return true;
            }

            List<Process> possibleVRProcesses = new List<Process>();

            possibleVRProcesses.AddRange(Process.GetProcessesByName("vrserver"));
            possibleVRProcesses.AddRange(Process.GetProcessesByName("vrcompositor"));

            Core.Log.Debug("VR processes found - " + possibleVRProcesses.Count);
            foreach (Process p in possibleVRProcesses)
            {
                Core.Log.Debug(p.ToString());
            }
            return possibleVRProcesses.Count > 0;
        }

        public ConfigEntry<bool> configEnableVR;
        public ConfigEntry<bool> configToggleVRBySteamVR;
        public ConfigEntry<bool> configUseControllers;
        public ConfigEntry<bool> configIRLCrouch;
        public ConfigEntry<bool> configUseLeftHand;
        public ConfigEntry<int> configLightResMode;
        public ConfigEntry<bool> configAlternateEyeRendering;
        public ConfigEntry<bool> configUseTwoHanded;
        public ConfigEntry<bool> configAlwaysDoubleHanded;
        public ConfigEntry<float> configSnapTurnAmount;
        public ConfigEntry<bool> configSmoothSnapTurn;
        public ConfigEntry<float> configWatchScaling;
        public ConfigEntry<bool> configUseNumbersForAmmoDisplay;
        public ConfigEntry<string> configWatchColorHex;
        public ConfigEntry<float> configCrouchHeight;
        public ConfigEntry<bool> configRecenterPlayspaceDuringSmoothTurn;
        public ConfigEntry<bool> configUseLaserPointerOnWeapons;
        public ConfigEntry<bool> configUseHapticsForShooting;
        public ConfigEntry<string> configLaserPointerColorHex;
        public ConfigEntry<float> configShootingHapticsStrength;
        public ConfigEntry<float> configWeaponRotationOffset;

        private void SetupConfig()
        {
            configEnableVR = Config.Bind("Startup", "Run VR plugin?", true, "If true, game will start in VR");
            configToggleVRBySteamVR = Config.Bind("Startup", "Start in pancake if SteamVR is off?", true, "If true, will start the game in pancake mode if SteamVR is not detected");
            configUseControllers = Config.Bind("Input", "Use VR Controllers?", true, "If true, will use VR controllers. You can play with a gamepad and head aiming if you set this to false");
            configIRLCrouch = Config.Bind("Input", "Crouch in-game when you crouch IRL?", true, "If true, when crouching down below a certain threshold IRL, the in-game character will also crouch");
            configUseLeftHand = Config.Bind("Input", "Use left hand as main hand?", false, "If true, all items will appear in the left hand");

            configLightResMode = Config.Bind("Experimental performance tweaks", "Light/fog render resolution tweak - the lower the resolution the greater the performance gain!", 1, 
                "0 = Native HMD resolution, looks great but requires a beastly PC" +
                "\n1 = 75% resolution (light shimmering on fog, performance increase)" +
                "\n2 = 60% resolution (Medium shimmering, small light artifacts, big performance increase)" +
                "\n3 = 30% resolution (Medium artifacting on lights and fog, great performance increase)");

            configUseTwoHanded = Config.Bind("Input", "Use both hands to aim?", true, "If true, two-handed weapons will be allowed to be aimed with both hands.");
            configAlwaysDoubleHanded = Config.Bind("Input", "Always use double handed aiming? (Where it applies)", false, "If true, double handed weapons will always use double handed aiming (RECOMMENDED FOR GUN STOCK USERS)");
            configSnapTurnAmount = Config.Bind("Input", "Snap turn angle", 60f, "The amount of degrees to turn on a snap turn (or turn per half a second if smooth turn is enabled)");
            configSmoothSnapTurn = Config.Bind("Input", "Use smooth turning?", false, "If true, will use smooth turn instead of snap turn");
            configRecenterPlayspaceDuringSmoothTurn = Config.Bind("Input", "Recenter playspace on smoothturn?", true, "If true, will recenter the player collision and playspace during turning. \nMight result in a little bit of teleporting on the first frame you use smoothturn, but will keep collisions in sync better.");
            configWatchScaling = Config.Bind("Misc", "Watch scale multiplier", 1.00f, "Size of the watch in-game will be multiplied by this value down to half of its default size or up to double (0.5 or 2.0)");
            configUseNumbersForAmmoDisplay = Config.Bind("Misc", "Use numbers for ammo display?", false, "If true, current ammo and max ammo will be displayed as numbers on the watch");
            configWatchColorHex = Config.Bind("Misc", "Hex color to use for watch", "#ffffff", "Google hexcolor and paste whatever color you want here");
            configCrouchHeight = Config.Bind("Input", "Crouch height in meters", 1.15f, "In-game character will be crouching if your head is lower than this height above the playspace (clamped to 1-1.45m)");
            configAlternateEyeRendering = Config.Bind("Experimental performance tweaks", "Alternate light and shadow rendering per frame per eye", false, "If true will alternate between eyes when drawing lights and shadows each frame, \n might look really janky so only use this if you absolutely want to play this in VR but don't have the rig for it!");

            configUseLaserPointerOnWeapons = Config.Bind("Misc", "Use laser pointer on weapons?", true, "If true, all weapons will have a laser pointer.");

            configLaserPointerColorHex = Config.Bind("Misc", "Hex color to use for laster pointer", "#eb8078", "Google hexcolor and paste whatever color you want here.");
            configUseHapticsForShooting = Config.Bind("Misc", "Use haptics for shooting?", true, "If true, haptics effect will trigger when shooting weapons.");
            configShootingHapticsStrength = Config.Bind("Misc", "Shooting haptic strength", .75f, "The strength of haptic feedback while shooting. (0.0 to 1.0)");
            configWeaponRotationOffset = Config.Bind("Misc", "Weapon rotation offset", 12f, "Change this to rotate all weapons forward by the given amount of degrees (-45,45) --- \n'12' seems to work really well for the Quest and Index with the 'tip' action pose");

            Core.Log.Debug("VR enabled?" + configEnableVR.Value);
            Core.Log.Debug("Toggle VR by SteamVR running?" + configToggleVRBySteamVR.Value);
            Core.Log.Debug("Use VR Controllers? : " + configUseControllers.Value);
            Core.Log.Debug("Crouch on IRL crouch? : " + configIRLCrouch.Value);
            Core.Log.Debug("Use left hand as main hand? : " + configUseLeftHand.Value);
            Core.Log.Debug("Light resolution mode: " + configLightResMode.Value.ToString());
            Core.Log.Debug("Use two handed aiming: " + configUseTwoHanded.Value);
            Core.Log.Debug("Start with double handed aiming: " + configAlwaysDoubleHanded.Value);
            Core.Log.Debug("Snapturn amount: " + configSnapTurnAmount.Value);
            Core.Log.Debug("Use smooth turn?: " + configSmoothSnapTurn.Value);
            Core.Log.Debug("Recenter playspace on smooth turn?: " + configRecenterPlayspaceDuringSmoothTurn.Value);
            Core.Log.Debug("Watch size multiplier: " + configWatchScaling.Value);
            Core.Log.Debug("Use numbers for number display?: " + configUseNumbersForAmmoDisplay.Value);
            Core.Log.Debug("Watch color - " + configWatchColorHex.Value);
            Core.Log.Debug("Laserpointer color - " + configLaserPointerColorHex.Value);
            Core.Log.Debug("Crouching height - " + configCrouchHeight.Value);
            Core.Log.Debug("Alternate eye rendering? - " + configAlternateEyeRendering.Value);
            Core.Log.Debug("Laserpointer on? " + configUseLaserPointerOnWeapons.Value);
            Core.Log.Debug("Haptics when shooting? " + configUseHapticsForShooting.Value);
            Core.Log.Debug("Haptics strength - " + configShootingHapticsStrength.Value);
            Core.Log.Debug("Weapon rotation offset - " + configWeaponRotationOffset.Value);

            VRSettings.useVRControllers = configUseControllers.Value;
            VRSettings.crouchOnIRLCrouch = configIRLCrouch.Value;
            VRSettings.lightRenderMode = configLightResMode.Value;
            VRSettings.twoHandedAimingEnabled = configUseTwoHanded.Value;
            VRSettings.alwaysDoubleHanded = configAlwaysDoubleHanded.Value;
            VRSettings.snapTurnAmount = configSnapTurnAmount.Value;
            VRSettings.useSmoothTurn = configSmoothSnapTurn.Value;
            VRSettings.recenterOnSmoothTurn = configRecenterPlayspaceDuringSmoothTurn.Value;
            VRSettings.watchScale = Mathf.Clamp(configWatchScaling.Value, 0.5f, 2f);
            VRSettings.toggleVRBySteamVRRunning = configToggleVRBySteamVR.Value;
            VRSettings.useNumbersForAmmoDisplay = configUseNumbersForAmmoDisplay.Value;
            VRSettings.IRLCrouchBorder = Mathf.Clamp(configCrouchHeight.Value, 1f, 1.45f);
            VRSettings.alternateLightRenderingPerEye = configAlternateEyeRendering.Value;
            VRSettings.useLaserPointer = configUseLaserPointerOnWeapons.Value;
            VRSettings.useHapticForShooting = configUseHapticsForShooting.Value;
            VRSettings.shootingHapticsStrength = configShootingHapticsStrength.Value;
            VRSettings.globalWeaponRotationOffset = Mathf.Clamp(configWeaponRotationOffset.Value, -45, 45);

            Util.ExtensionMethods.Hex(configWatchColorHex.Value, ref VRSettings.watchColor);
            Util.ExtensionMethods.Hex(configLaserPointerColorHex.Value, ref VRSettings.laserPointerColor);

            if (configUseLeftHand.Value)
            {
                VRSettings.mainHand = HandType.Left;
            }
        }
    }
}