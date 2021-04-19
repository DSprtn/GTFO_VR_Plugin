using GTFO_VR.Events;
using HarmonyLib;
using UnityEngine;

namespace GTFO_VR.Injections.Events
{
    /// <summary>
    /// Add event calls for the player reloading weapons
    /// </summary>

    [HarmonyPatch(typeof(PlayerInventoryLocal), nameof(PlayerInventoryLocal.DoReload))]
    internal class InjectPlayerReloadEvents
    {
        private static void Postfix(PlayerInventoryLocal __instance)
        {
            if(__instance.m_wieldedItem != null)
            {
                PlayerReloadEvents.WeaponReloaded();
            }
        }
    }
}