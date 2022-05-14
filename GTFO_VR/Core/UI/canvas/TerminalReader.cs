using GTFO_VR.UI.CANVAS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

namespace GTFO_VR.Core.UI.canvas
{
    public class TerminalReader : MonoBehaviour
    {
        private GameObject m_textCanvas;

        public TextMeshPro m_textMesh;
        private BoxCollider m_collider;

        private int m_lastIndex = -1;

        public static GameObject Create( GameObject textCanvas )
        {
            GameObject terminalReaderRoot = new GameObject();
            terminalReaderRoot.layer = TerminalKeyboardInterface.LAYER;
            terminalReaderRoot.name = "terminalReader";

            TerminalReader reader = terminalReaderRoot.AddComponent<TerminalReader>();
            reader.m_textCanvas = textCanvas;
            reader.m_collider = terminalReaderRoot.AddComponent<BoxCollider>();

            return terminalReaderRoot;
        }

        private void Start()
        {
            this.transform.SetPositionAndRotation(m_textCanvas.transform.position, m_textCanvas.transform.rotation);
            this.transform.localScale = new Vector3(TerminalKeyboardInterface.CANVAS_SCALE, TerminalKeyboardInterface.CANVAS_SCALE, TerminalKeyboardInterface.CANVAS_SCALE);

            this.m_textMesh = m_textCanvas.GetComponent<TMPro.TextMeshPro>(); ;

            RectTransform terminalCanvasRect = m_textCanvas.GetComponent<RectTransform>();
            m_collider.size = new Vector3(terminalCanvasRect.sizeDelta.x, terminalCanvasRect.sizeDelta.y, 0.1f);
        }

        public int findNearestCharacter(Vector3 position )
        {
            if (m_textMesh == null)
                return -1;

            int res = TMP_TextUtilities.FindNearestCharacter(m_textMesh, position, null, true);

            if (m_lastIndex != res)
            {
                Debug.Log("Index: " + res);
            }

            m_lastIndex = res;


            return res;

        }
        


    }
}
