using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace GTFO_VR.Events
{
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
                Debug.Log("Item equip changed---");
                Debug.Log(item.ArchetypeName);
                Debug.Log(item.PublicName);
                OnPlayerWieldItem.Invoke(item);
                
            }
        }

        public static bool IsCurrentItemShootableWeapon()
        {
            return currentItem != null && currentItem.IsWeapon && currentItem.AmmoType != Player.AmmoType.None;
        }

        public static bool CurrentItemHasFlashlight()
        {
            return currentItem != null && currentItem.HasFlashlight;
        }
    }
}
