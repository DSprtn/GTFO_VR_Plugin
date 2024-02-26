using BepInEx.Configuration;
using CellMenu;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using System;
using System.Collections.Generic;

namespace GTFO_VR.Core
{
    internal static class VRConfig
    {
        internal static ConfigEntry<bool> configUseControllers;
        internal static ConfigEntry<bool> configIRLCrouch;
        internal static ConfigEntry<bool> configUseLeftHand;
        internal static ConfigEntry<bool> configUseTwoHanded;
        internal static ConfigEntry<bool> configAlwaysDoubleHanded;
        internal static ConfigEntry<bool> configEarlyTransparentRendererFix;

        internal static ConfigEntry<float> configWatchScaling;
        internal static ConfigEntry<bool> configUseNumbersForAmmoDisplay;
        internal static ConfigEntry<string> configWatchColor;
        internal static ConfigEntry<bool> configDisplayChatOnWatch;
        internal static ConfigEntry<int> configCrouchHeight;
        internal static ConfigEntry<bool> configUseLaserPointerOnWeapons;
        internal static ConfigEntry<bool> configUseWeaponHaptics;
        internal static ConfigEntry<bool> configUseBhaptics;
        internal static ConfigEntry<bool> configUseShockwave;
        internal static ConfigEntry<string> configLaserPointerColor;
        internal static ConfigEntry<float> configShootingHapticsStrength;
        internal static ConfigEntry<int> configWeaponRotationOffset;

        internal static ConfigEntry<bool> configPostVignette;
        internal static ConfigEntry<bool> configOculusCrashWorkaround;
        internal static ConfigEntry<float> configRenderResolutionMultiplier;
        internal static ConfigEntry<bool> configHiddenAreaMask;
        internal static ConfigEntry<int> configFloorOffset;
        internal static ConfigEntry<bool> configUseOldHammer;
        internal static ConfigEntry<bool> configCameraBlood;
        internal static ConfigEntry<bool> configUseVisualHammerIndicator;
        internal static ConfigEntry<bool> configUseVignetteWhenMoving;
        internal static ConfigEntry<float> configMovementVignetteIntensity;
        internal static ConfigEntry<int> configSnapTurnAmount;
        internal static ConfigEntry<int> configSmoothTurnSpeed;
        internal static ConfigEntry<bool> configSmoothSnapTurn;

        internal static ConfigEntry<bool> configPosePredictionTranslation;
        internal static ConfigEntry<bool> configPosePredictionRotation;

        internal static ConfigEntry<bool> configUseQuickSwitch;
        internal static ConfigEntry<bool> configWatchInfoText;
        internal static ConfigEntry<bool> configWeaponInfoText;
        internal static ConfigEntry<bool> configCheckSteamVR;

        internal static ConfigEntry<bool> configTerminalKeyboard;

        internal static ConfigEntry<bool> configWeaponAmmoHoloText;

        internal static ConfigEntry<bool> configProtube;

        private static List<BepinGTFOSettingBase> VRSettings = new List<BepinGTFOSettingBase>();

        private static uint baseVRConfigID = 110000;

        private static Dictionary<eCellSettingID, ConfigEntryBase> configBindings = new Dictionary<eCellSettingID, ConfigEntryBase>();

        internal static ConfigEntry<bool> configDebugOrigin;
        internal static ConfigEntry<bool> configDebugShowTwoHHitboxes;
        internal static ConfigEntry<bool> configDebugShowHammerHitbox;

        public static Dictionary<uint, string> VRTextMappings = new Dictionary<uint, string>();


