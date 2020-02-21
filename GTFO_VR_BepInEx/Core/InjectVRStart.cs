using System;
using System.Collections.Generic;
using System.Text;
using CellMenu;
using Globals;
using GTFO_VR;
using HarmonyLib;
using Player;
using UnityEngine;


namespace GTFO_VR_BepInEx.Core
{
    /// <summary>
    /// Entry point for loading and initiating all things VR
    /// </summary>

    [HarmonyPatch(typeof(UI_Pass),"Awake")]
    class InjectVRStart
    {
        static void Prefix(UI_Pass __instance)
        {
            if(!VRGlobal.VR_ENABLED)
            {
                new GameObject("VR_Globals").AddComponent<VRGlobal>();
                VR_UI_Overlay.UI_ref = __instance;
            }
        }
    }
}
