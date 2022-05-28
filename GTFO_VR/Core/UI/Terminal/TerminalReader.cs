﻿using GTFO_VR.Core.UI.Terminal.Pointer;
using GTFO_VR.Util;
using System;
using System.Collections.Generic;
using TMPro;
using UnhollowerBaseLib.Attributes;
using UnityEngine;

namespace GTFO_VR.Core.UI.Terminal

{
    public class TerminalReader : MonoPointerEvent
    {
        public TerminalReader(IntPtr value) : base(value) { }

        private TerminalKeyboardInterface m_keyboardRoot;

        private static readonly float READER_SIZE_PADDING = 1;
        private static readonly float READER_POINTER_SIZE = 0.005f;

        private TextMeshPro m_textMesh;
        private BoxCollider m_collider;

        private int m_PreviousIndex = -1;
        private int m_PreviousIndexStart = -1;
        private int m_PreviousIndexEnd = -1;
        private string m_currentSelection = "";

        private GameObject m_highlight;
        private Material m_underlineMaterial;

        private PointerHistory m_pointerHistory = new PointerHistory();

        public static TerminalReader Create( TerminalKeyboardInterface keyboardRoot )
        {
            GameObject terminalReaderRoot = new GameObject();
            terminalReaderRoot.layer = TerminalKeyboardInterface.LAYER;
            terminalReaderRoot.name = "terminalReader";

            TerminalReader reader = terminalReaderRoot.AddComponent<TerminalReader>();
            reader.m_keyboardRoot = keyboardRoot;

            return reader;
        }

        private void Awake()
        {   
            ///////////////
            // Collider
            ///////////////

            m_collider = this.gameObject.AddComponent<BoxCollider>();

            ////////////////
            // Underline
            ////////////////

            // Line renderer doesn't want to display in-game here, so a quad it is.
            m_highlight = GameObject.CreatePrimitive(PrimitiveType.Quad);
            GameObject.Destroy(m_highlight.GetComponent<MeshCollider>());

            m_highlight.transform.SetParent(this.gameObject.transform);
            m_highlight.transform.localRotation = new Quaternion();
            m_highlight.transform.localScale = new Vector3(0, 0, 0);
        }

        private void Start()
        {
            m_highlight.GetComponent<MeshRenderer>().sharedMaterial = getUnderlineMaterial();
        }

        public void attachToTerminal(GameObject textCanvas)
        {
            this.transform.SetPositionAndRotation(textCanvas.transform.position, textCanvas.transform.rotation);
            this.transform.localScale = new Vector3(TerminalKeyboardInterface.CANVAS_SCALE, TerminalKeyboardInterface.CANVAS_SCALE, TerminalKeyboardInterface.CANVAS_SCALE);

            this.m_textMesh = textCanvas.GetComponent<TMPro.TextMeshPro>(); ;

            RectTransform terminalCanvasRect = textCanvas.GetComponent<RectTransform>();
            m_collider.size = new Vector3(terminalCanvasRect.sizeDelta.x + READER_SIZE_PADDING, terminalCanvasRect.sizeDelta.y + READER_SIZE_PADDING, 0.1f);
        }

        public void deatchFromTerminal()
        {
            m_textMesh = null;
        }

        private Material getUnderlineMaterial()
        {
            if (m_underlineMaterial == null)
            {
                Material lineMaterial = new Material(Shader.Find("Unlit/Color"));
                lineMaterial.color = m_keyboardRoot.getStyle().TerminalHighlightColor;
                m_underlineMaterial = lineMaterial;
            }

            return m_underlineMaterial;
        }

        public int findNearestCharacter(Vector3 position )
        {
            if (m_textMesh == null)
                return -1;

            int res = m_textMesh.FindNearestCharacterInWorldSpace(position);

            if (res < 0)
                return -1;

            return res;
        }   

        public void clearSelection()
        {
            m_currentSelection = "";

        }

        [HideFromIl2Cpp]
        private void drawHighlight( TMP_CharacterInfo[] characters, int start, int end)
        {
            // Sanity check
            if (start < 0 || end >= m_textMesh.textInfo.characterInfo.Length)
                return;

            TMP_CharacterInfo first = m_textMesh.textInfo.characterInfo[start];

            TMP_CharacterInfo last = m_textMesh.textInfo.characterInfo[end];

            float lineHeight = m_textMesh.textInfo.lineInfo[first.lineNumber].lineHeight;

            Vector3 topLeft = first.topLeft;
            Vector3 bottomRight = last.bottomRight;

            float width = Vector3.Distance(topLeft, bottomRight);

            width += Vector3.Distance(first.bottomLeft, first.bottomRight);
            lineHeight *= 1.1f;

            Vector3 center = (topLeft + bottomRight) * 0.5f;

            m_highlight.transform.position = m_textMesh.transform.TransformPoint( center );
            m_highlight.transform.position += this.transform.forward * 0.0001f; // at 0.001f the very bottom will clip into the backgorund.
                                                                                // maybe check render queue and adjust accordingly.

            m_highlight.transform.localScale = new Vector3(width, lineHeight, 0.03f);
        }

