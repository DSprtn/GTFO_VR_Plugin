using System;
using Player;

namespace GTFO_VR.Events
{
    public static class PlayerInteractionEvents
    {
        public static event Action<PlayerAgent> OnPlayerInteracted;

        public static void PlayerInteracted(PlayerAgent source = null)
        {
            OnPlayerInteracted?.Invoke(source);
        }
    }
}