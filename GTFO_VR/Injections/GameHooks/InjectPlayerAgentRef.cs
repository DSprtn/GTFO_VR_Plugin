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

    /// <summary>
    /// Some events like checkpoints are hard to properly handle in events. That's why we use this hack to reset the VR player if necessary.
    /// </summary>
    [HarmonyPatch(typeof(LocalPlayerAgent), nameof(LocalPlayerAgent.Update))]
    internal class InjectLocalPlayerAgentHeartbeat
    {
        private static void Postfix(LocalPlayerAgent __instance)
        {
            VRSystems.Heartbeat(__instance);
        }
    }
}