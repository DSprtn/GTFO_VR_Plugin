using GTFO_VR.Events;
using HarmonyLib;
using Player;

namespace GTFO_VR.Injections.Events
{
    /// <summary>
    /// Add event calls for entering ladders (need to re-orient the player and playspace, etc.)
    /// </summary>

    [HarmonyPatch(typeof(PLOC_ClimbLadder), nameof(PLOC_ClimbLadder.Enter))]
    internal class InjectLadderPLOCEvent
    {
        private static void Postfix(PLOC_ClimbLadder __instance)
        {
            if(__instance.m_owner.IsLocallyOwned)
            {
                PlayerLocomotionEvents.LadderEntered(__instance.m_owner.Cast<LocalPlayerAgent>());
            }    
        }
    }
}