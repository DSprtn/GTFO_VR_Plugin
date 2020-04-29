using GTFO_VR.Events;
using HarmonyLib;
using Player;

namespace GTFO_VR_BepInEx.Core
{
    /// <summary>
    /// Add event call onItemEquipped
    /// </summary>

    [HarmonyPatch(typeof(PlayerAmmoStorage), nameof(PlayerAmmoStorage.UpdateSlotAmmoUI), new[] { typeof(InventorySlotAmmo), typeof(int)})]
    class InjectInventoryAmmoUpdateEvent
    {
        static void Postfix(InventorySlotAmmo ammo, int clip, PlayerBackpack ___m_playerBackpack)
        {
            if(___m_playerBackpack.IsLocal)
            {
                InventoryAmmoEvents.AmmoUpdate(ammo, clip);
            }
        }
    }
}
