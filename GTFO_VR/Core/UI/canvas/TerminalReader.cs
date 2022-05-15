using Assets.scripts.KeyboardDefinition;
using GTFO_VR.UI.CANVAS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;

namespace GTFO_VR.Core.UI.canvas
{
    public class TerminalReader : MonoBehaviour
    {
        private GameObject m_textCanvas;

        private TerminalKeyboardInterface m_keyboardRoot;

        public TextMeshPro m_textMesh;
        private BoxCollider m_collider;

        private int m_lastIndex = -1;

        private int m_lastFirstIndex = -1;
        private int m_lastLastIndex = -1;

        private string m_currentSelection = "";

        private LineRenderer m_LineRenderer;

        private static readonly float MAX_DISTANCE = 0.1f * TerminalKeyboardInterface.CANVAS_SCALE;

        public static GameObject Create( GameObject textCanvas, TerminalKeyboardInterface keyboardRoot )
        {
            GameObject terminalReaderRoot = new GameObject();
            terminalReaderRoot.layer = TerminalKeyboardInterface.LAYER;
            terminalReaderRoot.name = "terminalReader";

            TerminalReader reader = terminalReaderRoot.AddComponent<TerminalReader>();
            reader.m_textCanvas = textCanvas;
            reader.m_collider = terminalReaderRoot.AddComponent<BoxCollider>();
            reader.m_keyboardRoot = keyboardRoot;

            return terminalReaderRoot;
        }

        private void Start()
        {
            this.transform.SetPositionAndRotation(m_textCanvas.transform.position, m_textCanvas.transform.rotation);
            this.transform.localScale = new Vector3(TerminalKeyboardInterface.CANVAS_SCALE, TerminalKeyboardInterface.CANVAS_SCALE, TerminalKeyboardInterface.CANVAS_SCALE);

            this.m_textMesh = m_textCanvas.GetComponent<TMPro.TextMeshPro>(); ;

            RectTransform terminalCanvasRect = m_textCanvas.GetComponent<RectTransform>();
            m_collider.size = new Vector3(terminalCanvasRect.sizeDelta.x, terminalCanvasRect.sizeDelta.y, 0.1f);

            ////////////////
            // Underline
            ////////////////

            m_LineRenderer = gameObject.AddComponent<LineRenderer>();
            m_LineRenderer.receiveShadows = false;
            m_LineRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;

            Material lineMaterial = new Material(Shader.Find("Unlit/Color"));
            m_LineRenderer.material = lineMaterial;
            lineMaterial.renderQueue = (int)RenderQueue.Overlay + 2;
            lineMaterial.color = KeyboardStyle.getPointerColor();
            m_LineRenderer.widthMultiplier = 0.001f;
        }

        private float getDistanceToCharacter( Vector3 from, TMP_CharacterInfo chrInfo )
        {

            // Treat character bounds as a sphere, calculate distance to outer bounds
            Vector3 crossSectionVector = chrInfo.topLeft - chrInfo.bottomRight;
            float crossSection = m_textMesh.transform.TransformPoint(crossSectionVector).magnitude;
            Vector3 center = (chrInfo.topLeft + chrInfo.bottomRight) / 2;
            center = m_textMesh.transform.TransformPoint(center);

            Debug.DrawLine(from, center);

            float distanceToCenter = (from - center).sqrMagnitude;

            if (distanceToCenter < crossSection)
                return 0;
            return distanceToCenter - crossSection;
        }

        public int findNearestCharacter(Vector3 position, float maxDistance  )
        {
            if (m_textMesh == null)
                return -1;

            int res = TMP_TextUtilities.FindNearestCharacter(m_textMesh, position, null, true);

            if (res < 0)
                return -1;

            float distance = getDistanceToCharacter(position, m_textMesh.textInfo.characterInfo[res]);

            if (distance > maxDistance)
                return -1;

            return res;
        }

        public void clearSelection()
        {
            m_currentSelection = "";

        }

        private void drawHighlight( int start, int end)
        {
            // Sanity check
            if (start < 0 || end >= m_textMesh.textInfo.characterInfo.Length)
                return;

            TMP_CharacterInfo first = m_textMesh.textInfo.characterInfo[start];
            TMP_CharacterInfo last = m_textMesh.textInfo.characterInfo[end];

            Vector3 firstVec = m_textMesh.transform.TransformPoint(first.bottomLeft);
            Vector3 lastVec = m_textMesh.transform.TransformPoint(last.bottomRight);

            firstVec -= this.transform.forward * 0.05f;
            lastVec -= this.transform.forward * 0.05f;

            m_LineRenderer.SetPosition(0, firstVec);
            m_LineRenderer.SetPosition(1, lastVec);
        }

        private static HashSet<Char> DELIMITERS = new HashSet<char>() { '\"', '\\', ' ', '\r', '\n', '\t', '\f', '\v', '<', '>' };

        public string getSelection()
        {
            return m_currentSelection;
        }

        // Either the pointer has a reference to the interface, or the reader does.
        public void submitSelection(bool addSpace)
        {
            m_keyboardRoot.HandleInput(getSelection() + (addSpace ? " " : "") );
        }

        public void hoverPointer(Vector3 position)
        {

            int nearestChar = findNearestCharacter(position, MAX_DISTANCE);

            // Out of range
            if (nearestChar < 0)
                return;

            // Same character, don't do anything
            if (nearestChar == m_lastIndex)
            {
                return;
            }

            m_lastIndex = nearestChar;

            // Start from index, find complete word.
            // Delimieters are space, linebreak, and "". Maybe : ?

            // The index returned is per character, excluding any extra formatting text like <b> </b>
            // Unfortunately, this does not include any whitespace or separating characters when text
            // is indented. Get the corresponding index in the original string and treat < > as delimiters.
            TMP_CharacterInfo[] text = m_textMesh.textInfo.characterInfo;
            string rawText = m_textMesh.text;

            ///////////////////////////////////////////
            // Get first and list char of selection
            /////////////////////////////////////////

            int nearestOriginalIndex = text[nearestChar].index;

            int firstIndex = nearestOriginalIndex;
            int lastIndex = nearestOriginalIndex;

            for (int i = nearestOriginalIndex; i < rawText.Length; i++ )
            {
                if (DELIMITERS.Contains((rawText[i])) )
                {
                    break;
                }

                lastIndex = i;
            }

            for (int i = nearestOriginalIndex; i > -1; i--)
            {
                if (DELIMITERS.Contains((rawText[i])))
                {
                    break;
                }

                firstIndex = i;
            }

            // Same word, do nothing.
            if (firstIndex == m_lastFirstIndex && lastIndex == m_lastLastIndex)
            {
                return;
            }

            //////////////////////////////////////////
            // Highlight selected word,
            //////////////////////////////////////////

            // Sanity check
            if (firstIndex < 0 || lastIndex > rawText.Length)
                return;

            clearSelection();

            // To draw the highlight we're back to needing the characterInfo indicides
            // index offset should be the same for both the characterinfo array and original string,
            // so we can just math it.
            int charIndexStart = (nearestChar) + (firstIndex - nearestOriginalIndex);
            int charIndexEnd =   (nearestChar) + (lastIndex - nearestOriginalIndex);

            drawHighlight(charIndexStart, charIndexEnd);

            /////////////////
            /// Extract text
            /////////////////

            m_currentSelection = "";

            for (int i = firstIndex; i < lastIndex + 1; i++)
            {
                m_currentSelection += rawText[i];
            }

            m_lastLastIndex = lastIndex;
            m_lastFirstIndex = firstIndex;
        }
    }
}
