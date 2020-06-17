using System;
using UnityEngine;
using BepInEx;
using HarmonyLib;
using BepInEx.Configuration;
using GTFO_VR.Core;

namespace GTFO_VR_BepInEx.Core
{

    /// <summary>
    /// Entry point for patching existing methods in GTFO libraries
    /// </summary>

    [BepInPlugin("com.github.dsprtn.gtfovr", "GTFO Virtual Reality Plug-in", "0.5.0.0")]
    public class Main : BaseUnityPlugin
    {

        private ConfigEntry<bool> configUseControllers;
        private ConfigEntry<bool> configIRLCrouch;
        private ConfigEntry<bool> configUseLeftHand;
        private ConfigEntry<int>  configLightResMode;
        private ConfigEntry<bool> configUseTwoHanded;
        private ConfigEntry<bool> configDisableCompass;
        private ConfigEntry<bool> configAlwaysDoubleHanded;
        private ConfigEntry<float> configSnapTurnAmount;
        private ConfigEntry<bool> configSmoothSnapTurn;


        void Awake()
        {
            Debug.Log("Loading VR plugin...");
            SetupConfig();
            Harmony harmony = new Harmony("com.github.dsprtn.gtfovr");
            harmony.PatchAll();
        }

        private void SetupConfig()
        {
            configUseControllers = Config.Bind("Input", "Use VR Controllers?", true, "If true will use VR controllers. If you're using a Oculus headset you can play with a gamepad by setting this to 'false'");
            configIRLCrouch = Config.Bind("Input", "Crouch in-game when you crouch IRL?", true, "If true, when crouching down below a certain threshold IRL, the in-game character will also crouch");
            configUseLeftHand = Config.Bind("Input", "Use left hand as main hand?", false, "If true all items will appear in the left hand");
            configLightResMode = Config.Bind("Experimental performance tweaks", "Light render resolution tweak - the lower resolution the greater the performance gain!", 1, "0 = 1920x1080, 1 = 1024x768 (Seems to be no difference, big performance increase), 2=640x480 (some small artifacting on lights, great performance increase)");
            configUseTwoHanded = Config.Bind("Input", "Use both hands to aim?", true, "If true, two-handed weapons will be allowed to be aimed with both hands.");
            configDisableCompass = Config.Bind("UI", "Disable compass in-game?", true, "If true, compass will not be shown in-game");
            configAlwaysDoubleHanded = Config.Bind("Input", "Always use double handed aiming? (Where it applies)", false, "If true, double handed weapons will always use double handed aiming (RECOMMENDED FOR GUN STOCK USERS)");
            configSnapTurnAmount = Config.Bind("Input", "Snap turn angle", 60f, "The amount of degrees to turn on a snap turn (or turn per half a second if smooth turn is enabled)");
            configSmoothSnapTurn = Config.Bind("Input", "Use smooth turning?", false, "If true, turning will use smooth turn instead of snap turn");

            Debug.Log("Use VR Controllers? : " + configUseControllers.Value);
            Debug.Log("Crouch on IRL crouch? : " + configIRLCrouch.Value);
            Debug.Log("Use left hand as main hand? : " + configUseLeftHand.Value);
            Debug.Log("Light resolution mode: " + configLightResMode.Value.ToString());
            Debug.Log("Use two handed aiming: " + configUseTwoHanded.Value);
            Debug.Log("Disable compass: " + configDisableCompass.Value);
            Debug.Log("Start with double handed aiming: " + configAlwaysDoubleHanded.Value);
            Debug.Log("Snapturn amount: " + configSnapTurnAmount.Value);
            Debug.Log("Use smooth turn?: " + configSmoothSnapTurn.Value);

            VRSettings.UseVRControllers = configUseControllers.Value;
            VRSettings.crouchOnIRLCrouch = configIRLCrouch.Value;
            VRSettings.lightRenderMode = configLightResMode.Value;
            VRSettings.twoHandedAimingEnabled = configUseTwoHanded.Value;
            VRSettings.disableCompass = configDisableCompass.Value;
            VRSettings.alwaysDoubleHanded = configAlwaysDoubleHanded.Value;
            VRSettings.snapTurnAmount = configSnapTurnAmount.Value;
            VRSettings.useSmoothTurn = configSmoothSnapTurn.Value;

            if (configUseLeftHand.Value)
            {
                VRSettings.mainHand = GTFO_VR.HandType.Left;
            }
        }
    }
}
