using Player;

namespace GTFO_VR.Events
{
    /// <summary>
    /// Add event calls for ammo updates
    /// This is mainly used to update the VR watch UI.
    /// </summary>
    public static class InventoryAmmoEvents
    {
        public static event InventoryAmmoUpdate OnInventoryAmmoUpdate;

        public delegate void InventoryAmmoUpdate(InventorySlotAmmo item, int clipLeft);

        public static void AmmoUpdate(InventorySlotAmmo item, int clipleft)
        {
            if (OnInventoryAmmoUpdate != null)
            {
                OnInventoryAmmoUpdate.Invoke(item, clipleft);
            }
        }
    }
}