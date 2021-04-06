using System;
using System.Collections.Generic;
using System.Text;
using CellMenu;
using Globals;
using GTFO_VR;
using GTFO_VR.UI;
using HarmonyLib;
using Player;
using UnityEngine;
using UnityEngine.Rendering;

namespace GTFO_VR_BepInEx.Core
{
    /// <summary>
    /// Get reference for the clustered rendering command buffer for use in PlayerVR
    /// </summary>

    [HarmonyPatch(typeof(FPSCamera),"Setup")]
    [HarmonyPatch(new Type[] {})]
    class InjectLightRenderCommandRef
    {
        static void Postfix(FPSCamera __instance)
        {
            PlayerVR.preRenderLights = __instance.m_preRenderCmds;
            PlayerVR.beforeForwardCmd = __instance.m_beforeForwardAlpahCmds;
        }
    }
}
