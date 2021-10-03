using Player;
using System;

namespace GTFO_VR.Events
{
    /// <summary>
    /// Add event calls for locomotion events
    /// This currently only needs to describe the player entering the ladder but might be expanded later depending on the game's needs.
    /// </summary>
    public static class PlayerLocomotionEvents
    {
        public static event Action<LG_Ladder> OnPlayerEnterLadder;
        public static event Action<PlayerLocomotion.PLOC_State> OnStateChange;


        public static PlayerLocomotion.PLOC_State Current;
 
        public static void StateChanged(PlayerLocomotion.PLOC_State newState)
        {
            Current = newState;
            OnStateChange?.Invoke(newState);
        }

        public static void LadderEntered(PlayerAgent owner)
        {
            if (OnPlayerEnterLadder != null && owner.IsLocallyOwned)
            {
                OnPlayerEnterLadder.Invoke(owner.Locomotion.CurrentLadder);
            }
        }

        internal static bool InControllablePLOCState()
        {
            return Current != PlayerLocomotion.PLOC_State.ClimbLadder && Current != PlayerLocomotion.PLOC_State.Downed && Current != PlayerLocomotion.PLOC_State.InElevator;
        }
    }
}