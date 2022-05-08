using Assets.scripts.KeyboardDefinition;
using Assets.scripts.Pointer;
using System.Collections;
using System.Collections.Generic;
using UnhollowerBaseLib.Attributes;
using UnityEngine;

namespace GTFO_VR.UI.CANVAS
{
    public class TerminalKeyboardCanvas : MonoBehaviour
    {
        public Canvas m_canvas;
        public TerminalKeyboardInterface m_keyboardRoot;

        public static TerminalKeyboardCanvas attach(GameObject go, float width, float height, TextAnchor gravity)
        {
            TerminalKeyboardCanvas canvas = go.AddComponent<TerminalKeyboardCanvas>();

            canvas.transform.localPosition = new Vector3();
            canvas.transform.localRotation = new Quaternion();
            canvas.transform.localScale = new Vector3(TerminalKeyboardInterface.CANVAS_SCALE,
                                                        TerminalKeyboardInterface.CANVAS_SCALE,
                                                        TerminalKeyboardInterface.CANVAS_SCALE);

            canvas.m_canvas = go.AddComponent<Canvas>();
            canvas.m_canvas.renderMode = RenderMode.WorldSpace;

            go.AddComponent<UnityEngine.UI.GraphicRaycaster>();

            RectTransform rect = go.GetComponent<RectTransform>();
            rect.sizeDelta = new Vector2(width, height);

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
            VRInputModule.m_TargetCanvases.Add(m_canvas);
            m_canvas.worldCamera = VRInputModule.getOrCreate().m_Camera;
        }

        private void OnDisable()
        {
            VRInputModule.m_TargetCanvases.Remove(m_canvas);
        }
    }

}
