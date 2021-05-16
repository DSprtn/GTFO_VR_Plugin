using Player;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GTFO_VR.Events
{
    public static class BackpackEvents
    {
        public static Action<InventorySlot, eInventoryItemStatus> OnNewItemStatus;
        public static void SlotStatusChanged(InventorySlot slot, eInventoryItemStatus status)
        {
            OnNewItemStatus?.Invoke(slot, status);
        }
    }
}
