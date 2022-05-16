using Assets.scripts.KeyboardDefinition;
using GTFO_VR.Core.UI.canvas;
using GTFO_VR.Core.VR_Input;
using System.Collections;
using System.Collections.Generic;
using UnhollowerBaseLib.Attributes;
using UnityEngine;

namespace GTFO_VR.UI.CANVAS
{

    enum CanvasPosition
    {
        left, right, bottom
    }
    public class TerminalKeyboardInterface : MonoBehaviour
    {
        private LevelGeneration.LG_ComputerTerminal m_terminal;
        private GameObject m_terminalCanvas;
        public KeyboardStyle m_keyboardStyle = new KeyboardStyle(2, 1);


        // Workaround for spacing without there being hitbox gaps between keys
        public static readonly float KEY_SCALE = 0.96f;
        public static readonly float HITBOX_SCALE = 1.04f;


        public static readonly int LAYER = 5;
        public static readonly int LAYER_MASK = 1 << LAYER;
        public static readonly float CANVAS_SCALE = 0.045f; // Same scaling used by GTFO, because otherwise units are silly.
        private static readonly float SECTION_PADDING = 0f * CANVAS_SCALE;

        public static string currentFrameInput = "";

        bool m_dataDirty = false;


        public static GameObject create(LevelGeneration.LG_ComputerTerminal terminal)
        {
            GameObject go = new GameObject();
            go.name = "keyboardRoot";
            go.layer = LAYER;
            TerminalKeyboardInterface inf = go.AddComponent<TerminalKeyboardInterface>();
            inf.m_terminalCanvas = terminal.m_text.gameObject;
            inf.m_terminal = terminal;
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

        private void Start()
        {
            this.transform.position = m_terminalCanvas.transform.position;
            this.transform.rotation = m_terminalCanvas.transform.rotation;

            m_keyboardStyle.TileSize = 1;
            m_keyboardStyle.FontSize = 0.5f;
            m_keyboardStyle.SpacingHorizontal = 0.00f;
            m_keyboardStyle.SpacingVertical = 0.00f;
            m_keyboardStyle.keyPadding = 0.01f;

            RectTransform terminalCanvasRect = m_terminalCanvas.GetComponent<RectTransform>();

            /////////////////////////
            // terminal text canvas
            /////////////////////////

            GameObject terminalReaderRoot = TerminalReader.Create(m_terminalCanvas, this);
            terminalReaderRoot.transform.SetParent(this.transform);



            ///////////////////////
            // Bottom keyboard
            ///////////////////////

            {
                GameObject bottomKeyboard = new GameObject();
                bottomKeyboard.name = "bottomKeyboard";

                bottomKeyboard.transform.SetParent(this.gameObject.transform);

                float bottomKeyboardHeight = 14;
                float bottomKeyboardWidth = 16;

                generateCanvas(bottomKeyboard, terminalCanvasRect, bottomKeyboardHeight, bottomKeyboardWidth, TextAnchor.UpperCenter, getBottomKeyboardLayout(), m_keyboardStyle, CanvasPosition.bottom);
            }

            {
                GameObject leftKeyboard = new GameObject();
                leftKeyboard.name = "leftKeyboard";

                leftKeyboard.transform.SetParent(this.gameObject.transform);

                float bottomKeyboardHeight = terminalCanvasRect.sizeDelta.y;
                float bottomKeyboardWidth = 16;

                generateCanvas(leftKeyboard, terminalCanvasRect, bottomKeyboardHeight, bottomKeyboardWidth, TextAnchor.LowerRight, getLeftKeyboardLayout(), m_keyboardStyle, CanvasPosition.left);
            }

            {
                GameObject rightKeyboard = new GameObject();
                rightKeyboard.name = "rightKeyboard";

                rightKeyboard.transform.SetParent(this.gameObject.transform);

                float bottomKeyboardHeight = terminalCanvasRect.sizeDelta.y;
                float bottomKeyboardWidth = 16;

                generateCanvas(rightKeyboard, terminalCanvasRect, bottomKeyboardHeight, bottomKeyboardWidth, TextAnchor.LowerLeft, getRightKeyboard(getZone()), m_keyboardStyle, CanvasPosition.right);
            }

        }

        private string getZone()
        {
            // Do terminals outside have these values?
            if (m_terminal?.m_terminalItem?.FloorItemLocation != null)
            {
                return m_terminal.m_terminalItem.FloorItemLocation;
            }
            else
            {
                return "ZONE_0";
            }

           
        }

        private void generateCanvas(  GameObject go, RectTransform terminalCanvasRect, float rawHeight, float rawWidth, TextAnchor gravity, KeyboardLayout layout, KeyboardStyle style, CanvasPosition position)
        {
            TerminalKeyboardCanvas newKeyboardCanvas = TerminalKeyboardCanvas.attach(go, rawWidth, rawHeight, TextAnchor.MiddleCenter);

            float terminalHeight = terminalCanvasRect.rect.height * CANVAS_SCALE;
            float terminalWidth = terminalCanvasRect.rect.width * CANVAS_SCALE;

            float keyboardCanvasHeight = rawHeight * CANVAS_SCALE;
            float keyboardCanvasWidth = rawWidth * CANVAS_SCALE;

            //////////////////////////////////////////
            // Align with bottom of terminal canvas
            //////////////////////////////////////////

            switch (position)
            {
                case CanvasPosition.left:
                    float leftKeyboardOffset = terminalWidth / 2; // distance to edge of monitor
                    leftKeyboardOffset += keyboardCanvasWidth / 2;                // half of keyboard height so top aligns with bottom edge
                    go.transform.localPosition = new Vector3(-leftKeyboardOffset, 0 , 0); ;
                    break;

                case CanvasPosition.right:
                    float RightKeyboardOffset = terminalWidth / 2; // distance to edge of monitor
                    RightKeyboardOffset += keyboardCanvasWidth / 2;                // half of keyboard height so top aligns with bottom edge
                    go.transform.localPosition = new Vector3(RightKeyboardOffset, 0, 0); ;
                    break;

                case CanvasPosition.bottom:
                    float bottomKeyboardOffset = terminalHeight / 2; // distance to edge of monitor
                    bottomKeyboardOffset += keyboardCanvasHeight / 2;                // half of keyboard height so top aligns with bottom edge
                    go.transform.localPosition = new Vector3(0, -bottomKeyboardOffset, 0); ;
                    break;
            }



            ////////////////////////////////
            // Add a bit of extra spacing
            ////////////////////////////////

            float horizontalSpacing = 0.01f;
            float verticalSpacing = 0.1f;

            switch (position)
            {
                case CanvasPosition.left:
                    go.transform.localPosition += new Vector3(-horizontalSpacing, 0, 0);
                    break;

                case CanvasPosition.right:
                    go.transform.localPosition += new Vector3(horizontalSpacing, 0, 0);
                    break;

                case CanvasPosition.bottom:
                    go.transform.localPosition += new Vector3(0, -verticalSpacing, 0);
                    break;
            }




            /////////////////////
            // Rotate upwards
            /////////////////////

            switch (position)
            {
                case CanvasPosition.left:
                    Vector3 leftAnchorLocal = this.transform.TransformPoint(new Vector3(-terminalWidth * 0.5f, 0, 0));
                    go.transform.RotateAround(leftAnchorLocal, transform.up, -45);
                    break;

                case CanvasPosition.right:
                    Vector3 rightAnchorLocal = this.transform.TransformPoint(new Vector3(terminalWidth * 0.5f, 0, 0));
                    go.transform.RotateAround(rightAnchorLocal, transform.up, 45);
                    break;

                case CanvasPosition.bottom:
                    Vector3 bottomAnchorLocal = this.transform.TransformPoint(new Vector3(0, -terminalHeight * 0.5f, 0));
                    go.transform.RotateAround(bottomAnchorLocal, transform.right, 45);
                    break;
            }


            
            ////////////////////
            // Project forward
            ////////////////////
            
            switch (position)
            {
                case CanvasPosition.left:
                    break;

                case CanvasPosition.right:
                    break;

                case CanvasPosition.bottom:
                    go.transform.localPosition += (-go.transform.forward) * 0.05f;
                    break;
            } 

            newKeyboardCanvas.inflateLayout(this, layout, style);
        }

        public void HandleInput( KeyDefinition key )
        {
            checkDirty();

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

                    case KeyType.LEFT:
                    {
                        SteamVR_InputHandler.triggerDummyAction(InputAction.TerminalLeft);
                        break;
                    }

                    case KeyType.UP:
                    {
                        SteamVR_InputHandler.triggerDummyAction(InputAction.TerminalUp);
                        break;
                    }

                    case KeyType.RIGHT:
                    {
                        SteamVR_InputHandler.triggerDummyAction(InputAction.TerminalRight);
                        break;
                    }

                    case KeyType.DOWN:
                    {
                        SteamVR_InputHandler.triggerDummyAction(InputAction.TerminalDown);
                        break;
                    }
                }
            }
        }