        internal static void SetupConfig(ConfigFile file)
        {
#if DEBUG_GTFO_VR
            BindHeader("DEBUG");
            configDebugOrigin = BindBool(file, "Debug - If you see this, I screwed up!", "Show Origin debug", false, "Shows origin/roomscale debug visuals", "Show Origin debug");
            configDebugShowTwoHHitboxes = BindBool(file, "Debug - If you see this, I screwed up!", "Show 2H hitboxes", false, "Shows two handed weapon hitboxes", "Show 2H hitboxes");
            configDebugShowHammerHitbox = BindBool(file, "Debug - If you see this, I screwed up!", "Show hammer hitbox", false, "Shows hammer hitbox", "Hammer hitbox debug");
#endif

            BindHeader("Input");
            configUseLeftHand = BindBool(file, "Input", "Use left hand as main hand?", false, "If true, all items will appear in the left hand", "Left handed mode");
            configProtube = BindBool(file, "Input", "Enable ProTubeVR support?", true, "If true, will enable ProTubeVR events", "ProtubeVR Support");
            configUseBhaptics = BindBool(file, "Bhaptics", "Enable bhaptics", true, "If true, bhaptics integration will be enabled", "Bhaptics Support");
            configUseShockwave = BindBool(file, "Shockwave", "Enable Shockwave (beta)", false, "If true, Shockwave suit integration will be enabled", "Shockwave Support");

            configPosePredictionTranslation = BindBool(file, "Input", "Pose Prediction Translation", true, "If true, motion prediction will be used to make controller position more responsive", "Pose Prediction Translation");
            configPosePredictionRotation = BindBool(file, "Input", "Pose Prediction Rotation", false, "If true, motion prediction will be used to make controller rotation more responsive", "Pose Prediction Rotation");

            configIRLCrouch = BindBool(file, "Input", "Crouch in-game when you crouch IRL?", true, "If true, when crouching down below a certain threshold IRL, the in-game character will also crouch", "Crouch on IRL crouch");
            configCrouchHeight = BindInt(file, "Input", "Crouch height in centimeters", 115, 90, 145, "In-game character will be crouching if your head is lower than this height above the playspace", "Crouch height (cm)");
            configSmoothSnapTurn = BindBool(file, "Input", "Use smooth turning?", false, "If true, will use smooth turn instead of snap turn", "Smooth turn");
            configSnapTurnAmount = BindInt(file, "Input", "Snap turn angle", 60, 0, 180, "The amount of degrees to turn on a snap turn", "Snap turn angle");            
            configSmoothTurnSpeed = BindInt(file, "Input", "Smooth turn speed", 90, 0, 180, "The amount of degrees to turn per second at full speed", "Smooth turn speed (degrees/s)");
            configFloorOffset = BindInt(file, "Misc", "Floor height offset (cm)", 0, 0, 50, "Floor offset in cm", "Floor offset (cm)");
            configUseVignetteWhenMoving = BindBool(file, "Misc", "Use Vignette when moving?", false, "If true, will display vignette effect while moving.", "Vignette while moving");
            configMovementVignetteIntensity = BindFloat(file, "Input", "Movement vignette intensity", 1f, .5f, 1.5f, "Multiplier for vignette intensity while moving", "Movement vignette intensity");

            BindHeader("Radial Menus");
            configUseQuickSwitch = BindBool(file, "Radial menu", "Use quick switch for radial menu?", true,
                "If true, opening and closing the radial menu fast will switch to last weapon", "Cycle to last weapon on fast press");
            configWatchInfoText = BindBool(file, "Radial menu", "Show watch info text in radial menu?", true,
    "If true, will show extra text info in watch radial menu", "Show watch menu text info");
            configWeaponInfoText = BindBool(file, "Radial menu", "Show weapon info text in radial menu?", true,
    "If true, will show extra text info in weapon radial menu", "Show weapon text info");

            BindHeader("Watch");
            configWatchColor = BindStringDropdown(file, "Watch", "Watch color", "WHITE", "Color to use for watch", "Watch color", new string[] { "WHITE", "RED", "GREEN", "BLUE", "CYAN", "YELLOW", "MAGENTA", "ORANGE", "BLACK" });
            configWatchScaling = BindFloat(file, "Watch", "Watch scale multiplier", 1f, 0.8f, 1.5f, "Watch size multiplier", "Watch size");
            configUseNumbersForAmmoDisplay = BindBool(file, "Watch", "Use numbers for ammo display?", false, "If true, current ammo and max ammo will be displayed as numbers on the watch", "Use number display for ammo");
            configDisplayChatOnWatch = BindBool(file, "Watch", "Display chat on watch?", false, "If true, the watch will also display the chat when toggled, in addition to player status and objectives. This does not affect the radial menu", "Display chat on watch?");

            BindHeader("Weapons");
            configUseWeaponHaptics = BindBool(file, "Haptics", "Use haptics for shooting?", true, "If true, haptics effect will trigger when shooting weapons.", "Weapon haptics");
            configShootingHapticsStrength = BindFloat(file, "Haptics", "Shooting haptic strength", .75f, 0f, 1f, "The strength of haptic feedback while shooting. (0.0 to 1.0)", "Haptic strength");

            configUseTwoHanded = BindBool(file, "Input", "Use both hands to aim?", true, "If true, two-handed weapons will be allowed to be aimed with both hands. Weapons will always fire as if you're using ADS'ing if this is off.", "Two handed aiming");
            configAlwaysDoubleHanded = BindBool(file, "Input", "Always use double handed aiming? (Where it applies)", false, "If true, double handed weapons will always use double handed aiming (RECOMMENDED FOR GUN STOCK USERS)", "Always use two handed");

            configUseLaserPointerOnWeapons = BindBool(file, "Laser pointer", "Use laser pointer on weapons?", true, "If true, all weapons will have a laser pointer.", "Laserpointer");
            configLaserPointerColor = BindStringDropdown(file, "Laser pointer", "Laser pointer color", "RED", "Color to use for the laser pointer", "Laserpointer color", 
                new string[] { "WHITE", "RED", "GREEN", "CYAN", "YELLOW", "MAGENTA", "ORANGE", "ALABASTER_RED"});

            configWeaponAmmoHoloText = BindBool(file, "Misc", "Use holo ammo text on weapons?", true, "If true, weapons will display clip ammo text.", "Holographic ammo display (WIP)");

            configWeaponRotationOffset = BindInt(file, "Misc", "Weapon forward rotation offset in degrees", 12, -45, 45, 
                "Change this to rotate all weapons forward by the given amount of degrees (-45,45) --- \n'12' seems to work really well for the Quest and Index with the 'tip' action pose", "Weapon Tilt (angles, forward)");


            BindHeader("Melee");
            configUseVisualHammerIndicator = BindBool(file, "Misc", "Show light for melee weapon charge?", true, "If true, will show a light indicator when hammer is half/fully charged", "Flash on 50%/100% charge");
            configUseOldHammer = BindBool(file, "Misc", "Use old melee weapon swing?", false, "If true, will use the in-game system for melee that uses animations and moves by itself.", "(OLD) Auto-swing melee");


            BindHeader("Rendering");

            configRenderResolutionMultiplier = BindFloat(file, "Rendering", "Render resolution multiplier", 1f, 0.2f, 2.5f, "Global rendering resolution multiplier", "Rendering resolution multiplier");

            configHiddenAreaMask = BindBool(file, "Rendering", "Use hidden area mask?", true,
                "If true, will not render parts of the frame that will not be visible in the HMD after the image has been distorted.",
                "Use hidden area mask");

            configPostVignette = BindBool(file, "Rendering - PostProcessing", "Use vignette effect? (Darkened edges of screen)", false, "If false, will disable the vignette effect in-game.", "Vignette");

            configCameraBlood = BindBool(file, "Rendering - Postprocessing", "Enable Camera liquid effects?", true, "If false, will disable camera liquid effects. Will give a little FPS boost.", "Camera liquid effects");

            
            configEarlyTransparentRendererFix = BindBool(file, "Rendering", "Use water performance fix?", true,
                "If true, water surfaces and water tanks will be modified to not degrade performance. Should be toggled before cage drop. Disable if transparent objects suddenly stop rendering.",
                "Water performance fix");

            configTerminalKeyboard = BindBool(file, "Input", "Use terminal keyboard?", true,
                "If true, a fancy in-game keyboard will be displayed when using the terminal, instead of the SteamVR overlay keyboard.",
                "Use terminal keyboard");

            BindHeader("Misc");
            configOculusCrashWorkaround = BindBool(file, "Misc", "Use Oculus crash workaround?", false, "If true, map and menu might look a little janky but it should crash less. Blame Zuck!", "Oculus crash workaround");
            configUseControllers = BindBool(file, "Input", "Use VR Controllers?", true, "If true, will use VR controllers. You can play with a gamepad and head aiming if you set this to false", "Motion Controllers (Restart if turning off!)");
            configCheckSteamVR = BindBool(file, "Misc", "Check if SteamVR running", true, "If true, will check if SteamVR is running before launching in VR.", "SteamVR running check");
        }

