using System;
using GTFO_VR.Core;
using HarmonyLib;
using UnityEngine;
using Valve.VR;

namespace GTFO_VR.Injections
{
    /// <summary>
    /// Experimental performance tweak - changes light rendering resolution (with none to little visual difference, but a pretty good performance increase!)
    /// </summary>
    /// 


    [HarmonyPatch(typeof(ClusteredRendering), nameof(ClusteredRendering.OnResolutionChange))]
    [HarmonyPatch(new Type[] { typeof(Resolution) })]
    class InjectClusteredRenderingResolutionTweak
    {
        static void Prefix(ref Resolution res)
        {
            if (VR_Settings.lightRenderMode.Equals(0))
            {
                res = SteamVR_Camera.GetSceneResolution();
            }
            else
            if (VR_Settings.lightRenderMode.Equals(1))
            {
                res.width = 1920;
                res.height = 1080;
            }
            else if (VR_Settings.lightRenderMode.Equals(2))
            {
                res.width = 1024;
                res.height = 768;
            }
            else if (VR_Settings.lightRenderMode.Equals(3))
            {
                res.width = 640;
                res.height = 480;
            }
        }
    }
}
