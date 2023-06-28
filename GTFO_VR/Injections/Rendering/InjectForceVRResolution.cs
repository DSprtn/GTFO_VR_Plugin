using GTFO_VR.Core;
using GTFO_VR.Core.UI;
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
            Log.Info("Updating global resolution to 16:9 HMD res...");
            // We use a scaled 16:9 resolution because that's the best aspect for rendering the UI of GTFO
            Resolution scaledHMDRes = SteamVR_Camera.GetResolutionForAspect(16, 9, VR_UI_Overlay.MAX_GUI_RESOLUTION);
            res.width = scaledHMDRes.width;
            res.height = scaledHMDRes.height;
        }
    }

    [HarmonyPatch(typeof(GuiManager), nameof(GuiManager.UpdateResolution))]
    internal class InjectForceVRResolutionFixScreenRes
    {
        private static void Postfix(GuiManager __instance)
        {
            Log.Info("Updating menu page resolution to HMD res...");
            Resolution scaledHMDResolution = SteamVR_Camera.GetResolutionForAspect(16, 9, VR_UI_Overlay.MAX_GUI_RESOLUTION);
            GuiManager.ScreenRes = scaledHMDResolution;
            GuiManager.ScreenCenter = new Vector2(GuiManager.ScreenRes.width / 2, GuiManager.ScreenRes.height / 2);
            __instance.CalcSafeArea();
        }
    }


    /// <summary>
    /// Patches the screen liquid system to use the proper VR aspect
    /// </summary>
    [HarmonyPatch(typeof(GlassLiquidSystem), nameof(GlassLiquidSystem.OnResolutionChange))]
    internal class InjectScreenLiquidResolutionTweak
    {
        private static void Prefix(ref Resolution res)
        {
           // We use the unscaled resolution for screen liquid effects to prevent the effects from being scaled or going off-screen
           Resolution scaledHMDRes = SteamVR_Camera.GetResolutionForAspect(16, 9);
           res.width = scaledHMDRes.width;
           res.height = scaledHMDRes.height;
        }
    }

}
