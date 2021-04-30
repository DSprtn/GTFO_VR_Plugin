using BepInEx.Configuration;
using GTFO_VR.Core.VR_Input;
using SteamVR_Standalone_IL2CPP.Util;

namespace GTFO_VR.Core
{
    internal static class VRConfig
    {
        internal static ConfigEntry<bool> configEnableVR;
        internal static ConfigEntry<bool> configToggleVRBySteamVR;
        internal static ConfigEntry<bool> configUseControllers;
        internal static ConfigEntry<bool> configIRLCrouch;
        internal static ConfigEntry<bool> configUseLeftHand;
        internal static ConfigEntry<int> configLightResMode;
        internal static ConfigEntry<bool> configAlternateEyeRendering;
        internal static ConfigEntry<bool> configUseTwoHanded;
        internal static ConfigEntry<bool> configAlwaysDoubleHanded;
        internal static ConfigEntry<float> configSnapTurnAmount;
        internal static ConfigEntry<bool> configSmoothSnapTurn;
        internal static ConfigEntry<float> configWatchScaling;
        internal static ConfigEntry<bool> configUseNumbersForAmmoDisplay;
        internal static ConfigEntry<string> configWatchColorHex;
        internal static ConfigEntry<float> configCrouchHeight;
        internal static ConfigEntry<bool> configRecenterPlayspaceDuringSmoothTurn;
        internal static ConfigEntry<bool> configUseLaserPointerOnWeapons;
        internal static ConfigEntry<bool> configUseHapticsForShooting;
        internal static ConfigEntry<string> configLaserPointerColorHex;
        internal static ConfigEntry<float> configShootingHapticsStrength;
        internal static ConfigEntry<float> configWeaponRotationOffset;

        internal static ConfigEntry<bool> configPostVignette;
        internal static ConfigEntry<bool> configPostBloom;
        internal static ConfigEntry<bool> configPostEyeAdaptation;
        internal static ConfigEntry<bool> configOculusCrashWorkaround;

