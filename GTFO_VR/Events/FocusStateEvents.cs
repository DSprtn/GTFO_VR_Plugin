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
                currentState = state;
                OnFocusStateChange.Invoke(state);
                lastState = state;
            }
        }

        public static bool IsInGameState(eFocusState state)
        {
            if (state.Equals(eFocusState.FPS) || state.Equals(eFocusState.InElevator))
            {
                return true;
            }
            return false;
        }
    }
}