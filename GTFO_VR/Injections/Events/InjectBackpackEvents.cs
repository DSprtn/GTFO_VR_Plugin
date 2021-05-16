using GTFO_VR.Events;
using HarmonyLib;
using Player;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GTFO_VR.Injections.Events
{
    [HarmonyPatch(typeof(PlayerBackpack), nameof(PlayerBackpack.UpdateItemStatus))]
    internal class InjectBackpackEvents
    {
        private static void Postfix(PlayerBackpack __instance, InventorySlot slot, eInventoryItemStatus status)
        {
            if(__instance.IsLocal)
            {
                BackpackEvents.SlotStatusChanged(slot, status);
            }
        }
    }
}
