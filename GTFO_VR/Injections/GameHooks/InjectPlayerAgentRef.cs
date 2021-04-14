using GTFO_VR.Core;
using HarmonyLib;
using Player;
using System;

namespace GTFO_VR.Injections
{
    /// <summary>
    /// Get local player reference
    /// </summary>

    [HarmonyPatch(typeof(FPSCamera), nameof(FPSCamera.Setup))]
    [HarmonyPatch(new Type[] { typeof(PlayerAgent) })]
    class InjectGetLocalPlayerAgentRef
    {
        static void Postfix(FPSCamera __instance, PlayerAgent owner)
        {
            VRSystems.OnPlayerSpawned(__instance, owner);
        }
    }
}
