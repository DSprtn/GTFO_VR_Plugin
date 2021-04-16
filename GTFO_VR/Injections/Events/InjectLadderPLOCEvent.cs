using GTFO_VR.Events;
using HarmonyLib;

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
            PlayerLocomotionEvents.LadderEntered(__instance.m_owner);
        }
    }
}