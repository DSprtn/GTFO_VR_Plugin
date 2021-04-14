using GTFO_VR.Core.PlayerBehaviours;
using HarmonyLib;
using Player;
using UnityEngine;


namespace GTFO_VR.Injections
{
    /// <summary>
    /// Change detection to use weapon flashlight direction (when you're the host or playing solo)
    /// </summary>
    [HarmonyPatch(typeof(PlayerAgent), nameof(PlayerAgent.GetDetectionMod))]
    class InjectWeaponAimFlashlightAggro
    {
        static void Postfix(PlayerAgent __instance, Vector3 dir, float distance, ref float __result)
        {
            __result = VRDetectionModHack.VRDetectionMod(dir, distance, __instance.Inventory.m_flashlight.range, __instance.Inventory.m_flashlight.spotAngle);
        }
    }
}
