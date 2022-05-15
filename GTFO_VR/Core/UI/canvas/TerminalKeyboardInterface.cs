using Assets.scripts.KeyboardDefinition;
using GTFO_VR.Core.UI.canvas;
using GTFO_VR.Core.VR_Input;
using System.Collections;
using System.Collections.Generic;
using UnhollowerBaseLib.Attributes;
using UnityEngine;

namespace GTFO_VR.UI.CANVAS
{


    public class TerminalKeyboardInterface : MonoBehaviour
    {
        private GameObject m_terminalCanvas;
        private KeyboardStyle m_keyboardStyle = new KeyboardStyle(2, 1);
        private Material keyboardMaterial;


        // Workaround for spacing while maintainig hitboxes
        public static readonly float KEY_SCALE = 0.96f;
        public static readonly float HITBOX_SCALE = 1.04f;


        public static readonly int LAYER = 5;
        public static readonly int LAYER_MASK = 1 << LAYER;
        public static readonly float CANVAS_SCALE = 0.045f; // Same scaling used by GTFO, because otherwise units are silly.
        private static readonly float SECTION_PADDING = 0f * CANVAS_SCALE;

        public static string currentFrameInput = "";

        bool m_dataDirty = false;


        public static GameObject create(GameObject terminalCanvas)
        {
            GameObject go = new GameObject();
            go.name = "keyboardRoot";
            go.layer = LAYER;
            TerminalKeyboardInterface inf = go.AddComponent<TerminalKeyboardInterface>();
            inf.m_terminalCanvas = terminalCanvas;
            return go;
        }

        private void LateUpdate()
        {
           m_dataDirty = true;
        }

        public static string getKeyboardInput()
        {
            return currentFrameInput;
        }


