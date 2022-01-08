using GTFO_VR.Core;
using HarmonyLib;
using System;
using UnityEngine;
using Valve.VR;

namespace GTFO_VR.Injections.Rendering
{
    /// <summary>
    /// [Deprecated] - Gives more artifacts than performance now
    /// Experimental performance tweak - changes light rendering resolution (with none to little visual difference, but a pretty good performance increase!)
    /// </summary>
    ///

    [HarmonyPatch(typeof(ClusteredRendering), nameof(ClusteredRendering.OnResolutionChange))]
    [HarmonyPatch(new Type[] { typeof(Resolution) })]
    internal class InjectClusteredRenderingResolutionTweak
    {
        private static void Prefix(ref Resolution res)
        {
            GetClusteredResolutionTweak(ref res);
        }

        public static Resolution GetClusteredResolutionTweak(ref Resolution res)
        {
            Resolution HMDRes = SteamVR_Camera.GetSceneResolution();
            res.width = HMDRes.width;
            res.height = HMDRes.height;
            return res;
        }
    }
}