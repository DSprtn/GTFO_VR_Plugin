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
    /// Replicate new objectives on the VR watch
    /// </summary>

    [HarmonyPatch(typeof(FPSCamera),"Setup")]
    [HarmonyPatch(new Type[] {})]
    class InjectLightRenderCommandRef
    {
        static void Postfix(CommandBuffer ___m_preRenderCmds)
        {
            PlayerVR.preRenderLights = ___m_preRenderCmds;
        }
    }
}
