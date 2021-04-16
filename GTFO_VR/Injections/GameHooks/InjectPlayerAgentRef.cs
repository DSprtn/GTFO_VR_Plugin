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
    [HarmonyPatch(new Type[] { typeof(PlayerAgent) })]
    internal class InjectGetLocalPlayerAgentRef
    {
        private static void Postfix(FPSCamera __instance, PlayerAgent owner)
        {
            VRSystems.OnPlayerSpawned(__instance, owner);
        }
    }
}