        internal static void SetupConfig(ConfigFile file)
        {
            
            configEnableVR = file.Bind("Startup", "Run VR plugin?", true, "If true, game will start in VR");
            configToggleVRBySteamVR = file.Bind("Startup", "Start in pancake if SteamVR is off?", true, "If true, will start the game in pancake mode if SteamVR is not detected");

            configUseControllers = file.Bind("Input", "Use VR Controllers?", true, "If true, will use VR controllers. You can play with a gamepad and head aiming if you set this to false");
            configIRLCrouch = file.Bind("Input", "Crouch in-game when you crouch IRL?", true, "If true, when crouching down below a certain threshold IRL, the in-game character will also crouch");
            configUseLeftHand = file.Bind("Input", "Use left hand as main hand?", false, "If true, all items will appear in the left hand");
            configCrouchHeight = file.Bind("Input", "Crouch height in meters", 1.15f, "In-game character will be crouching if your head is lower than this height above the playspace (clamped to 1-1.45m)");
            configUseTwoHanded = file.Bind("Input", "Use both hands to aim?", true, "If true, two-handed weapons will be allowed to be aimed with both hands.");
            configAlwaysDoubleHanded = file.Bind("Input", "Always use double handed aiming? (Where it applies)", false, "If true, double handed weapons will always use double handed aiming (RECOMMENDED FOR GUN STOCK USERS)");
            configSnapTurnAmount = file.Bind("Input", "Snap turn angle", 60f, "The amount of degrees to turn on a snap turn (or turn per half a second if smooth turn is enabled)");
            configSmoothSnapTurn = file.Bind("Input", "Use smooth turning?", false, "If true, will use smooth turn instead of snap turn");
            configRecenterPlayspaceDuringSmoothTurn = file.Bind("Input", "Recenter playspace on smoothturn?", true, "If true, will recenter the player collision and playspace during turning. \nMight result in a little bit of teleporting on the first frame you use smoothturn, but will keep collisions in sync better.");


            configPostVignette = file.Bind("Rendering - PostProcessing", "Use vignette effect? (Darkened edges of screen)", false, "If false, will disable the vignette effect in-game.");
            configPostBloom = file.Bind("Rendering - PostProcessing", "Use bloom effect? (Glow on bright lights)", false, "If false, will disable the bloom effect in-game. Gives a VERY nice performance boost if it's disabled! (1.5-2.0 ms!) ");
            configPostEyeAdaptation = file.Bind("Rendering - PostProcessing", "Use eye adaptation? (Simulate the way a human eye adapts to light changes)", true, "If false, will disable eye adaptation. Gives a very slight performance boost if disabled (0.1-0.2ms.) Will make dark areas much darker though!");

            configLightResMode = file.Bind("Rendering - Experimental", "Light/fog render resolution tweak - the lower the resolution the greater the performance gain!", 1,
                "0 = Native HMD resolution, looks great but requires a beastly PC" +
                "\n1 = 75% resolution (Almost unnoticeable light shimmering on fog, good performance increase)" +
                "\n2 = 60% resolution (Medium shimmering, small light artifacts, big performance increase)" +
                "\n3 = 30% resolution (Small artifacting on lights/shadows, shimmering on fog, great performance increase)");

            configAlternateEyeRendering = file.Bind("Rendering - Experimental", "Alternate light and shadow rendering per frame per eye", false, "If true will alternate between eyes when drawing lights and shadows each frame, \n might look really janky so only use this if you absolutely want to play this in VR but don't have the rig for it!");

            configWatchScaling = file.Bind("Watch", "Watch scale multiplier", 1.00f, "Size of the watch in-game will be multiplied by this value down to half of its default size or up to double (0.5 or 2.0)");
            configUseNumbersForAmmoDisplay = file.Bind("Watch", "Use numbers for ammo display?", false, "If true, current ammo and max ammo will be displayed as numbers on the watch");
            configWatchColorHex = file.Bind("Watch", "Hex color to use for watch", "#ffffff", "Google hexcolor and paste whatever color you want here");

            configUseLaserPointerOnWeapons = file.Bind("Laser pointer", "Use laser pointer on weapons?", true, "If true, all weapons will have a laser pointer.");
            configLaserPointerColorHex = file.Bind("Laser pointer", "Hex color to use for laster pointer", "#eb8078", "Google hexcolor and paste whatever color you want here.");
            configUseHapticsForShooting = file.Bind("Haptics", "Use haptics for shooting?", true, "If true, haptics effect will trigger when shooting weapons.");
            configShootingHapticsStrength = file.Bind("Haptics", "Shooting haptic strength", .75f, "The strength of haptic feedback while shooting. (0.0 to 1.0)");
            configWeaponRotationOffset = file.Bind("Misc", "Weapon forward rotation offset in degrees", 12f, "Change this to rotate all weapons forward by the given amount of degrees (-45,45) --- \n'12' seems to work really well for the Quest and Index with the 'tip' action pose");

            configOculusCrashWorkaround = file.Bind("Misc", "Use Oculus crash workaround?", false, "If true, map and menu might look a little janky but it should crash less. Blame Zuck!");

            Log.Debug("VR enabled?" + configEnableVR.Value);
            Log.Debug("Toggle VR by SteamVR running?" + configToggleVRBySteamVR.Value);
            Log.Debug("Use VR Controllers? : " + configUseControllers.Value);
            Log.Debug("Crouch on IRL crouch? : " + configIRLCrouch.Value);
            Log.Debug("Use left hand as main hand? : " + configUseLeftHand.Value);
            Log.Debug("Light resolution mode: " + configLightResMode.Value.ToString());
            Log.Debug("Use two handed aiming: " + configUseTwoHanded.Value);
            Log.Debug("Start with double handed aiming: " + configAlwaysDoubleHanded.Value);
            Log.Debug("Snapturn amount: " + configSnapTurnAmount.Value);
            Log.Debug("Use smooth turn?: " + configSmoothSnapTurn.Value);
            Log.Debug("Recenter playspace on smooth turn?: " + configRecenterPlayspaceDuringSmoothTurn.Value);
            Log.Debug("Watch size multiplier: " + configWatchScaling.Value);
            Log.Debug("Use numbers for number display?: " + configUseNumbersForAmmoDisplay.Value);
            Log.Debug("Watch color - " + configWatchColorHex.Value);
            Log.Debug("Laserpointer color - " + configLaserPointerColorHex.Value);
            Log.Debug("Crouching height - " + configCrouchHeight.Value);
            Log.Debug("Alternate eye rendering? - " + configAlternateEyeRendering.Value);
            Log.Debug("Laserpointer on? " + configUseLaserPointerOnWeapons.Value);
            Log.Debug("Haptics when shooting? " + configUseHapticsForShooting.Value);
            Log.Debug("Haptics strength - " + configShootingHapticsStrength.Value);
            Log.Debug("Weapon rotation offset - " + configWeaponRotationOffset.Value);

            Log.Debug("Oculus crash workaround? - " + configOculusCrashWorkaround.Value);


            Log.Debug("Use eye adaptation? " + configPostEyeAdaptation.Value);
            Log.Debug("Use vignette? " + configPostVignette.Value);
            Log.Debug("Use bloom? " + configPostBloom.Value);

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

            VRSettings.oculusCrashWorkaround = configOculusCrashWorkaround.Value;

            VRSettings.useBloomPostProcess = configPostBloom.Value;
            VRSettings.useEyeAdaptionPostProcess = configPostEyeAdaptation.Value;
            VRSettings.useVignettePostProcess = configPostVignette.Value;

            Util.ExtensionMethods.Hex(configWatchColorHex.Value, ref VRSettings.watchColor);
            Util.ExtensionMethods.Hex(configLaserPointerColorHex.Value, ref VRSettings.laserPointerColor);

            if (configUseLeftHand.Value)
            {
                VRSettings.mainHand = HandType.Left;
            }
        }
    }
}