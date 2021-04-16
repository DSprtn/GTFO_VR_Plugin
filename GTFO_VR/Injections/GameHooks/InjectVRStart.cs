using GTFO_VR.Core;
using HarmonyLib;
using UnityEngine;

namespace GTFO_VR.Injections.GameHooks
{
    /// <summary>
    /// Injection point for all things VR
    /// </summary>
    /// 
    [HarmonyPatch(typeof(GuiManager), nameof(GuiManager.Setup))]
    internal class InjectVRStart
    {
        private static void Postfix(GuiManager __instance)
        {
            __instance.m_resolutionCheckActive = false;
            if (!VRSystems.Current)
            {
                new GameObject("VR_Globals").AddComponent<VRSystems>();
            }
        }
    }
}
