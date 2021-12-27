using HarmonyLib;
using LevelGeneration;
using GTFO_VR.Events;
using Player;
using ChainedPuzzles;
using Il2CppSystem.Collections.Generic;

namespace GTFO_VR.Injections.Events
{
    /// <summary>
    /// Add event calls for player interactions
    /// </summary>

    [HarmonyPatch(typeof(LG_DoorButton), nameof(LG_DoorButton.Interact))]
    internal class InjectDoorButtonInteract
    {
        private static void Postfix(LocalPlayerAgent source)
        {
            PlayerInteractionEvents.PlayerInteracted(source);
        }
    }

    [HarmonyPatch(typeof(PUI_InteractionPrompt), nameof(PUI_InteractionPrompt.SetTimerFill))]
    internal class InjectTimerFilledInteract
    {
        private static void Postfix(float fill)
        {
            if (fill >= 1.0f)
            {
                PlayerInteractionEvents.PlayerInteracted();
            }
        }
    }

    [HarmonyPatch(typeof(CP_Bioscan_Sync), nameof(CP_Bioscan_Sync.SetStateData))]
    internal class InjectBioscanStateInteract
    {
        private static void Postfix(eBioscanStatus status, float progress, List<PlayerAgent> playersInScan)
        {
            PlayerInteractionEvents.SetBioscanState(status, progress, playersInScan);
        }
    }
}