        internal static Il2CppSystem.Collections.Generic.List<iSettingsFieldData> InjectConfigIntoGame()
        {
            Il2CppSystem.Collections.Generic.List<iSettingsFieldData> GTFO_Setting_Data = new Il2CppSystem.Collections.Generic.List<iSettingsFieldData>();

            foreach (BepinGTFOSettingBase setting in VRSettings)
            {
                GTFO_Setting_Data.Add(setting.Register(configBindings));
            }
            return GTFO_Setting_Data;
        }

        public static string[] TryGetStringDropdownValues(eCellSettingID settingID)
        {
            foreach (BepinGTFOSettingBase setting in VRSettings)
            {
                if (setting.settingID == settingID && setting is BepinGTFOSettingString stringSetting)
                {
                    return stringSetting.possibleValues;
                }
            }
            return null;
        }

        public static void ConfigChanged<T>(eCellSettingID id, T value)
        {
            if (configBindings.TryGetValue(id, out ConfigEntryBase cfgBase))
            {
                ConfigEntry<T> cfgEntry = (ConfigEntry<T>)cfgBase;
                cfgEntry.Value = value;
            }
        }

        private static void BindHeader(string header)
        {
            baseVRConfigID++;
            VRSettings.Add(new BepinGTFOSettingHeader(header, eSettingInputType.Header, (eCellSettingID)baseVRConfigID));
        }

