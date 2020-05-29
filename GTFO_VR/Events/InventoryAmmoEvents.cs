using Player;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace GTFO_VR.Events
{
    public static class InventoryAmmoEvents
    {

        public static event InventoryAmmoUpdate OnInventoryAmmoUpdate;
        public delegate void InventoryAmmoUpdate(InventorySlotAmmo item, int clipLeft);

        public static void AmmoUpdate(InventorySlotAmmo item, int clipleft)
        {
            if(OnInventoryAmmoUpdate != null)
            { 
                OnInventoryAmmoUpdate.Invoke(item, clipleft);
            }
        }

    }
}
