using GTFO_VR.Core;
using HarmonyLib;
using System;
using UnityEngine;
using Valve.VR;

namespace GTFO_VR.Injections.Rendering
{
    /// <summary>
    /// [Deprecated] - Gives more artifacts than performance now, instead this corrects the HMD 
    /// Experimental performance tweak - changes light rendering resolution (with none to little visual difference, but a pretty good performance increase!)
    /// </summary>
    ///

    [HarmonyPatch(typeof(ClusteredRendering), nameof(ClusteredRendering.OnResolutionChange))]
    [HarmonyPatch(new Type[] { typeof(Resolution) })]
    internal class InjectClusteredRenderingResolutionTweak
    {
        private static void Prefix(ref Resolution res)
        {
            res = SteamVR_Camera.GetSceneResolution();
        }
    }

    [HarmonyPatch(typeof(PreLitVolume), nameof(PreLitVolume.OnResolutionChange))]
    internal class InjectPreLitVolumeResolutionTweak
    {
        private static void Prefix(ref int width, ref int height)
        {
            Resolution res = SteamVR_Camera.GetSceneResolution();

            width = res.width;
            height = res.height;
        }
    }
}