        [HideFromIl2Cpp]
        private static KeyboardLayout getBottomKeyboardLayout()
        {
            LinearLayout bottomKeyboardLayout = new LinearLayout(LinearOrientation.VERTICAL, TextAnchor.UpperCenter);

            KeyboardLayoutParameters rowParams = new KeyboardLayoutParameters(15, 1, false, false);

            {
                LinearLayout keyboardRow = new LinearLayout(LinearOrientation.HORIZONTAL, TextAnchor.UpperCenter, rowParams);

                keyboardRow.AddChild(new KeyDefinition(KeyType.ESC, "x"));
                keyboardRow.AddChild(new KeyDefinition("1"));
                keyboardRow.AddChild(new KeyDefinition("2"));
                keyboardRow.AddChild(new KeyDefinition("3"));
                keyboardRow.AddChild(new KeyDefinition("4"));
                keyboardRow.AddChild(new KeyDefinition("5"));
                keyboardRow.AddChild(new KeyDefinition("6"));
                keyboardRow.AddChild(new KeyDefinition("7"));
                keyboardRow.AddChild(new KeyDefinition("8"));
                keyboardRow.AddChild(new KeyDefinition("9"));
                keyboardRow.AddChild(new KeyDefinition("0"));
                keyboardRow.AddChild(new KeyDefinition("."));   // For typing ip addresses
                keyboardRow.AddChild(new KeyDefinition(KeyType.BACKPSPACE, "backspace", new KeyboardLayoutParameters(1, true)));

                bottomKeyboardLayout.AddChild(keyboardRow);
            }

            {
                LinearLayout keyboardRow = new LinearLayout(LinearOrientation.HORIZONTAL, TextAnchor.UpperCenter, rowParams);

                keyboardRow.AddChild(new KeyDefinition(KeyType.TAB, "tab", 1.5f));
                keyboardRow.AddChild(new KeyDefinition("q"));
                keyboardRow.AddChild(new KeyDefinition("w"));
                keyboardRow.AddChild(new KeyDefinition("e"));
                keyboardRow.AddChild(new KeyDefinition("r"));
                keyboardRow.AddChild(new KeyDefinition("t"));
                keyboardRow.AddChild(new KeyDefinition("y"));
                keyboardRow.AddChild(new KeyDefinition("u"));
                keyboardRow.AddChild(new KeyDefinition("i"));
                keyboardRow.AddChild(new KeyDefinition("o"));
                keyboardRow.AddChild(new KeyDefinition("p"));
                keyboardRow.AddChild(new KeyDefinition(KeyType.EMPTY, "-"));
                keyboardRow.AddChild(new KeyDefinition(KeyType.EMPTY, "", new KeyboardLayoutParameters(1f, true)));

                bottomKeyboardLayout.AddChild(keyboardRow);
            }


            {
                LinearLayout keyboardRow = new LinearLayout(LinearOrientation.HORIZONTAL, TextAnchor.UpperCenter, rowParams);

                keyboardRow.AddChild(new KeyDefinition(KeyType.CAPS_LOCK, "", 1.85f));
                keyboardRow.AddChild(new KeyDefinition("a"));
                keyboardRow.AddChild(new KeyDefinition("s"));
                keyboardRow.AddChild(new KeyDefinition("d"));
                keyboardRow.AddChild(new KeyDefinition("f"));
                keyboardRow.AddChild(new KeyDefinition("g"));
                keyboardRow.AddChild(new KeyDefinition("h"));
                keyboardRow.AddChild(new KeyDefinition("j"));
                keyboardRow.AddChild(new KeyDefinition("k"));
                keyboardRow.AddChild(new KeyDefinition("l"));
                keyboardRow.AddChild(new KeyDefinition("_"));
                keyboardRow.AddChild(new KeyDefinition(KeyType.ENTER, "enter", new KeyboardLayoutParameters(1f, true)));

                bottomKeyboardLayout.AddChild(keyboardRow);
            }


            {
                LinearLayout keyboardRow = new LinearLayout(LinearOrientation.HORIZONTAL, TextAnchor.UpperCenter, rowParams);

                keyboardRow.AddChild(new KeyDefinition(KeyType.SHIFT, "", 2.4f));
                keyboardRow.AddChild(new KeyDefinition("z"));
                keyboardRow.AddChild(new KeyDefinition("x"));
                keyboardRow.AddChild(new KeyDefinition("c"));
                keyboardRow.AddChild(new KeyDefinition("v"));
                keyboardRow.AddChild(new KeyDefinition("b"));
                keyboardRow.AddChild(new KeyDefinition("n"));
                keyboardRow.AddChild(new KeyDefinition("m"));
                keyboardRow.AddChild(new KeyDefinition(","));
                keyboardRow.AddChild(new KeyDefinition("."));
                keyboardRow.AddChild(new KeyDefinition(KeyType.UP, "^"));
                keyboardRow.AddChild(new KeyDefinition(KeyType.EMPTY, "", new KeyboardLayoutParameters(1f, true)));

                bottomKeyboardLayout.AddChild(keyboardRow);
            }

            {
                LinearLayout keyboardRow = new LinearLayout(LinearOrientation.HORIZONTAL, TextAnchor.UpperCenter, rowParams);

                keyboardRow.AddChild(new KeyDefinition(KeyType.EMPTY, "", 3.2f));
                keyboardRow.AddChild(new KeyDefinition(KeyType.SPACE, "space", 7.2f));
                keyboardRow.AddChild(new KeyDefinition(KeyType.LEFT, "<"));
                keyboardRow.AddChild(new KeyDefinition(KeyType.DOWN, "v"));
                keyboardRow.AddChild(new KeyDefinition(KeyType.RIGHT, ">"));
                keyboardRow.AddChild(new KeyDefinition(KeyType.EMPTY, "", new KeyboardLayoutParameters(1f, true)));

                bottomKeyboardLayout.AddChild(keyboardRow);
            }

            return bottomKeyboardLayout;
        }


