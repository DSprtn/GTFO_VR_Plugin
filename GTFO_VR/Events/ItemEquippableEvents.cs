using GTFO_VR.Core;
using UnityEngine;

namespace GTFO_VR.Events
{
    /// <summary>
    /// Add event calls for the player changing weapons/equipment.
    /// </summary>
    public static class ItemEquippableEvents
    {
        public static ItemEquippable currentItem;

        public static event PlayerWieldItem OnPlayerWieldItem;

        public delegate void PlayerWieldItem(ItemEquippable item);

        public static void ItemEquipped(ItemEquippable item)
        {
            if (OnPlayerWieldItem != null && item.Owner.IsLocallyOwned)
            {
                currentItem = item;
                Log.Debug("Item equip changed---");
                Log.Debug(item.ArchetypeName);
                Log.Debug(item.PublicName);
                OnPlayerWieldItem.Invoke(item);

                if(currentItem.LeftHandGripTrans)
                {
                    Log.Debug($"Distance from left hand align to origin = {Vector3.Distance(currentItem.LeftHandGripTrans.position, currentItem.transform.position)}");
                }
            }
        }

        public static bool IsCurrentItemShootableWeapon()
        {
            return currentItem != null && currentItem.IsWeapon && currentItem.AmmoType != Player.AmmoType.None && currentItem.HasFlashlight;
        }

        public static bool CurrentItemHasFlashlight()
        {
            return currentItem != null && currentItem.HasFlashlight;
        }

        public static Vector3 GetCorrectedGripPosition()
        {
            if(!currentItem || !currentItem.LeftHandGripTrans || !currentItem.MuzzleAlign)
            {
                Log.Warning("Trying to get grip position for null item, null grip or null muzzle!");
                return Vector3.zero;
            }
            Vector3 offsetToGrip = currentItem.LeftHandGripTrans.position - currentItem.transform.position;
            Vector3 normalToMuzzle = (currentItem.MuzzleAlign.transform.position - currentItem.transform.position).normalized;
            return currentItem.transform.position + Vector3.Project(offsetToGrip, normalToMuzzle);
        }
    }
}