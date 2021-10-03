using GTFO_VR.Events;
using HarmonyLib;

namespace GTFO_VR.Injections.Events
{
    /// <summary>
    /// Add event calls for the player starting to reload weapons
    /// </summary>

    [HarmonyPatch(typeof(PlayerInventoryLocal), nameof(PlayerInventoryLocal.TriggerReload))]
    internal class InjectPlayerTriggerReloadEvents
    {
        private static void Postfix(PlayerInventoryLocal __instance)
        {
            if (__instance.m_wieldedItem != null)
            {
                PlayerTriggerReloadEvents.TriggerWeaponReloaded();
            }
        }
    }
}