using System;
using System.Collections.Generic;
using System.Text;
using CellMenu;
using Globals;
using GTFO_VR;
using GTFO_VR.Core;
using HarmonyLib;
using Player;
using UnityEngine;


namespace GTFO_VR_BepInEx.Core
{
    /// <summary>
    /// Experimental performance tweak - changes light rendering resolution (with none to little visual difference, but a pretty good performance increase!)
    /// </summary>

    [HarmonyPatch(typeof(ClusteredRendering))]
    [HarmonyPatch("OnResolutionChange")]
    [HarmonyPatch(new Type[] { typeof(Resolution) })]
    class InjectClusteredRenderingResolutionTweak
    {
        static void Prefix(ref Resolution res)
        {
            if (VRSettings.lightRenderMode.Equals(0))
            {
                res.width = 1920;
                res.height = 1080;
            } else 
            if (VRSettings.lightRenderMode.Equals(1))
            {
                res.width = 1024;
                res.height = 768;
            } 
            else if(VRSettings.lightRenderMode.Equals(2))
            {
                res.width = 640;
                res.height = 480;
            }
        }
    }
}
