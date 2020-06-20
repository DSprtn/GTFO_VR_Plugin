using Player;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace GTFO_VR.Events
{
    public static class PlayerLocomotionEvents
    {

        public static event Action<LG_Ladder> OnPlayerEnterLadder;

        public static void LadderEntered(PlayerAgent owner)
        {
            if(OnPlayerEnterLadder != null && owner.IsLocallyOwned)
            {
                Debug.Log("Player entered ladder");
                OnPlayerEnterLadder.Invoke(owner.Locomotion.CurrentLadder);
            }
        }

    }
}