        private static HashSet<Char> DELIMITERS = new HashSet<char>() { '\'','\"', '\\', ' ', '\r', '\n', '\t', '\f', '\v', '<', '>', ',' };

        public string getSelection()
        {
            return m_currentSelection;
        }

        public void submitSelection(bool addSpace)
        {
            m_keyboardRoot.HandleInput(getSelection() + (addSpace ? " " : "") );
        }

        public void hoverPointer(Vector3 position)
        {
            int nearestChar = findNearestCharacter(position);

            // Out of range
            if (nearestChar < 0)
                return;

            // Same character, don't do anything
            if (nearestChar == m_PreviousIndex)
            {
                return;
            }

            m_PreviousIndex = nearestChar;

            // Iterate backwards and forwards from position to find word to select.
            // The index returned above is per character, excluding any extra formatting text like <b> </b>
            // Unfortunately, this does not include any whitespace or separating characters when text
            // is indented. Get the corresponding index in the original string and treat < > as delimiters.

            TMP_CharacterInfo[] text = m_textMesh.textInfo.characterInfo;
            string rawText = m_textMesh.text;

            int nearestOriginalIndex = text[nearestChar].index;

            // This will often be out of range when the pointer is near the bottom of the terminal
            // when a bunch of text is dumped into it.
            if (nearestOriginalIndex >= rawText.Length || nearestOriginalIndex < 0)
                return;

            int indexStart = nearestOriginalIndex;
            int indexEnd = nearestOriginalIndex;

            for (int i = nearestOriginalIndex; i < rawText.Length; i++ )
            {
                if (DELIMITERS.Contains(rawText[i]) )
                {
                    break;
                }

                indexEnd = i;
            }

            for (int i = nearestOriginalIndex; i >= 0; i--)
            {
                if (DELIMITERS.Contains(rawText[i]))
                {
                    break;
                }

                indexStart = i;
            }

            // Same word, do nothing.
            if (indexStart == m_PreviousIndexStart && indexEnd == m_PreviousIndexEnd)
            {
                return;
            }

            //////////////////////////////////////////
            // Highlight selected word,
            //////////////////////////////////////////

            // Sanity check
            if (indexStart < 0 || indexEnd >= rawText.Length)
                return;

            clearSelection();

            // To draw the highlight we're back to needing the characterInfo indicides
            // index offset should be the same for both the characterinfo array and original string,
            // so we can just math it.
            int charIndexStart = (nearestChar) + (indexStart - nearestOriginalIndex);
            int charIndexEnd =   (nearestChar) + (indexEnd - nearestOriginalIndex);
            drawHighlight(text, charIndexStart, charIndexEnd);

            /////////////////
            /// Extract text
            /////////////////

            m_currentSelection = "";

            for (int i = indexStart; i < indexEnd + 1; i++)
            {
                m_currentSelection += rawText[i];
            }

            m_PreviousIndexEnd = indexEnd;
            m_PreviousIndexStart = indexStart;
        }

        private void OnDestroy()
        {
            if (m_underlineMaterial != null)
            {
                UnityEngine.Object.Destroy(m_underlineMaterial);
            }
        }

        [HideFromIl2Cpp]
        public override void OnPointerEnter(PointerEvent ev)
        {
            m_pointerHistory.clearPointerHistory();
        }

        [HideFromIl2Cpp]
        public override void OnPointerExit(PointerEvent ev)
        {
            m_highlight.transform.localScale = Vector3.zero;
            m_PreviousIndexStart = -1;
            m_PreviousIndex = -1;
            m_PreviousIndexEnd = -1;
        }

        [HideFromIl2Cpp]
        public override Vector3 onPointerMove(PointerEvent ev)
        {
            m_pointerHistory.addPointerHistory(ev.Position);
            Vector3 smoothed = m_pointerHistory.getSmoothenedPointerPosition();
            hoverPointer(smoothed);
            return smoothed;
        }

        [HideFromIl2Cpp]
        public override void onPointerDown(PointerEvent ev)
        {
            submitSelection(true);
        }

        public override float getPointerSize(float defaultSize)
        {
            // Tiny text, tiny pointer
            return READER_POINTER_SIZE;
        }
    }

 
}
