using GTFO_VR.UI.CANVAS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;

namespace GTFO_VR.Core.UI.canvas.KeyboardDefinition
{
    class RectColliderSizer : MonoBehaviour // Replacement for UIBehaviour::OnRectTransformDimensionsChange() because unhollowed libs don't have the virtual modifier (fixed by updating bepinex )
    {
        RectTransform m_rectTransform;

        private float m_Width;
        private float m_Height;

        void handleResize()
        {
            BoxCollider collider = GetComponent<BoxCollider>();
            RectTransform trans = GetComponent<RectTransform>();

            // multiplier is Temporary until I can figure out a better way of adding borders
            collider.size = new Vector3(trans.sizeDelta.x * TerminalKeyboardInterface.HITBOX_SCALE, trans.sizeDelta.y * TerminalKeyboardInterface.HITBOX_SCALE, collider.size.z);
        }

        void Start()
        {
            m_rectTransform = GetComponent<RectTransform>();
            if (m_rectTransform != null)
            {
                m_Width = m_rectTransform.sizeDelta.x;
                m_Height = m_rectTransform.sizeDelta.y;
            }

            handleResize();
        }

        void Update()
        {
            if (m_rectTransform != null)
            {
                if (m_rectTransform.sizeDelta.x != m_Width || m_rectTransform.sizeDelta.y != m_Height)
                {
                    m_Width = m_rectTransform.sizeDelta.x;
                    m_Height = m_rectTransform.sizeDelta.y;

                    handleResize();
                }
            }
        }
    }
}
