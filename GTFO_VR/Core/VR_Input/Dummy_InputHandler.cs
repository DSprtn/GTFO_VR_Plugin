using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GTFO_VR.Core.VR_Input
{
    public class DummyAction
    {
        private bool m_autoUp = false;
        private bool m_down = false;
        private bool m_up = false;

        public DummyAction(bool autoUp)
        {
            m_autoUp = autoUp;
        }

        public void requestPress()
        {

            m_down = true;
        }

        public bool getState()
        {
            return m_down;
        }

        public bool getDown()
        {
            if (m_down)
            {
                m_up = m_autoUp;
                m_down = false;

                return true;
            }

            return false;
        }

        public bool getUp()
        {
            if (m_up)
            {
                m_up = false;
                return true;
            }

            return false;
        }
    }

    class Dummy_InputHandler
    {
        private static DummyAction m_terminalDeleteAction = new DummyAction( true );
        private static DummyAction m_terminalExitAction =   new DummyAction( true );

        private static DummyAction m_terminalLeftAction =   new DummyAction( true );
        private static DummyAction m_terminalUpAction =     new DummyAction( true);
        private static DummyAction m_terminalRightAction =  new DummyAction( true );
        private static DummyAction m_terminalDownAction =   new DummyAction( true );

        private static Dictionary<InputAction, DummyAction> dummyBoolActions;

        static Dummy_InputHandler()
        {
            dummyBoolActions = new Dictionary<InputAction, DummyAction>
            {
                { InputAction.TerminalDel, m_terminalDeleteAction },
                { InputAction.TerminalExit, m_terminalExitAction },

                { InputAction.TerminalLeft, m_terminalLeftAction },
                { InputAction.TerminalUp, m_terminalUpAction },
                { InputAction.TerminalRight, m_terminalRightAction },
                { InputAction.TerminalDown, m_terminalDownAction }
            };
        }

        private static DummyAction GetDummyBoolActionMapping(InputAction action)
        {
            if (dummyBoolActions.ContainsKey(action))
            {
                return dummyBoolActions[action];
            }
            return null;
        }

        public static bool GetActionUp(InputAction action)
        {
            DummyAction dummyAction = GetDummyBoolActionMapping(action);
            if (dummyAction != null && dummyAction.getUp())
                return true;

            return false;
        }

        public static bool GetActionDown(InputAction action)
        {
            DummyAction dummyAction = GetDummyBoolActionMapping(action);
            if (dummyAction != null && dummyAction.getDown())
                return true;

            return false;
        }

        public static bool GetAction(InputAction action)
        {
            DummyAction dummyAction = GetDummyBoolActionMapping(action);
            if (dummyAction != null && dummyAction.getDown())
                return dummyAction.getState();
            return false;
        }

        public static void triggerDummyAction(InputAction action)
        {
            DummyAction dummyAction = GetDummyBoolActionMapping(action);
            if (dummyAction != null)
            {
                dummyAction.requestPress();
            }
        }

    }
}
