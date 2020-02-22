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

    [BepInPlugin("com.github.dsprtn.gtfovr", "GTFO Virtual Reality Plug-in", "0.3.0.0")]
    public class Main : BaseUnityPlugin
    {


        private ConfigEntry<bool> configUseControllers;
        private ConfigEntry<bool> configIRLCrouch;
        private ConfigEntry<bool> configUseLeftHand;


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

            Debug.Log("Use VR Controllers? : " + configUseControllers.Value);
            Debug.Log("Crouch on IRL crouch? : " + configIRLCrouch.Value);
            Debug.Log("Use left hand as main hand? : " + configUseLeftHand.Value);

            VRSettings.UseVRControllers = configUseControllers.Value;
            VRSettings.crouchOnIRLCrouch = configIRLCrouch.Value;
            if(configUseLeftHand.Value)
            {
                VRSettings.mainHand = GTFO_VR.HandType.Left;
            }
        }
    }
}
