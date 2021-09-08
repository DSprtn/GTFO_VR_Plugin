using System;
using Player;
using ChainedPuzzles;
using Il2CppSystem.Collections.Generic;

namespace GTFO_VR.Events
{
    public static class PlayerInteractionEvents
    {
        public static event Action<PlayerAgent> OnPlayerInteracted;
        public static event Action<eBioscanStatus, float, List<PlayerAgent>> OnBioscanSetState;

        public static void PlayerInteracted(PlayerAgent source = null)
        {
            OnPlayerInteracted?.Invoke(source);
        }
        
        public static void SetBioscanState(eBioscanStatus status, float progress, List<PlayerAgent> playersInScan)
        {
            OnBioscanSetState?.Invoke(status, progress, playersInScan);
        }
    }
}