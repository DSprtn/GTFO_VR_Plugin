using GTFO_VR.Core;

namespace GTFO_VR.Events
{
    /// <summary>
    /// Add event calls for focus state updates
    /// Focus states describe the current 'focus' context of the game --- InTerminal, Elevator, MainMenu etc.
    /// </summary>
    public static class FocusStateEvents
    {
        public static event FocusStateChange OnFocusStateChange;

        public delegate void FocusStateChange(eFocusState newState);

        public static eFocusState lastState = eFocusState.MainMenu;

        public static eFocusState currentState = eFocusState.MainMenu;

        public static void FocusChanged(eFocusState state)
        {
            if (OnFocusStateChange != null)
            {
                Log.Debug($"Switching to state {state}");
                currentState = state;
                OnFocusStateChange.Invoke(state);
                lastState = state;
            }
        }

        public static bool IsInGame()
        {
            if (currentState.Equals(eFocusState.FPS) || currentState.Equals(eFocusState.InElevator))
            {
                return true;
            }
            return false;
        }
    }
}