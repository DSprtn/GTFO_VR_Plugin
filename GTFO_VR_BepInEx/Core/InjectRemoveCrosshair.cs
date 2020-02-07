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
    /// Remove unneccessary crosshair from gameview
    /// </summary>

    [HarmonyPatch(typeof(CrosshairGuiLayer),"SetCrosshairAlpha")]
    class InjectRemoveCrosshair
    {
        static void Prefix(CrosshairGuiLayer __instance)
        {
            __instance.HideAllCrosshairs();
        }
    }
}
