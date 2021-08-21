using System;

namespace GTFO_VR.Events
{
    public static class ItemInteractEvents
    {
        public static event Action OnItemInteracted;

        public static void ItemInteracted()
        {
            OnItemInteracted?.Invoke();
        }
    }
}