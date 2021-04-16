using GTFO_VR.Core;
using HarmonyLib;
using UnityEngine;

namespace GTFO_VR.Injections.GameHooks
{
    /// <summary>
    /// Injection point for all things VR
    /// </summary>
    /// 
    [HarmonyPatch(typeof(GuiManager), nameof(GuiManager.OnResolutionChange))]
    internal class InjectVRStart
    {
        private static void Postfix()
        {
            if (!VRSystems.Current)
            {
                new GameObject("VR_Globals").AddComponent<VRSystems>();
            }
        }
    }
}
