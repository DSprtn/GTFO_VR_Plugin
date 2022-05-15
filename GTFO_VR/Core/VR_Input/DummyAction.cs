using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace GTFO_VR.Core.VR_Input
{
    class DummyAction
    {
        InputAction m_action;

        bool m_autoUp = false;

        bool m_down = false;
        bool m_up = false;

        public DummyAction( InputAction action, bool autoUp )
        {
            m_action = action;
            m_autoUp = autoUp;
        }

        public void requestPress()
        {
            Debug.Log("requesting press: " + m_action);

            m_down = true;
        }

        public bool getDown()
        {
            if (m_down)
            {
                m_up = m_autoUp;
                m_down = false;

                Debug.Log("Down for action " + m_action);

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
}
