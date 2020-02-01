using System;
using UnityEngine;
using BepInEx;
using HarmonyLib;

namespace GTFO_VR_BepInEx.Core
{

    /// <summary>
    /// Entry point for patching existing methods in GTFO libraries
    /// </summary>

    [BepInPlugin("com.github.dsprtn.gtfovr", "GTFO Virtual Reality Plug-in", "0.1.0.0")]
    public class Main : BaseUnityPlugin
    {
        void Awake()
        {
            Debug.Log("Loading VR plugin...");
            Harmony harmony = new Harmony("com.github.dsprtn.gtfovr");
            harmony.PatchAll();
        }
    }
}
