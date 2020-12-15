using GTFO_VR.Events;
using HarmonyLib;
using Player;

namespace GTFO_VR_BepInEx.Core
{
    /// <summary>
    /// Add event calls for item equips
    /// </summary>

    [HarmonyPatch(typeof(PLOC_ClimbLadder),"Enter")]
    class InjectLadderPLOCEvent
    {
        static void Postfix(PLOC_ClimbLadder __instance)
        {
            PlayerLocomotionEvents.LadderEntered(__instance.m_owner);
        }
    }
}
