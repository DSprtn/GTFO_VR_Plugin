﻿using GTFO_VR.Core.UI.Terminal.KeyboardDefinition;
using Il2CppInterop.Runtime.Attributes;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace GTFO_VR.Core.UI.Terminal
{


    /// <summary>
    /// Holder canvas for each section of the terminal keyboard
    /// </summary>
    public class TerminalKeyboardCanvas : MonoBehaviour
    {
        private UnityEngine.Canvas m_canvas;

        public TerminalKeyboardCanvas(IntPtr value) : base(value) { }

        public static TerminalKeyboardCanvas Instantiate(GameObject go, float width, float height, TextAnchor gravity)
        {
            TerminalKeyboardCanvas canvas = go.AddComponent<TerminalKeyboardCanvas>();

            canvas.transform.localPosition = new Vector3();
            canvas.transform.localRotation = new Quaternion();
            canvas.transform.localScale = new Vector3(TerminalKeyboardInterface.CANVAS_SCALE,
                                                        TerminalKeyboardInterface.CANVAS_SCALE,
                                                        TerminalKeyboardInterface.CANVAS_SCALE);

            canvas.m_canvas = go.AddComponent<UnityEngine.Canvas>();
            canvas.m_canvas.renderMode = RenderMode.WorldSpace;

            RectTransform rect = go.GetComponent<RectTransform>();
            rect.sizeDelta = new Vector2(width, height);

            // Needed to make the child respect its own layout element
            VerticalLayoutGroup layoutGroup = go.AddComponent<VerticalLayoutGroup>();
            layoutGroup.childAlignment = gravity;
            layoutGroup.childForceExpandHeight = false;
            layoutGroup.childForceExpandWidth = false;

            return canvas;
        }

        [HideFromIl2Cpp]
        public void InflateLayout(TerminalKeyboardInterface keyboardRoot, KeyboardLayout layout, KeyboardStyle style)
        {
            GameObject child = layout.GenerateLayout(keyboardRoot, style);

            // Canvas should only have a single child
            child.transform.SetParent(this.transform, false);
            child.GetComponent<RectTransform>().sizeDelta = this.gameObject.GetComponent<RectTransform>().sizeDelta;
        }
    }

}
