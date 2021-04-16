using GTFO_VR.Core;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Valve.VR;

namespace GTFO_VR.Injections.Rendering
{
    /// <summary>
    /// Resolution tweaks for UI etc.
    /// </summary>
    /// 
    [HarmonyPatch(typeof(GuiManager), nameof(GuiManager.OnResolutionChange))]
    internal class InjectForceVRResolution
    {
        private static void Prefix(ref Resolution res)
        {
            if (!VRSystems.Current)
            {
                return;
            }
            Log.Info("Updating global resolution to HMD res...");
            Resolution hmdResolution = SteamVR_Camera.GetSceneResolution();
            // We calcuate an optimal 16:9 resolution to use with the HMD resolution because that's the best aspect for the UI rendering
            Resolution closest16to9Res = hmdResolution;
            closest16to9Res.height = closest16to9Res.width / 16 * 9;

            res.width = closest16to9Res.width;
            res.height = closest16to9Res.height;
        }
    }

    [HarmonyPatch(typeof(GuiManager), nameof(GuiManager.UpdateResolution))]
    internal class InjectForceVRResolutionFixScreenRes
    {
        private static void Postfix(GuiManager __instance)
        {
            if (!VRSystems.Current)
            {
                return;
            }
            Log.Info("Updating menu page resolution to HMD res...");
            Resolution hmdResolution = SteamVR_Camera.GetSceneResolution();

            // We calcuate an optimal 16:9 resolution to use with the HMD resolution because that's the best aspect for the UI rendering
            Resolution closest16to9Res = hmdResolution;
            closest16to9Res.height = closest16to9Res.width / 16 * 9;

            GuiManager.ScreenRes = closest16to9Res;
            GuiManager.ScreenCenter = new Vector2(GuiManager.ScreenRes.width / 2, GuiManager.ScreenRes.height / 2);
            __instance.CalcSafeArea();
        }
    }
}