        public void HandleInput(string str)
        {
            checkDirty();
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

        [HideFromIl2Cpp]
        private static KeyboardLayout getRightKeyboard(string zoneName)
        {
            LinearLayout bottomKeyboardLayout = new LinearLayout(LinearOrientation.VERTICAL, TextAnchor.LowerLeft);

            KeyboardLayoutParameters rowParams = new KeyboardLayoutParameters(15, 1, false, false);

            {
                LinearLayout keyboardRow = new LinearLayout(LinearOrientation.HORIZONTAL, TextAnchor.LowerLeft, rowParams);
                keyboardRow.AddChild(new KeyDefinition("TERMINAL ", "TERMINAL", 4));
                bottomKeyboardLayout.AddChild(keyboardRow);
            }

            {
                LinearLayout keyboardRow = new LinearLayout(LinearOrientation.HORIZONTAL, TextAnchor.LowerLeft, rowParams);
                keyboardRow.AddChild(new KeyDefinition("BULKHEAD ", "BULKHEAD", 4));
                bottomKeyboardLayout.AddChild(keyboardRow);
            }

            {
                LinearLayout keyboardRow = new LinearLayout(LinearOrientation.HORIZONTAL, TextAnchor.LowerLeft, rowParams);
                keyboardRow.AddChild(new KeyDefinition("FOG ", "FOG", 2));
                keyboardRow.AddChild(new KeyDefinition("GENERATOR ", "GEN", 2));
                bottomKeyboardLayout.AddChild(keyboardRow);
            }

            {
                LinearLayout keyboardRow = new LinearLayout(LinearOrientation.HORIZONTAL, TextAnchor.LowerLeft, rowParams);
                keyboardRow.AddChild(new KeyDefinition("POWER_CELL ", "CELL", 2));
                keyboardRow.AddChild(new KeyDefinition("KEY ", "KEY", 2));
                bottomKeyboardLayout.AddChild(keyboardRow);
            }

            {
                LinearLayout keyboardRow = new LinearLayout(LinearOrientation.HORIZONTAL, TextAnchor.LowerLeft, rowParams);
                keyboardRow.AddChild(new KeyDefinition("DISINFECT ", "DISIN", 2));
                keyboardRow.AddChild(new KeyDefinition("MEDI ", "MEDI", 2));
                bottomKeyboardLayout.AddChild(keyboardRow);
            }

            {
                LinearLayout keyboardRow = new LinearLayout(LinearOrientation.HORIZONTAL, TextAnchor.LowerLeft, rowParams);
                keyboardRow.AddChild(new KeyDefinition("AMMO ", "AMMO ", 2));
                keyboardRow.AddChild(new KeyDefinition("TOOL ", "TOOL ", 2));
                bottomKeyboardLayout.AddChild(keyboardRow);
            }

            {
                LinearLayout keyboardRow = new LinearLayout(LinearOrientation.HORIZONTAL, TextAnchor.LowerLeft, rowParams);
                keyboardRow.AddChild(new KeyDefinition( zoneName + " ", zoneName, 4));
                bottomKeyboardLayout.AddChild(keyboardRow);
            }

            {
                LinearLayout keyboardRow = new LinearLayout(LinearOrientation.HORIZONTAL, TextAnchor.LowerLeft, rowParams);
                keyboardRow.AddChild(new KeyDefinition("RESOURCE ", "RESOURCE", 4));
                bottomKeyboardLayout.AddChild(keyboardRow);
            }

            return bottomKeyboardLayout;
        }

        [HideFromIl2Cpp]
        private static KeyboardLayout getLeftKeyboardLayout()
        {
            LinearLayout bottomKeyboardLayout = new LinearLayout(LinearOrientation.VERTICAL, TextAnchor.LowerRight);

            KeyboardLayoutParameters rowParams = new KeyboardLayoutParameters(15, 1, false, false);

            {
                LinearLayout keyboardRow = new LinearLayout(LinearOrientation.HORIZONTAL, TextAnchor.LowerRight, rowParams);
                keyboardRow.AddChild(new KeyDefinition("REACTOR_VERIFY ", "REACTOR_VERIFY", 5));
                bottomKeyboardLayout.AddChild(keyboardRow);
            }

            {
                LinearLayout keyboardRow = new LinearLayout(LinearOrientation.HORIZONTAL, TextAnchor.LowerRight, rowParams);
                keyboardRow.AddChild(new KeyDefinition("UPLINK_VERIFY ", "UPLINK_VERIFY", 5));
                bottomKeyboardLayout.AddChild(keyboardRow);
            }

            {
                LinearLayout keyboardRow = new LinearLayout(LinearOrientation.HORIZONTAL, TextAnchor.LowerRight, rowParams);
                keyboardRow.AddChild(new KeyDefinition("UPLINK_CONNECT ", "UPLINK_CONNECT", 5));
                bottomKeyboardLayout.AddChild(keyboardRow);
            }

            {
                LinearLayout keyboardRow = new LinearLayout(LinearOrientation.HORIZONTAL, TextAnchor.LowerRight, rowParams);
                keyboardRow.AddChild(new KeyDefinition("ZONE_", "ZONE_", 3));
                keyboardRow.AddChild(new KeyDefinition("-T", "-T", 1));
                bottomKeyboardLayout.AddChild(keyboardRow);
            }

            {
                LinearLayout keyboardRow = new LinearLayout(LinearOrientation.HORIZONTAL, TextAnchor.LowerRight, rowParams);
                keyboardRow.AddChild(new KeyDefinition("LOGS\r", "LOGS", 2));
                keyboardRow.AddChild(new KeyDefinition("READ ", "READ", 2));
                bottomKeyboardLayout.AddChild(keyboardRow);
            }

            {
                LinearLayout keyboardRow = new LinearLayout(LinearOrientation.HORIZONTAL, TextAnchor.LowerRight, rowParams);
                keyboardRow.AddChild(new KeyDefinition("INFO\r", "INFO", 2));
                keyboardRow.AddChild(new KeyDefinition("LIST ", "LIST", 2));
                bottomKeyboardLayout.AddChild(keyboardRow);
            }

            {
                LinearLayout keyboardRow = new LinearLayout(LinearOrientation.HORIZONTAL, TextAnchor.LowerRight, rowParams);
                keyboardRow.AddChild(new KeyDefinition("QUERY ", "QUERY", 2));
                keyboardRow.AddChild(new KeyDefinition("PING ", "PING", 2));
                bottomKeyboardLayout.AddChild(keyboardRow);
            }

            {
                LinearLayout keyboardRow = new LinearLayout(LinearOrientation.HORIZONTAL, TextAnchor.LowerRight, rowParams);
                keyboardRow.AddChild(new KeyDefinition("COMMANDS\r", "COMMANDS", 4));
                bottomKeyboardLayout.AddChild(keyboardRow);
            }

            return bottomKeyboardLayout;
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
                LinearLayout shortRowHorizontal = new LinearLayout(LinearOrientation.HORIZONTAL, TextAnchor.UpperCenter, new KeyboardLayoutParameters(15, 3, false, false));
                LinearLayout shortRowVertical = new LinearLayout(LinearOrientation.VERTICAL, TextAnchor.UpperLeft, new KeyboardLayoutParameters(12.5f, 3, false, false));
                KeyboardLayoutParameters shortRowParams = new KeyboardLayoutParameters(12.5f, 1, false, false);

                {
                    LinearLayout keyboardRow = new LinearLayout(LinearOrientation.HORIZONTAL, TextAnchor.UpperLeft, shortRowParams);

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
                    //keyboardRow.AddChild(new KeyDefinition(KeyType.EMPTY, "", new KeyboardLayoutParameters(1f, true)));

                    shortRowVertical.AddChild(keyboardRow);
                }


                {
                    LinearLayout keyboardRow = new LinearLayout(LinearOrientation.HORIZONTAL, TextAnchor.UpperLeft, shortRowParams);

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
                    keyboardRow.AddChild(new KeyDefinition(KeyType.ENTER, "", new KeyboardLayoutParameters(0.01f, true)));

                    shortRowVertical.AddChild(keyboardRow);
                }


                {
                    LinearLayout keyboardRow = new LinearLayout(LinearOrientation.HORIZONTAL, TextAnchor.UpperLeft, rowParams);

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
                    keyboardRow.AddChild(new KeyDefinition(KeyType.UP, "^", 1.1f));
                    //keyboardRow.AddChild(new KeyDefinition(KeyType.EMPTY, "", new KeyboardLayoutParameters(1f, true)));

                    shortRowVertical.AddChild(keyboardRow);
                }

                shortRowHorizontal.AddChild(shortRowVertical);
                shortRowHorizontal.AddChild(new KeyDefinition(KeyType.ENTER, "enter", new KeyboardLayoutParameters(1f, 3f, true, false)));
                bottomKeyboardLayout.AddChild(shortRowHorizontal);
            }

            {
                LinearLayout keyboardRow = new LinearLayout(LinearOrientation.HORIZONTAL, TextAnchor.UpperCenter, rowParams);

                keyboardRow.AddChild(new KeyDefinition(KeyType.EMPTY, "", 3.2f));
                keyboardRow.AddChild(new KeyDefinition(KeyType.SPACE, "space", 7.2f));
                keyboardRow.AddChild(new KeyDefinition(KeyType.LEFT, "<"));
                keyboardRow.AddChild(new KeyDefinition(KeyType.DOWN, "v", 1.1f));
                keyboardRow.AddChild(new KeyDefinition(KeyType.RIGHT, ">"));
                keyboardRow.AddChild(new KeyDefinition(KeyType.EMPTY, "", new KeyboardLayoutParameters(1f, true)));

                bottomKeyboardLayout.AddChild(keyboardRow);
            }

            return bottomKeyboardLayout;
        }

        private void OnDestroy()
        {
            // Let's not leak materials
            m_keyboardStyle.cleanup();

        }

    }


}