        private static ConfigEntry<T> BindConfig<T>(ConfigFile file, ConfigDefinition definition, ConfigDescription cd, T defaultValue)
        {
            baseVRConfigID++;
            var entry = file.Bind<T>(definition, defaultValue, cd);
            return entry;
        }

        private static ConfigEntry<float> BindFloat(ConfigFile file, string section, string key, float defaultVal, float minVal, float maxVal, string description, string title)
        {
            AcceptableValueBase a = new AcceptableValueRange<float>(minVal, maxVal);
            ConfigDefinition def = new ConfigDefinition(section, key);
            ConfigDescription cd = new ConfigDescription(description, a);
            var entry = BindConfig(file, def, cd, defaultVal);

            VRSettings.Add(new BepinGTFOSettingFloat(title, eSettingInputType.FloatSlider, (eCellSettingID)baseVRConfigID, entry, defaultVal, minVal, maxVal));
            return entry;
        }

        private static ConfigEntry<int> BindInt(ConfigFile file, string section, string key, int defaultVal, int minVal, int maxVal, string description, string title)
        {
            AcceptableValueBase a = new AcceptableValueRange<int>(minVal, maxVal);
            ConfigDefinition def = new ConfigDefinition(section, key);
            ConfigDescription cd = new ConfigDescription(description, a);
            var entry = BindConfig(file, def, cd, defaultVal);

            VRSettings.Add(new BepinGTFOSettingInt(title, eSettingInputType.IntMinMaxSlider, (eCellSettingID)baseVRConfigID, entry, defaultVal, minVal, maxVal));

            return entry;
        }

        private static ConfigEntry<string> BindStringDropdown(ConfigFile file, string section, string key, string defaultVal, string description, string title, string[] possibleValues)
        {
            AcceptableValueList<string> acceptableValues = new AcceptableValueList<string>(possibleValues);
            ConfigDefinition def = new ConfigDefinition(section, key);
            ConfigDescription cd = new ConfigDescription(description, acceptableValues);
            var entry = BindConfig(file, def, cd, defaultVal);

            VRSettings.Add(new BepinGTFOSettingString(title, eSettingInputType.StringArrayDropdown, (eCellSettingID)baseVRConfigID, entry, defaultVal, possibleValues));
            return entry;
        }

        private static ConfigEntry<bool> BindBool(ConfigFile file, string section, string key, bool defaultVal, string description, string title)
        {
            ConfigDefinition def = new ConfigDefinition(section, key);
            ConfigDescription cd = new ConfigDescription(description);
            var entry = BindConfig(file, def, cd, defaultVal);
            VRSettings.Add(new BepinGTFOSettingBool(title, eSettingInputType.BoolToggle, (eCellSettingID)baseVRConfigID, entry, defaultVal));
            return entry;
        }

        private abstract class BepinGTFOSettingBase
        {
            internal string title;
            internal eSettingInputType inputType;
            internal eCellSettingID settingID;

            internal virtual iSettingsFieldData Register(Dictionary<eCellSettingID, ConfigEntryBase> configs)
            {
                VRTextMappings.Add((uint)settingID, title);
                CellSettingGlobals.m_titles.Add(settingID, (uint)settingID);
                SettingsFieldData settingData = new SettingsFieldData
                {
                    Type = inputType,
                    Id = settingID
                };
                return settingData.Cast<iSettingsFieldData>();
            }

            protected BepinGTFOSettingBase(string title, eSettingInputType inputType, eCellSettingID settingID)
            {
                this.title = title;
                this.inputType = inputType;
                this.settingID = settingID;
            }
        }

