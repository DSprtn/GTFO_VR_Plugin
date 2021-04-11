using GTFO_VR.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            if(OnPlayerWieldItem != null && item.Owner.IsLocallyOwned)
            {
                currentItem = item;
                GTFO_VR_Plugin.log.LogDebug("Item equip changed---");
                GTFO_VR_Plugin.log.LogDebug(item.ArchetypeName);
                GTFO_VR_Plugin.log.LogDebug(item.PublicName);
                OnPlayerWieldItem.Invoke(item);
                
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
    }
}
