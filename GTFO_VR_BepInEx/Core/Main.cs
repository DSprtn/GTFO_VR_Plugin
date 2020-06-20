using System;
using UnityEngine;
using BepInEx;
using HarmonyLib;
using BepInEx.Configuration;
using GTFO_VR.Core;
using System.Diagnostics;
using Debug = UnityEngine.Debug;
using System.Collections.Generic;

namespace GTFO_VR_BepInEx.Core
{

    /// <summary>
    /// Entry point for patching existing methods in GTFO libraries
    /// </summary>

    [BepInPlugin("com.github.dsprtn.gtfovr", "GTFO Virtual Reality Plug-in", "0.6.0.0")]
    public class Main : BaseUnityPlugin
    {

        private ConfigEntry<bool> configEnableVR;
        private ConfigEntry<bool> configToggleVRBySteamVR;
        private ConfigEntry<bool> configUseControllers;
        private ConfigEntry<bool> configIRLCrouch;
        private ConfigEntry<bool> configUseLeftHand;
        private ConfigEntry<int>  configLightResMode;
        private ConfigEntry<bool> configUseTwoHanded;
        private ConfigEntry<bool> configDisableCompass;
        private ConfigEntry<bool> configAlwaysDoubleHanded;
        private ConfigEntry<float> configSnapTurnAmount;
        private ConfigEntry<bool> configSmoothSnapTurn;
        private ConfigEntry<float> configWatchScaling;


        void Awake()
        {
            Debug.Log("Loading VR plugin...");
            SetupConfig();
            Harmony harmony = new Harmony("com.github.dsprtn.gtfovr");

            if(VR_Settings.enabled && SteamVRRunningCheck())
            {
                harmony.PatchAll();
            } else
            {
                Debug.Log("VR launch aborted, VR is disabled or SteamVR is off!");
            }
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

            Debug.Log("VR processes found - " + possibleVRProcesses.Count);
            foreach(Process p in possibleVRProcesses)
            {
                Debug.Log(p);
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
            configLightResMode = Config.Bind("Experimental performance tweaks", "Light render resolution tweak - the lower resolution the greater the performance gain!", 1, "0 = 1920x1080, 1 = 1024x768 (Seems to be no difference, big performance increase), 2=640x480 (some small artifacting on lights, great performance increase)");
            configUseTwoHanded = Config.Bind("Input", "Use both hands to aim?", true, "If true, two-handed weapons will be allowed to be aimed with both hands.");
            configDisableCompass = Config.Bind("UI", "Disable compass in-game?", true, "If true, compass will not be shown in-game");
            configAlwaysDoubleHanded = Config.Bind("Input", "Always use double handed aiming? (Where it applies)", false, "If true, double handed weapons will always use double handed aiming (RECOMMENDED FOR GUN STOCK USERS)");
            configSnapTurnAmount = Config.Bind("Input", "Snap turn angle", 60f, "The amount of degrees to turn on a snap turn (or turn per half a second if smooth turn is enabled)");
            configSmoothSnapTurn = Config.Bind("Input", "Use smooth turning?", false, "If true, will use smooth turn instead of snap turn");
            configWatchScaling = Config.Bind("Misc", "Watch scale multiplier", 1.00f, "Size of the watch in-game will be multiplied by this value down to half of its default size or up to double (0.5 or 2.0)");

            Debug.Log("VR enabled?" + configEnableVR.Value);
            Debug.Log("Toggle VR by SteamVR running?" + configToggleVRBySteamVR.Value);
            Debug.Log("Use VR Controllers? : " + configUseControllers.Value);
            Debug.Log("Crouch on IRL crouch? : " + configIRLCrouch.Value);
            Debug.Log("Use left hand as main hand? : " + configUseLeftHand.Value);
            Debug.Log("Light resolution mode: " + configLightResMode.Value.ToString());
            Debug.Log("Use two handed aiming: " + configUseTwoHanded.Value);
            Debug.Log("Disable compass: " + configDisableCompass.Value);
            Debug.Log("Start with double handed aiming: " + configAlwaysDoubleHanded.Value);
            Debug.Log("Snapturn amount: " + configSnapTurnAmount.Value);
            Debug.Log("Use smooth turn?: " + configSmoothSnapTurn.Value);
            Debug.Log("Watch size multiplier: " + configWatchScaling.Value);

            VR_Settings.UseVRControllers = configUseControllers.Value;
            VR_Settings.crouchOnIRLCrouch = configIRLCrouch.Value;
            VR_Settings.lightRenderMode = configLightResMode.Value;
            VR_Settings.twoHandedAimingEnabled = configUseTwoHanded.Value;
            VR_Settings.disableCompass = configDisableCompass.Value;
            VR_Settings.alwaysDoubleHanded = configAlwaysDoubleHanded.Value;
            VR_Settings.snapTurnAmount = configSnapTurnAmount.Value;
            VR_Settings.useSmoothTurn = configSmoothSnapTurn.Value;
            VR_Settings.watchScale = Mathf.Clamp(configWatchScaling.Value, 0.5f, 2f);
            VR_Settings.toggleVRBySteamVRRunning = configToggleVRBySteamVR.Value;


            if (configUseLeftHand.Value)
            {
                VR_Settings.mainHand = GTFO_VR.HandType.Left;
            }
        }
    }
}
