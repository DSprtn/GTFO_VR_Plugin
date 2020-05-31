using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GTFO_VR.Events
{
    public static class FocusStateEvents
    {

        public static event FocusStateChange OnFocusStateChange;
        public delegate void FocusStateChange(eFocusState newState);

        public static eFocusState lastState = eFocusState.MainMenu;

        public static eFocusState currentState = eFocusState.MainMenu;
        public static void FocusChanged(eFocusState state)
        {
            if(OnFocusStateChange != null)
            {
                currentState = state;
                OnFocusStateChange.Invoke(state);
                lastState = state;
            }
        }

        public static bool IsInGameState(eFocusState state)
        {
            if(state.Equals(eFocusState.FPS) || state.Equals(eFocusState.InElevator))
            {
                return true; 
            }
            return false;
        }

    }
}
