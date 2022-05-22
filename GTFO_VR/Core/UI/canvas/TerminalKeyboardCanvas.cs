using Assets.scripts.KeyboardDefinition;
using GTFO_VR.Core.UI.Canvas.KeyboardDefinition;
using System.Collections;
using System.Collections.Generic;
using UnhollowerBaseLib.Attributes;
using UnityEngine;
using UnityEngine.UI;

namespace GTFO_VR.Core.UI.Canvas
{
    public class TerminalKeyboardCanvas : MonoBehaviour
    {
        public UnityEngine.Canvas m_canvas;
        public TerminalKeyboardInterface m_keyboardRoot;

        public static TerminalKeyboardCanvas attach(GameObject go, float width, float height, TextAnchor gravity)
        {
            TerminalKeyboardCanvas canvas = go.AddComponent<TerminalKeyboardCanvas>();

            canvas.transform.localPosition = new Vector3();
            canvas.transform.localRotation = new Quaternion();
            canvas.transform.localScale = new Vector3(TerminalKeyboardInterface.CANVAS_SCALE,
                                                        TerminalKeyboardInterface.CANVAS_SCALE,
                                                        TerminalKeyboardInterface.CANVAS_SCALE);

            canvas.m_canvas = go.AddComponent<UnityEngine.Canvas>();
            canvas.m_canvas.renderMode = RenderMode.WorldSpace;

            go.AddComponent<UnityEngine.UI.GraphicRaycaster>();

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
        public void inflateLayout(TerminalKeyboardInterface keyboardRoot, KeyboardLayout layout, KeyboardStyle style)
        {
            GameObject child = layout.GenerateLayout(keyboardRoot, style);

            // Canvas should only have a single child with the same dimensions as its parent
            child.transform.position = this.transform.position;
            child.transform.localScale = this.transform.localScale;
            child.transform.rotation = this.transform.rotation;
            child.GetComponent<RectTransform>().sizeDelta = this.gameObject.GetComponent<RectTransform>().sizeDelta;
            child.transform.SetParent(this.transform);

        }

        private void Start()
        {
            //VRInputModule.AddCanvas(m_canvas);
            //m_canvas.worldCamera = VRInputModule.getOrCreate().m_Camera;
        }

        private void OnDisable()
        {
            //VRInputModule.RemoveCanvas(m_canvas);
        }
    }

}
