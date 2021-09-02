using System;
using Player;

namespace GTFO_VR.Events
{
    public static class ItemInteractEvents
    {
        public static event Action<PlayerAgent> OnItemInteracted;

        public static void ItemInteracted(PlayerAgent source = null)
        {
            OnItemInteracted?.Invoke(source);
        }
    }
}