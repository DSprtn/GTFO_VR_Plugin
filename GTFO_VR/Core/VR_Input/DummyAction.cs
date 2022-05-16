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
        private readonly InputAction m_action;
        private bool m_autoUp = false;
        private bool m_down = false;
        private bool m_up = false;

        public DummyAction( InputAction action, bool autoUp )
        {
            m_action = action;
            m_autoUp = autoUp;
        }

        public void requestPress()
        {

            m_down = true;
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
}
