using HarmonyLib;
using LevelGeneration;
using GTFO_VR.Events;
using Player;

namespace GTFO_VR.Injections.Events
{
    /// <summary>
    /// Add event calls for player interactions
    /// </summary>

    [HarmonyPatch(typeof(LG_DoorButton), nameof(LG_DoorButton.Interact))]
    internal class InjectDoorButtonInteract
    {
        private static void Postfix(PlayerAgent source)
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
}