using GTFO_VR.Core;
using Player;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace GTFO_VR.Events
{
    /// <summary>
    /// Add event calls for locomotion events
    /// This currently only needs to describe the player entering the ladder but might be expanded later depending on the game's needs.
    /// </summary>
    public static class PlayerLocomotionEvents
    {

        public static event Action<LG_Ladder> OnPlayerEnterLadder;

        public static void LadderEntered(PlayerAgent owner)
        {
            if(OnPlayerEnterLadder != null && owner.IsLocallyOwned)
            {
                GTFO_VR_Plugin.log.LogDebug("Player entered ladder");
                GTFO_VR_Plugin.log.LogDebug(owner.Locomotion.CurrentLadder);
                OnPlayerEnterLadder.Invoke(owner.Locomotion.CurrentLadder);
            }
        }

    }
}
