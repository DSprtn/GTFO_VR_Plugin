using GTFO_VR.Core.PlayerBehaviours;
using GTFO_VR.Core.VR_Input;
using HarmonyLib;
using Player;
using UnityEngine;


namespace GTFO_VR_BepInEx.Core
{
    /// <summary>
    /// Change look dir to controller aim (when weapon has flashlight) for flashlight aggro consistency --- for other players and if you're not the host
    /// </summary>
    [HarmonyPatch(typeof(PlayerSync), "SendLocomotion")]
    class InjectWeaponAimFlashlightAggroOnline
    {
        static void Prefix(ref Vector3 lookDir)
        {
            lookDir = HMD.GetVRInteractionLookDir();
        }
    }

    /// <summary>
    /// Change detection to use weapon flashlight direction (when you're the host or playing solo)
    /// </summary>
    [HarmonyPatch(typeof(PlayerAgent), "GetDetectionMod")]
    class InjectWeaponAimFlashlightAggro
    {
        static void Postfix(PlayerAgent __instance, Vector3 dir, float distance, ref float __result)
        {
            __result = PlayerVR.VRDetectionMod(dir, distance, __instance.Inventory.m_flashlight.range, __instance.Inventory.m_flashlight.spotAngle);
        }
    }
}
