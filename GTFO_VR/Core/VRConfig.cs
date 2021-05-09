using BepInEx.Configuration;
using CellMenu;
using System;
using System.Collections.Generic;

namespace GTFO_VR.Core
{
    internal static class VRConfig
    {
        internal static ConfigEntry<bool> configUseControllers;
        internal static ConfigEntry<bool> configIRLCrouch;
        internal static ConfigEntry<bool> configUseLeftHand;
        internal static ConfigEntry<string> configLightResMode;
        internal static ConfigEntry<bool> configAlternateEyeRendering;
        internal static ConfigEntry<bool> configUseTwoHanded;
        internal static ConfigEntry<bool> configAlwaysDoubleHanded;
        internal static ConfigEntry<int> configSnapTurnAmount;
        internal static ConfigEntry<bool> configSmoothSnapTurn;
        internal static ConfigEntry<float> configWatchScaling;
        internal static ConfigEntry<bool> configUseNumbersForAmmoDisplay;
        internal static ConfigEntry<string> configWatchColor;
        internal static ConfigEntry<int> configCrouchHeight;
        internal static ConfigEntry<bool> configUseLaserPointerOnWeapons;
        internal static ConfigEntry<bool> configUseWeaponHaptics;
        internal static ConfigEntry<string> configLaserPointerColor;
        internal static ConfigEntry<float> configShootingHapticsStrength;
        internal static ConfigEntry<int> configWeaponRotationOffset;

        internal static ConfigEntry<bool> configPostVignette;
        internal static ConfigEntry<bool> configPostBloom;
        internal static ConfigEntry<bool> configPostEyeAdaptation;
        internal static ConfigEntry<bool> configOculusCrashWorkaround;
        internal static ConfigEntry<float> configRenderResolutionMultiplier;
        internal static ConfigEntry<int> configFloorOffset;
        internal static ConfigEntry<bool> configUseOldHammer;
        internal static ConfigEntry<bool> configCameraBlood;

        private static List<BepinGTFOSettingBase> VRSettings = new List<BepinGTFOSettingBase>();

        private static int baseVRConfigID = 11000;

        private static Dictionary<eCellSettingID, ConfigEntryBase> configBindings = new Dictionary<eCellSettingID, ConfigEntryBase>();

