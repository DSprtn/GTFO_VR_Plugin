using GTFO_VR.Core;
using HarmonyLib;
using System;
using UnityEngine;
using Valve.VR;

namespace GTFO_VR.Injections.Rendering
{
    /// <summary>
    /// Experimental performance tweak - changes light rendering resolution (with none to little visual difference, but a pretty good performance increase!)
    /// </summary>
    ///

    [HarmonyPatch(typeof(ClusteredRendering), nameof(ClusteredRendering.OnResolutionChange))]
    [HarmonyPatch(new Type[] { typeof(Resolution) })]
    internal class InjectClusteredRenderingResolutionTweak
    {
        private static void Prefix(ref Resolution res)
        {
            Resolution HMDRes = SteamVR_Camera.GetSceneResolution();
            if (VRConfig.configLightResMode.Value.Equals("75%"))
            {
                HMDRes.width = Mathf.FloorToInt(HMDRes.width * .75f);
                HMDRes.height = Mathf.FloorToInt(HMDRes.height * .75f);
            }
            else if (VRConfig.configLightResMode.Value.Equals("60%"))
            {
                HMDRes.width = Mathf.FloorToInt(HMDRes.width * .5f);
                HMDRes.height = Mathf.FloorToInt(HMDRes.height * .5f);
            }
            else if (VRConfig.configLightResMode.Value.Equals("30%"))
            {
                HMDRes.width =  Mathf.FloorToInt(HMDRes.width * .3f);
                HMDRes.height = Mathf.FloorToInt(HMDRes.height * .3f);
            }
            res.width = HMDRes.width;
            res.height = HMDRes.height;
        }
    }
}