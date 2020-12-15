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
        static void Postfix(CommandBuffer ___m_preRenderCmds, CommandBuffer ___m_beforeForwardAlpahCmds)
        {
            PlayerVR.preRenderLights = ___m_preRenderCmds;
            PlayerVR.beforeForwardCmd = ___m_beforeForwardAlpahCmds;
        }
    }
}
