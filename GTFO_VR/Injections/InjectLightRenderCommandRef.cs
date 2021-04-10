using System;
using GTFO_VR.Core.PlayerBehaviours;
using HarmonyLib;

namespace GTFO_VR.Injections
{
    /// <summary>
    /// Get reference for the clustered rendering command buffer for use in PlayerVR
    /// </summary>

    [HarmonyPatch(typeof(FPSCamera), nameof(FPSCamera.Setup))]
    [HarmonyPatch(new Type[] { })]
    class InjectLightRenderCommandRef
    {
        static void Postfix(FPSCamera __instance)
        {
            PlayerVR.preRenderLights = __instance.m_preRenderCmds;
            PlayerVR.beforeForwardCmd = __instance.m_beforeForwardAlpahCmds;
        }
    }
}
