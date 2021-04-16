using GTFO_VR.Events;
using HarmonyLib;

namespace GTFO_VR.Injections.Events
{
    /// <summary>
    /// Inject event calls for focus state updates
    /// Focus states describe the current 'focus' context of the game --- InTerminal, Elevator, MainMenu etc.
    /// </summary>

    [HarmonyPatch(typeof(InputMapper), nameof(InputMapper.OnFocusStateChanged))]
    internal class InjectFocusStateEvents
    {
        private static void Prefix(eFocusState state)
        {
            FocusStateEvents.FocusChanged(state);
        }
    }
}