using System;
using Player;

namespace GTFO_VR.Events
{
    public static class ItemInteractEvents
    {
        public static event Action<PlayerAgent> OnItemInteracted;
        public static event Action<bool> OnFlashlightToggled;

        public static void ItemInteracted(PlayerAgent player)
        {
            OnItemInteracted?.Invoke(player);
        }

        public static void FlashlightToggled(bool enabled)
        {
            OnFlashlightToggled?.Invoke(enabled);
        }
    }
}