        private class BepinGTFOSettingHeader : BepinGTFOSettingBase
        {
            public BepinGTFOSettingHeader(string title, eSettingInputType inputType, eCellSettingID settingID) : base(title, inputType, settingID)
            {
            }

            internal override iSettingsFieldData Register(Dictionary<eCellSettingID, ConfigEntryBase> configs)
            {
                return base.Register(configs);
            }
        }

        private class BepinGTFOSettingInt : BepinGTFOSettingBase
        {
            internal ConfigEntry<int> entry;
            internal int defaultValue;
            internal int minValue;
            internal int maxValue;

            public BepinGTFOSettingInt(string title, eSettingInputType inputType, eCellSettingID settingID, ConfigEntry<int> entry, int defaultValue, int minValue, int maxValue) : base(title, inputType, settingID)
            {
                this.entry = entry;
                this.defaultValue = defaultValue;
                this.minValue = minValue;
                this.maxValue = maxValue;
            }

            internal override iSettingsFieldData Register(Dictionary<eCellSettingID, ConfigEntryBase> configs)
            {
                configs.Add(settingID, entry);
                new CS_Value<int>(settingID, new Action<eCellSettingID, CS_Value<int>>(CellSettingsManager.RegIntVal), null, entry.Value);
                Il2CppStructArray<int> minMaxVals = new Il2CppStructArray<int>(2);

                minMaxVals[0] = minValue;
                minMaxVals[1] = maxValue;

                CellSettingGlobals.m_minMaxIntLimits.Add(settingID, minMaxVals);

                return base.Register(configs);
            }
        }

        private class BepinGTFOSettingFloat : BepinGTFOSettingBase
        {
            internal ConfigEntry<float> entry;
            internal float defaultValue;
            internal float minValue;
            internal float maxValue;

            public BepinGTFOSettingFloat(string title, eSettingInputType inputType, eCellSettingID settingID, ConfigEntry<float> entry, float defaultValue, float minValue, float maxValue) : base(title, inputType, settingID)
            {
                this.entry = entry;
                this.defaultValue = defaultValue;
                this.minValue = minValue;
                this.maxValue = maxValue;
            }

            internal override iSettingsFieldData Register(Dictionary<eCellSettingID, ConfigEntryBase> configs)
            {
                configs.Add(settingID, entry);
                new CS_Value<float>(settingID, new Action<eCellSettingID, CS_Value<float>>(CellSettingsManager.RegFloatVal), null, entry.Value);

                Il2CppStructArray<float> minMaxVals = new Il2CppStructArray<float>(2);

                minMaxVals[0] = minValue;
                minMaxVals[1] = maxValue;

                CellSettingGlobals.m_minMaxFloatLimits.Add(settingID, minMaxVals);
                return base.Register(configs);
            }
        }

        private class BepinGTFOSettingString : BepinGTFOSettingBase
        {
            internal ConfigEntry<string> entry;
            internal string defaultValue;
            internal string[] possibleValues;

            public BepinGTFOSettingString(string title, eSettingInputType inputType, eCellSettingID settingID, ConfigEntry<string> entry, string defaultValue, string[] possibleValues) : base(title, inputType, settingID)
            {
                this.entry = entry;
                this.defaultValue = defaultValue;
                this.possibleValues = possibleValues;
            }

            internal override iSettingsFieldData Register(Dictionary<eCellSettingID, ConfigEntryBase> configs)
            {
                configs.Add(settingID, entry);
                new CS_Value<string>(settingID, new Action<eCellSettingID, CS_Value<string>>(CellSettingsManager.RegStringVal), null, entry.Value);
                return base.Register(configs);
            }
        }

        private class BepinGTFOSettingBool : BepinGTFOSettingBase
        {
            internal ConfigEntry<bool> entry;
            internal bool defaultValue;

            public BepinGTFOSettingBool(string title, eSettingInputType inputType, eCellSettingID settingID, ConfigEntry<bool> entry, bool defaultValue) : base(title, inputType, settingID)
            {
                this.entry = entry;
                this.defaultValue = defaultValue;
            }

            internal override iSettingsFieldData Register(Dictionary<eCellSettingID, ConfigEntryBase> configs)
            {
                configs.Add(settingID, entry);
                new CS_Value<bool>(settingID, new Action<eCellSettingID, CS_Value<bool>>(CellSettingsManager.RegBoolVal), null, entry.Value);
                return base.Register(configs);
            }
        }
    }
}