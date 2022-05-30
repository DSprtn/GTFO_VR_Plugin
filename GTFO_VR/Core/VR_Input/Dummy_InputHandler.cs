using System;
using System.Collections.Generic;
using UnityEngine;

namespace GTFO_VR.Core.VR_Input
{

    /// <summary>
    /// Works just like a SteamVR boolean input, but you can call down and have GTFO treat it as a normal input.
    /// Basically what the WeaponRadialMenu already does, but compartmentalized.
    /// </summary>
    public class DummyAction
    {
        private bool m_autoUp = true;

        private bool m_state = false;
        private bool m_down = false;
        private bool m_up = false;

        private float m_duration = 0;

        private float m_downTime = -1;

        private int m_firstDownFrame = -1;
        private int m_firstUpFrame = -1;

        public DummyAction()
        {
            // Default 1-frame down-up
        }

        public DummyAction(bool autoUp)
        {
            // Only set to false if you intend to call requestUp() manually
            m_autoUp = autoUp;
        }

        public DummyAction(float duration)
        {
            // Keep the key pressed for duration
            m_duration = duration;
        }

        public void requestDown()
        {
            if (m_down)
            {
                // Key hit again before being released
                m_up = true;
                m_firstUpFrame = -1; // Set when getUp() first called
            }

            m_downTime = Time.time;
            m_firstDownFrame = -1; // Set when getDown() first called
            m_state = true;
            m_down = true;
        }

        public void requestUp()
        {
            if ( m_state)
            {
                m_state = false;
                m_up = true;
                m_firstUpFrame = -1; // Set when getUp() first called
            }
        }

        public bool getState()
        {
            return m_state;
        }

        public bool getDown()
        {
            if (m_state)
            {
                if (m_down)
                {
                    // Inputs are presumably handled at the beginning of each frame, while the state of the
                    // dummy inputs may change whenever. Resetting m_down when getDown() is called works as long as 
                    // it is only called once a frame, but unfortunately it is sometimes called multiple times a frame.
                    // ( e.g. TerminalExit in R6.5DX after and only after jumping to the outside. persists for duration of mission )
                    // Keep track of the first call to getDown() and don't reset until frame changes.
                    // Ditto for getUp(), though it is not used for any currently mapped action.

                    // First time getDown() was called since down was requested. Keep returning true until next frame.
                    if (m_firstDownFrame == -1)
                    {
                        m_firstDownFrame = Time.frameCount;
                    }
                    else if ( Time.frameCount != m_firstDownFrame)
                    {
                        m_down = false;
                    }
                }

                // Can begin returning up as soon as we stop returning down
                if (!m_down )
                {
                    // Let keyDown persist for at least one frame
                    if (m_autoUp)
                    {
                        if ((Time.time - m_downTime) > m_duration)
                        {
                            requestUp();
                        }
                    }
                }

                return m_down;
            }

            return false;
        }

        public bool getUp()
        {
            if (m_up)
            {
                // see getDown()
                if (m_firstUpFrame == -1)
                {
                    m_firstUpFrame = Time.frameCount;
                }
                else if ( Time.frameCount != m_firstUpFrame)
                {
                    m_up = false;
                    return false;
                }

                return true;
            }

            return false;
        }
    }

    class Dummy_InputHandler
    {
        private static DummyAction m_terminalDeleteAction = new DummyAction();
        private static DummyAction m_terminalExitAction =   new DummyAction();
        private static DummyAction m_terminalLeftAction =   new DummyAction();
        private static DummyAction m_terminalUpAction =     new DummyAction();
        private static DummyAction m_terminalRightAction =  new DummyAction();
        private static DummyAction m_terminalDownAction =   new DummyAction();

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
            if (dummyAction != null)
                return dummyAction.getState();
            return false;
        }


        /// <summary>
        /// Trigger the requested action, if it has been mapped
        /// </summary>
        public static void triggerDummyAction(InputAction action)
        {
            DummyAction dummyAction = GetDummyBoolActionMapping(action);
            if (dummyAction != null)
            {
                dummyAction.requestDown();
            }
        }

    }
}
