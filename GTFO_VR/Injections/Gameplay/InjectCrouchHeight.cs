using HarmonyLib;
using Player;
using UnityEngine;

namespace GTFO_VR.Injections.Gameplay
{
    /// <summary>
    /// Makes the first person items follow the position and aim direction of the main controller(s) of the player
    /// </summary>
    [HarmonyPatch(typeof(PlayerCharacterController), nameof(PlayerCharacterController.SetColliderCrouched))]
    internal class InjectCrouchPopFix
    {
        private static bool Prefix(PlayerCharacterController __instance, bool crouched)
        {
            if (__instance.m_characterController == null || __instance.m_owner == null || __instance.m_owner.IsBeingDespawned)
                return false;
            if (crouched)
            {
                __instance.m_characterController.height = __instance.m_collCrouchHeight;
                __instance.m_characterController.center = new Vector3(0.0f, __instance.m_collCrouchHeight * 0.5f, 0.0f);
            }
            else
            {
                __instance.m_characterController.height = __instance.m_collStandHeight;
                __instance.m_characterController.center = new Vector3(0.0f, __instance.m_collStandHeight * 0.5f, 0.0f);
            }

            return false;
        }
    }
}