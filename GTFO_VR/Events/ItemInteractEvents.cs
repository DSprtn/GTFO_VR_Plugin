using System;

namespace GTFO_VR.Events
{
    public static class ItemInteractEvents
    {
        public static event Action OnItemInteracted;
        public static event Action<bool> OnFlashlightToggled;

        public static void ItemInteracted()
        {
            OnItemInteracted?.Invoke();
        }

        public static void FlashlightToggled(bool enabled)
        {
            OnFlashlightToggled?.Invoke(enabled);
        }
    }
}