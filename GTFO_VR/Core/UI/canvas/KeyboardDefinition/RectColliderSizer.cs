using Assets.scripts.canvas;
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
            RoundedCubeBackground roundedBackground = GetComponent<RoundedCubeBackground>();

            if (collider != null)
            {
                collider.size = new Vector3(trans.sizeDelta.x, trans.sizeDelta.y, collider.size.z);
            }          
            
            if (roundedBackground != null)
            {
                roundedBackground.setSize(trans.sizeDelta.x, trans.sizeDelta.y);
            }
            
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
