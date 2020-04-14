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
    /// Remove unneccessary crosshairs from gameview
    /// </summary>

    [HarmonyPatch(typeof(CrosshairGuiLayer), "ShowPrecisionDot")]
    class InjectRemoveCrosshairDot
    {
        static void Postfix(CrosshairGuiLayer __instance)
        {
            __instance.HideAllCrosshairs();
        }
    }

    [HarmonyPatch(typeof(CrosshairGuiLayer), "ShowSpreadCircle")]
    class InjectRemoveCrosshairCircle
    {
        static void Postfix(CrosshairGuiLayer __instance)
        {
            __instance.HideAllCrosshairs();
        }
    }

    // TODO, not working correctly yet --- Game is still showing hit indicators in game view (not in HMD though, so this only affects video recordings)
    // Update - Might be fixed?
    [HarmonyPatch(typeof(CrosshairGuiLayer), "ShowHitIndicator")]
    class InjectRemoveCrosshairHit
    {
        static bool Prefix()
        {

            return false;
        }
    }
}