        private void Start()
        {
            this.transform.position = m_terminalCanvas.transform.position;
            this.transform.rotation = m_terminalCanvas.transform.rotation;

            m_keyboardStyle.TileSize = 1;
            m_keyboardStyle.FontSize = 0.5f;
            m_keyboardStyle.SpacingHorizontal = 0.00f;
            m_keyboardStyle.SpacingVertical = 0.00f;
            m_keyboardStyle.keyPadding = 0.01f;

            /*
            Shader uiShader = Shader.Find("UI/Unlit/Detail");
            m_keyboardStyle.keyboardMaterial = new Material(uiShader);
            m_keyboardStyle.keyboardMaterial.renderQueue = 3002;

            Shader textShader = Shader.Find("TextMeshPro/Distance Field Overlay");
            m_keyboardStyle.fontMaterial = new Material(uiShader);
            m_keyboardStyle.fontMaterial.renderQueue = 3003;
            */

            /////////////////////////
            // terminal text canvas
            /////////////////////////

            GameObject terminalReaderRoot = TerminalReader.Create(m_terminalCanvas, this);
            terminalReaderRoot.transform.SetParent(this.transform);


            ///////////////////////
            // Bottom keyboard
            ///////////////////////

            GameObject bottomKeyboard = new GameObject();
            bottomKeyboard.name = "bottomKeyboard";

            bottomKeyboard.transform.SetParent(this.gameObject.transform);
            //bottomKeyboard.transform.localPosition = new Vector3();
            //bottomKeyboard.transform.localRotation = new Quaternion() ;

            RectTransform terminalCanvasRect = m_terminalCanvas.GetComponent<RectTransform>();
            float bottomKeyboardHeight = 14;
            float bottomKeyboardWidth = 16;

            TerminalKeyboardCanvas bottomKeyboardCanvas = TerminalKeyboardCanvas.attach(bottomKeyboard, bottomKeyboardWidth, bottomKeyboardHeight, TextAnchor.MiddleCenter);

            float terminalCanvasRectHeightAdjusted = terminalCanvasRect.rect.height * CANVAS_SCALE;
            float bottomKeyboardCanvasHeightAdjusted = bottomKeyboardHeight * CANVAS_SCALE;

            //////////////////////////////////////////
            // Align with bottom of terminal canvas
            //////////////////////////////////////////

            float bottomKeyboardOffset = terminalCanvasRectHeightAdjusted / 2; // distance to edge of monitor
            bottomKeyboardOffset += bottomKeyboardCanvasHeightAdjusted / 2;                // half of keyboard height so top aligns with bottom edge

            Vector3 offset = new Vector3(0,-bottomKeyboardOffset,0);
            bottomKeyboard.transform.localPosition = offset;

            ////////////////////////////////
            // Add a bit of extra spacing
            ////////////////////////////////
            bottomKeyboard.transform.localPosition += new Vector3(0, -0.1f, 0);

            /////////////////////
            // Rotate upwards
            /////////////////////
            Vector3 bottomAnchorLocal = this.transform.TransformPoint(new Vector3(0, -terminalCanvasRectHeightAdjusted * 0.5f, 0));
            bottomKeyboard.transform.RotateAround(bottomAnchorLocal, transform.right, 45);

            ////////////////////
            // Project forward
            ////////////////////
            bottomKeyboard.transform.localPosition += (-bottomKeyboard.transform.forward) * 0.05f;

            KeyboardLayout bottomKeyboardLayout = getBottomKeyboardLayout();

            bottomKeyboardCanvas.inflateLayout(this, bottomKeyboardLayout, m_keyboardStyle);

        }

        public void HandleInput( KeyDefinition key )
        {
            checkDirty();

            Debug.Log("Key press: " + key.GetName());

            if (key.hasInput())
            {
                currentFrameInput += key.Input;
            }
            else
            {
                switch(key.KeyType)
                {
                    case KeyType.BACKPSPACE:
                    {
                        SteamVR_InputHandler.triggerDummyAction(InputAction.TerminalDel);
                        break;
                    }

                    case KeyType.ESC:
                    {
                        SteamVR_InputHandler.triggerDummyAction(InputAction.TerminalExit);
                        break;
                    }
                }
            }
        }

        public void HandleInput(string str)
        {
            checkDirty();

            Debug.Log("String input: " + str);

            currentFrameInput += str;
            
        }

        private void checkDirty()
        {
            if (m_dataDirty)
            {
                currentFrameInput = "";
                m_dataDirty = false;
            }
        }

        // Update is called once per frame
        void Update()
        {
            checkDirty();
        }

        private void OnDestroy()
        {

        }
    }

}