        internal static void SetupConfig(ConfigFile file)
        {
            BindHeader("Input");
            configUseLeftHand = BindBool(file, "Input", "Use left hand as main hand?", false, "If true, all items will appear in the left hand", "Left handed mode");

            configIRLCrouch = BindBool(file, "Input", "Crouch in-game when you crouch IRL?", true, "If true, when crouching down below a certain threshold IRL, the in-game character will also crouch", "Crouch on IRL crouch");
            configCrouchHeight = BindInt(file, "Input", "Crouch height in centimeters", 115, 90, 145, "In-game character will be crouching if your head is lower than this height above the playspace", "Crouch height (cm)");

            configSmoothSnapTurn = BindBool(file, "Input", "Use smooth turning?", false, "If true, will use smooth turn instead of snap turn", "Smooth turning");
            configSnapTurnAmount = BindInt(file, "Input", "Snap turn angle", 60, 0, 180, "The amount of degrees to turn on a snap turn (or turn per half a second if smooth turn is enabled)", "Snap turn amount/speed (angle)");

            BindHeader("Watch");
            configWatchColor = BindStringDropdown(file, "Watch", "Hex color to use for watch", "0", "Configure this in-game...", "Watch color", new string[] { "WHITE", "RED", "GREEN", "BLUE", "CYAN", "YELLOW", "MAGENTA", "ORANGE", "BLACK" });
            configWatchScaling = BindFloat(file, "Watch", "Watch scale multiplier", 1f, 0.8f, 1.5f, "Watch size multiplier", "Watch size");
            configUseNumbersForAmmoDisplay = BindBool(file, "Watch", "Use numbers for ammo display?", false, "If true, current ammo and max ammo will be displayed as numbers on the watch", "Use number display for ammo");

            BindHeader("Weapons");
            configUseWeaponHaptics = BindBool(file, "Haptics", "Use haptics for shooting?", true, "If true, haptics effect will trigger when shooting weapons.", "Weapon haptics");
            configShootingHapticsStrength = BindFloat(file, "Haptics", "Shooting haptic strength", .75f, 0f, 1f, "The strength of haptic feedback while shooting. (0.0 to 1.0)", "Haptic strength");

            configUseTwoHanded = BindBool(file, "Input", "Use both hands to aim?", true, "If true, two-handed weapons will be allowed to be aimed with both hands. Weapons will always fire as if you're using ADS'ing if this is off.", "Two handed aiming");
            configAlwaysDoubleHanded = BindBool(file, "Input", "Always use double handed aiming? (Where it applies)", false, "If true, double handed weapons will always use double handed aiming (RECOMMENDED FOR GUN STOCK USERS)", "Always use two handed");

            configUseLaserPointerOnWeapons = BindBool(file, "Laser pointer", "Use laser pointer on weapons?", true, "If true, all weapons will have a laser pointer.", "Laserpointer");
            configLaserPointerColor = BindStringDropdown(file, "Laser pointer", "Hex color to use for laster pointer", "ORANGE", "Google hexcolor and paste whatever color you want here.", "Laserpointer color", new string[] { "WHITE", "RED", "GREEN", "CYAN", "YELLOW", "MAGENTA", "ORANGE" });

            configWeaponRotationOffset = BindInt(file, "Misc", "Weapon forward rotation offset in degrees", 12, -45, 45, "Change this to rotate all weapons forward by the given amount of degrees (-45,45) --- \n'12' seems to work really well for the Quest and Index with the 'tip' action pose",
"Weapon Tilt (angles, forward)");

            BindHeader("Rendering");

            configLightResMode = BindStringDropdown(file, "Rendering - Experimental", "Light/fog render resolution tweak - the lower the resolution the greater the performance gain!", "75%",
    "0 = Native HMD resolution, looks great but requires a beastly PC" +
    "\n1 = 75% resolution (Almost unnoticeable light shimmering on fog, good performance increase)" +
    "\n2 = 50% resolution (Medium shimmering, small light artifacts, big performance increase)" +
    "\n3 = 30% resolution (Small artifacting on lights/shadows, shimmering on fog, great performance increase)", "Light/Fog Resolution", new string[] { "NATIVE", "75%", "50%", "30%" });

            configRenderResolutionMultiplier = BindFloat(file, "Rendering", "Render resolution multiplier", 1f, 0.2f, 2.5f, "Global rendering resolution multiplier", "Rendering resolution multiplier");

            configPostVignette = BindBool(file, "Rendering - PostProcessing", "Use vignette effect? (Darkened edges of screen)", false, "If false, will disable the vignette effect in-game.", "Vignette");
            configPostBloom = BindBool(file, "Rendering - PostProcessing", "Use bloom effect? (Glow on bright lights)", false, "If false, will disable the bloom effect in-game. Gives a VERY nice performance boost if it's disabled! (1.5-2.0 ms!)", "Bloom");
            configPostEyeAdaptation = BindBool(file, "Rendering - PostProcessing", "Use eye adaptation? (Simulate the way a human eye adapts to light changes)", true,
                "If false, will disable eye adaptation. Gives a very slight performance boost if disabled (0.1-0.2ms.) Will make dark areas much darker though!", "Eye adaptation");

            configCameraBlood = BindBool(file, "Rendering - Postprocessing", "Enable Camera blood effect?", true, "If false, will disable camera blood effect. Will give a little FPS boost.", "Camera blood streaks");

            configAlternateEyeRendering = BindBool(file, "Rendering - Experimental", "Alternate light and shadow rendering per frame per eye", false,
    "If true will alternate between eyes when drawing lights and shadows each frame, \n might look really janky so only use this if you absolutely want to play this in VR but don't have the rig for it!",
    "Alternate rendering per eye");

            BindHeader("Misc");

            configFloorOffset = BindInt(file, "Misc", "Floor height offset (cm)", 0, 0, 50, "Floor offset in cm", "Floor offset (cm)");
            configUseOldHammer = BindBool(file, "Misc", "Use old hammer?", false, "If true, will use the old hammer that uses animations and moves by itself.", "Old VR hammer");
            configOculusCrashWorkaround = BindBool(file, "Misc", "Use Oculus crash workaround?", false, "If true, map and menu might look a little janky but it should crash less. Blame Zuck!", "Oculus crash workaround");

            configUseControllers = BindBool(file, "Input", "Use VR Controllers?", true, "If true, will use VR controllers. You can play with a gamepad and head aiming if you set this to false", "Motion Controllers (Restart if turning off!)");
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
                Log.Info($"Config changed! {cfgEntry.Definition.Key} - new value: {value}");
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
            Log.Info($"{entry.Definition.Key} - {entry.Value}");
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
                CellSettingGlobals.m_titles.Add(settingID, title);

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
                UnhollowerBaseLib.Il2CppStructArray<int> minMaxVals = new UnhollowerBaseLib.Il2CppStructArray<int>(2);

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

                UnhollowerBaseLib.Il2CppStructArray<float> minMaxVals = new UnhollowerBaseLib.Il2CppStructArray<float>(2);

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