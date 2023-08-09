using GTFO_VR.Events;
using HarmonyLib;
using UnityEngine;
using System;
using GTFO_VR.Core;

namespace GTFO_VR.Injections.Events
{
    /// <summary>
    /// Add event calls for player receiving events on the visor
    /// </summary>
    
    [HarmonyPatch(typeof(HUDGlassShatter), nameof(HUDGlassShatter.OnRenderImage))]
    internal class InjectHUDGlassShatterEvents
    {
        private static void Postfix()
        {
            Log.Info("HUDGlassShatter EVENT");
            //Log.Info(setting.ToString());
            //PlayerHudEvents.LiquidSplat();
        }
    }
}