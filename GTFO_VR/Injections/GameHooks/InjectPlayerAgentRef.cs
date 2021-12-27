using GTFO_VR.Core;
using HarmonyLib;
using Player;
using System;

namespace GTFO_VR.Injections.GameHooks
{
    /// <summary>
    /// Get local player reference
    /// </summary>

    [HarmonyPatch(typeof(FPSCamera), nameof(FPSCamera.Setup))]
    [HarmonyPatch(new Type[] { typeof(LocalPlayerAgent) })]
    internal class InjectGetLocalPlayerAgentRef
    {
        private static void Postfix(FPSCamera __instance, LocalPlayerAgent owner)
        {
            VRSystems.OnPlayerSpawned(__instance, owner);
        }
    }
}