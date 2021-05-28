using GTFO_VR.Events;
using HarmonyLib;
using Player;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GTFO_VR.Injections.Events
{
    [HarmonyPatch(typeof(PlayerLocomotion), nameof(PlayerLocomotion.ChangeState))]
    internal class InjectPLOCEvents
    {
        private static void Postfix(PlayerLocomotion __instance, PlayerLocomotion.PLOC_State state, bool wasWarpedIntoState = false)
        {
            if(!__instance.m_owner.IsLocallyOwned)
            {
                return;
            }
            PlayerLocomotionEvents.StateChanged(state);
        }
    }
}
