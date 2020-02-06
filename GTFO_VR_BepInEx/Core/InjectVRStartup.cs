using System;
using System.Collections.Generic;
using System.Text;
using GTFO_VR;
using HarmonyLib;
using Player;
using UnityEngine;


namespace GTFO_VR_BepInEx.Core
{
    /// <summary>
    /// Entry point for loading and initiating all things VR
    /// </summary>

    [HarmonyPatch(typeof(FPSCamera),"Update")]
    class InjectVRStartup
    {
        static void Prefix(PlayerAgent __instance)
        {
            if (Input.GetKeyDown(KeyCode.F1) && !VRInitiator.VR_ENABLED)
            {
                Debug.Log("Creating VR instance...");
                GameObject vr = new GameObject();
                vr.AddComponent<VRInitiator>();
            }
        }
    }
}
