using GTFO_VR.Events;
using HarmonyLib;
using Player;

namespace GTFO_VR.Injections.Events
{
    /// <summary>
    /// Add event calls for changes in ammo
    /// </summary>

    [HarmonyPatch(typeof(PlayerAmmoStorage), nameof(PlayerAmmoStorage.UpdateSlotAmmoUI), new[] { typeof(InventorySlotAmmo), typeof(int) })]
    class InjectInventoryAmmoUpdateEvent
    {
        static void Postfix(PlayerAmmoStorage __instance, InventorySlotAmmo ammo, int clip)
        {
            if (__instance.m_playerBackpack.IsLocal)
            {
                InventoryAmmoEvents.AmmoUpdate(ammo, clip);
            }
        }
    }
}
