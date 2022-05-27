﻿using GTFO_VR.Core.UI.Terminal.KeyboardDefinition;
using GTFO_VR.Core.VR_Input;
using System;
using UnhollowerBaseLib.Attributes;
using UnityEngine;

namespace GTFO_VR.Core.UI.Terminal
{

    enum CanvasPosition
    {
        left, right, bottom
    }
    public class TerminalKeyboardInterface : MonoBehaviour
    {
        private LevelGeneration.LG_ComputerTerminal m_terminal;
        private KeyboardStyle m_keyboardStyle = new KeyboardStyle();

        private GameObject m_leftKeyboard;
        private GameObject m_rightKeyboard;
        private GameObject m_bottomKeyboard;

        private KeyDefinition m_ZoneButton;

        private TerminalReader m_Reader;

        public static readonly int LAYER = 2;   // ignore raycast, by defaul at least.
        public static readonly int LAYER_MASK = 1 << LAYER;
        public static readonly float CANVAS_SCALE = 0.045f; // Same scaling used by GTFO, because otherwise units are silly.

        public static string currentFrameInput = "";

        bool m_dataDirty = false;

        public static TerminalKeyboardInterface create()
        {
            GameObject go = new GameObject();
            go.name = "keyboardRoot";
            go.layer = LAYER;
            TerminalKeyboardInterface inf = go.AddComponent<TerminalKeyboardInterface>();
            go.SetActive(false); // let Awake() run then deactivate until needed.
            return inf;
        }

        private void Awake()
        {
            generateKeyboads();

            m_Reader = TerminalReader.Create(this);
            m_Reader.transform.SetParent(this.transform);
        }

        [HideFromIl2Cpp]
        public KeyboardStyle getStyle()
        {
            return m_keyboardStyle;
        }


        [HideFromIl2Cpp]
        public void attachToTerminal(LevelGeneration.LG_ComputerTerminal terminal )
        {
            m_terminal = terminal;

            GameObject terminalCanvas = m_terminal.m_text.gameObject;
            RectTransform terminalRect = terminalCanvas.GetComponent<RectTransform>();

            this.transform.SetPositionAndRotation(terminalCanvas.transform.position, terminalCanvas.transform.rotation); 

            positionKeyboard(m_leftKeyboard, terminalRect, CanvasPosition.left);
            positionKeyboard(m_rightKeyboard, terminalRect, CanvasPosition.right);
            positionKeyboard(m_bottomKeyboard, terminalRect, CanvasPosition.bottom);

            if (m_ZoneButton != null)
            {
                String zone = getZone();
                m_ZoneButton.updateContent(zone + " ", zone);
            }

            m_Reader.attachToTerminal(terminalCanvas);

            this.gameObject.SetActive(true);
        }

        public void deatchFromTerminal()
        {
            if (m_terminal != null)
            {
                m_terminal = null;
            }

            if (m_Reader != null)
            {
                m_Reader.deatchFromTerminal();

            }

            this.gameObject.SetActive(false);
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

        private void generateKeyboads()
        {
            m_bottomKeyboard = new GameObject();
            m_bottomKeyboard.name = "bottomKeyboard";
            m_bottomKeyboard.transform.SetParent(this.gameObject.transform);
            generateKeyboard(m_bottomKeyboard, 14, 16, TextAnchor.UpperCenter, getBottomKeyboardLayout(), m_keyboardStyle);

            m_leftKeyboard = new GameObject();
            m_leftKeyboard.name = "leftKeyboard";
            m_leftKeyboard.transform.SetParent(this.gameObject.transform);
            generateKeyboard(m_leftKeyboard, 14, 16, TextAnchor.LowerRight, getLeftKeyboardLayout(), m_keyboardStyle);

            m_rightKeyboard = new GameObject();
            m_rightKeyboard.name = "rightKeyboard";
            m_rightKeyboard.transform.SetParent(this.gameObject.transform);
            generateKeyboard(m_rightKeyboard, 14, 16, TextAnchor.LowerLeft, getRightKeyboard(out m_ZoneButton), m_keyboardStyle);
            
        }

        [HideFromIl2Cpp]
        private void generateKeyboard(GameObject go, float rawHeight, float rawWidth, TextAnchor gravity, KeyboardLayout layout, KeyboardStyle style)
        {
            TerminalKeyboardCanvas newKeyboardCanvas = TerminalKeyboardCanvas.attach(go, rawWidth, rawHeight, gravity);
            newKeyboardCanvas.inflateLayout(this, layout, style);
        }

        [HideFromIl2Cpp]
        private void positionKeyboard( GameObject go, RectTransform terminalCanvasRect, CanvasPosition position)
        {
            float terminalHeight = terminalCanvasRect.rect.height * CANVAS_SCALE;
            float terminalWidth = terminalCanvasRect.rect.width * CANVAS_SCALE;

            RectTransform keyboardCanvasRect = go.GetComponent<RectTransform>();

            float keyboardCanvasHeight = keyboardCanvasRect.sizeDelta.y * CANVAS_SCALE;
            float keyboardCanvasWidth = keyboardCanvasRect.sizeDelta.x * CANVAS_SCALE;

            // Reset rotation
            go.transform.localRotation = new Quaternion();

            //////////////////////////////////////////
            // Align with edge of terminal canvas
            //////////////////////////////////////////

            switch (position)
            {
                case CanvasPosition.left:
                    float leftKeyboardOffset = terminalWidth / 2;                                       // distance to edge of monitor
                    leftKeyboardOffset += keyboardCanvasWidth / 2;                                      // half of keyboard width so side aligns with side
                    float leftKeyboardHeightOffset = (keyboardCanvasHeight - terminalHeight) * 0.5f;    // Half of height difference to line up bottom edge
                    go.transform.localPosition = new Vector3(-leftKeyboardOffset, leftKeyboardHeightOffset, 0); ;
                    break;

                case CanvasPosition.right:
                    float rightKeyboardOffset = terminalWidth / 2;                                      // distance to edge of monitor
                    rightKeyboardOffset += keyboardCanvasWidth / 2;                                     // half of keyboard width so side aligns with side
                    float rightKeyboardHeightOffset = (keyboardCanvasHeight - terminalHeight) * 0.5f;   // Half of height difference to line up bottom edge
                    go.transform.localPosition = new Vector3(rightKeyboardOffset, rightKeyboardHeightOffset, 0); ;
                    break;

                case CanvasPosition.bottom:
                    float bottomKeyboardOffset = terminalHeight / 2;                // distance to edge of monitor
                    bottomKeyboardOffset += keyboardCanvasHeight / 2;               // half of keyboard height so top aligns with bottom edge
                    go.transform.localPosition = new Vector3(0, -bottomKeyboardOffset, 0); ;
                    break;
            }


            
            ////////////////////////////////
            // Add a bit of extra spacing
            ////////////////////////////////

            float horizontalSpacing = 0.05f;
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
            
        }

        [HideFromIl2Cpp]
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
                        Dummy_InputHandler.triggerDummyAction(InputAction.TerminalDel);
                        break;
                    }

                    case KeyType.ESC:
                    {
                        Dummy_InputHandler.triggerDummyAction(InputAction.TerminalExit);
                        break;
                    }

                    case KeyType.LEFT:
                    {
                        Dummy_InputHandler.triggerDummyAction(InputAction.TerminalLeft);
                        break;
                    }

                    case KeyType.UP:
                    {
                        Dummy_InputHandler.triggerDummyAction(InputAction.TerminalUp);
                        break;
                    }

                    case KeyType.RIGHT:
                    {
                        Dummy_InputHandler.triggerDummyAction(InputAction.TerminalRight);
                        break;
                    }

                    case KeyType.DOWN:
                    {
                        Dummy_InputHandler.triggerDummyAction(InputAction.TerminalDown);
                        break;
                    }
                }
            }
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
        private static KeyboardLayout getRightKeyboard( out KeyDefinition zoneButton )
        {
            LinearLayout bottomKeyboardLayout = new LinearLayout(LinearOrientation.VERTICAL, TextAnchor.LowerLeft, LayoutParameters.WrapContent());
            bottomKeyboardLayout.m_showBackground = true;

            LayoutParameters rowParams = new LayoutParameters( LayoutParameters.WRAP_CONTENT, 1);

            bottomKeyboardLayout.AddChild(new KeyDefinition("TERMINAL_ ", "TERMINAL", 4).setApperance(KeyApperanceType.ALT));
            
            bottomKeyboardLayout.AddChild(new KeyDefinition("BULKHEAD ", "BULKHEAD", 4).setApperance(KeyApperanceType.ALT));

            bottomKeyboardLayout.AddChild(new KeyDefinition("SECURITY ", "SECURITY", 4).setApperance(KeyApperanceType.ALT));

            bottomKeyboardLayout.AddChild(new KeyDefinition("UNKNOWN ", "UNKNOWN", 4).setApperance(KeyApperanceType.ALT));

            {
                LinearLayout keyboardRow = new LinearLayout(LinearOrientation.HORIZONTAL, TextAnchor.LowerLeft, rowParams);
                keyboardRow.AddChild(new KeyDefinition("AMMO ", "AMMO ", 2).setApperance(KeyApperanceType.ALT));
                keyboardRow.AddChild(new KeyDefinition("TOOL ", "TOOL ", 2).setApperance(KeyApperanceType.ALT));
                bottomKeyboardLayout.AddChild(keyboardRow);
            }

            {
                LinearLayout keyboardRow = new LinearLayout(LinearOrientation.HORIZONTAL, TextAnchor.LowerLeft, rowParams);
                keyboardRow.AddChild(new KeyDefinition("MEDI ", "MEDI", 2).setApperance(KeyApperanceType.ALT));
                keyboardRow.AddChild(new KeyDefinition("DISINFECT ", "DISIN", 2).setApperance(KeyApperanceType.ALT));
                bottomKeyboardLayout.AddChild(keyboardRow);
            }

            bottomKeyboardLayout.AddChild(new KeyDefinition("RESOURCES ", "RESOURCES", 4).setApperance(KeyApperanceType.ALT));

            {
                // This will be updated when assigned to a terminal
                zoneButton = new KeyDefinition("ZONE_-1 ", "ZONE_-1", 4).setApperance(KeyApperanceType.ALT);
                bottomKeyboardLayout.AddChild(zoneButton);
            }

            return bottomKeyboardLayout;
        }

        [HideFromIl2Cpp]
        private static KeyboardLayout getLeftKeyboardLayout()
        {
            LinearLayout bottomKeyboardLayout = new LinearLayout(LinearOrientation.VERTICAL, TextAnchor.LowerRight, LayoutParameters.WrapContent());
            bottomKeyboardLayout.m_showBackground = true;

            LayoutParameters rowParams = new LayoutParameters(LayoutParameters.WRAP_CONTENT, 1);
 
            bottomKeyboardLayout.AddChild(new KeyDefinition("UPLINK_VERIFY ", "UPLINK_VERIFY", 5).setApperance(KeyApperanceType.ALT));

            bottomKeyboardLayout.AddChild(new KeyDefinition("UPLINK_CONNECT ", "UPLINK_CONNECT", 5).setApperance(KeyApperanceType.ALT));
            
            bottomKeyboardLayout.AddChild(new KeyDefinition("REACTOR_VERIFY ", "REACTOR_VERIFY", 5).setApperance(KeyApperanceType.ALT));

            {
                LinearLayout keyboardRow = new LinearLayout(LinearOrientation.HORIZONTAL, TextAnchor.LowerRight, rowParams);
                keyboardRow.AddChild(new KeyDefinition("ZONE_", "ZONE_", 3.5f).setApperance(KeyApperanceType.ALT));
                keyboardRow.AddChild(new KeyDefinition("-T ", "-T", 1.5f).setApperance(KeyApperanceType.ALT));
                bottomKeyboardLayout.AddChild(keyboardRow);
            }

            {
                LinearLayout keyboardRow = new LinearLayout(LinearOrientation.HORIZONTAL, TextAnchor.LowerRight, rowParams);
                keyboardRow.AddChild(new KeyDefinition("LOGS\r", "LOGS", 2.5f).setApperance(KeyApperanceType.ALT));
                keyboardRow.AddChild(new KeyDefinition("LIST ", "LIST", 2.5f).setApperance(KeyApperanceType.ALT));
                bottomKeyboardLayout.AddChild(keyboardRow);
            }

            {
                LinearLayout keyboardRow = new LinearLayout(LinearOrientation.HORIZONTAL, TextAnchor.LowerRight, rowParams);
                keyboardRow.AddChild(new KeyDefinition("READ ", "READ", 2.5f).setApperance(KeyApperanceType.ALT));
                keyboardRow.AddChild(new KeyDefinition("PING ", "PING", 2.5f).setApperance(KeyApperanceType.ALT));
                bottomKeyboardLayout.AddChild(keyboardRow);
            }

            {
                LinearLayout keyboardRow = new LinearLayout(LinearOrientation.HORIZONTAL, TextAnchor.LowerRight, rowParams);
                keyboardRow.AddChild(new KeyDefinition("INFO\r", "INFO", 2.5f).setApperance(KeyApperanceType.ALT));
                keyboardRow.AddChild(new KeyDefinition("QUERY ", "QUERY", 2.5f).setApperance(KeyApperanceType.ALT));
                bottomKeyboardLayout.AddChild(keyboardRow);
            }

            {
                bottomKeyboardLayout.AddChild(new KeyDefinition("COMMANDS\r", "COMMANDS", 5f).setApperance(KeyApperanceType.ALT));
            }

            return bottomKeyboardLayout;
        }

        [HideFromIl2Cpp]
        private static KeyboardLayout getBottomKeyboardLayout()
        {
            LinearLayout bottomKeyboardLayout = new LinearLayout(LinearOrientation.VERTICAL, TextAnchor.UpperCenter, LayoutParameters.WrapContent() );
            bottomKeyboardLayout.m_showBackground = true;

            LayoutParameters rowParams = new LayoutParameters(15, 1);

            {
                LinearLayout keyboardRow = new LinearLayout(LinearOrientation.HORIZONTAL, TextAnchor.UpperCenter, rowParams);

                keyboardRow.AddChild(new KeyDefinition(KeyType.ESC, "x")
                    .setApperance(KeyApperanceType.EXIT));
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
                keyboardRow.AddChild(new KeyDefinition(KeyType.BACKPSPACE, "Backspace", new LayoutParameters( LayoutParameters.FILL_PARENT ))
                    .setRepeatKey(true)
                    .setApperance(KeyApperanceType.ALT));

                bottomKeyboardLayout.AddChild(keyboardRow);
            }

            {
                LinearLayout shortRowHorizontal = new LinearLayout(LinearOrientation.HORIZONTAL, TextAnchor.UpperCenter, new LayoutParameters(15, 3));
                LinearLayout shortRowVertical = new LinearLayout(LinearOrientation.VERTICAL, TextAnchor.UpperLeft, new LayoutParameters(12.5f, 3));
                LayoutParameters shortRowParams = new LayoutParameters(12.5f, 1);

                {
                    LinearLayout keyboardRow = new LinearLayout(LinearOrientation.HORIZONTAL, TextAnchor.UpperLeft, shortRowParams);

                    keyboardRow.AddChild(new KeyDefinition(KeyType.TAB, "Tab", 1.5f)
                        .setApperance(KeyApperanceType.ALT));
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
                    keyboardRow.AddChild(new KeyDefinition("-"));

                    shortRowVertical.AddChild(keyboardRow);
                }


                {
                    LinearLayout keyboardRow = new LinearLayout(LinearOrientation.HORIZONTAL, TextAnchor.UpperLeft, shortRowParams);

                    keyboardRow.AddChild(new KeyDefinition(KeyType.CAPS_LOCK, "", 1.5f)
                        .setApperance(KeyApperanceType.ALT));
                    keyboardRow.AddChild(new KeyDefinition(KeyType.EMPTY, "", 0.35f)
                    .setApperance(KeyApperanceType.GONE));
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
                    keyboardRow.AddChild(new KeyDefinition(KeyType.ENTER, "", new LayoutParameters(LayoutParameters.FILL_PARENT, 1, 0.01f))
                        .setApperance(KeyApperanceType.GONE));

                    shortRowVertical.AddChild(keyboardRow);
                }


                {
                    LinearLayout keyboardRow = new LinearLayout(LinearOrientation.HORIZONTAL, TextAnchor.UpperLeft, rowParams);

                    keyboardRow.AddChild(new KeyDefinition(KeyType.SHIFT, "", 2.4f)
                        .setApperance(KeyApperanceType.ALT));
                    keyboardRow.AddChild(new KeyDefinition("z"));
                    keyboardRow.AddChild(new KeyDefinition("x"));
                    keyboardRow.AddChild(new KeyDefinition("c"));
                    keyboardRow.AddChild(new KeyDefinition("v"));
                    keyboardRow.AddChild(new KeyDefinition("b"));
                    keyboardRow.AddChild(new KeyDefinition("n"));
                    keyboardRow.AddChild(new KeyDefinition("m"));
                    keyboardRow.AddChild(new KeyDefinition(","));
                    keyboardRow.AddChild(new KeyDefinition("."));
                    keyboardRow.AddChild(new KeyDefinition(KeyType.UP, "^", 1.1f)
                        .setApperance(KeyApperanceType.ALT));
                    //keyboardRow.AddChild(new KeyDefinition(KeyType.EMPTY, "", new KeyboardLayoutParameters(1f, true)));

                    shortRowVertical.AddChild(keyboardRow);
                }

                shortRowHorizontal.AddChild(shortRowVertical);
                shortRowHorizontal.AddChild(new KeyDefinition(KeyType.ENTER, "Enter", new LayoutParameters(LayoutParameters.FILL_PARENT, 3f))
                    .setApperance(KeyApperanceType.ALT));
                bottomKeyboardLayout.AddChild(shortRowHorizontal);
            }

            {
                LinearLayout keyboardRow = new LinearLayout(LinearOrientation.HORIZONTAL, TextAnchor.UpperCenter, rowParams);

                keyboardRow.AddChild(new KeyDefinition(KeyType.EMPTY, "", 3.2f)
                    .setApperance(KeyApperanceType.ALT));
                keyboardRow.AddChild(new KeyDefinition(KeyType.SPACE, "Space", 7.2f));
                keyboardRow.AddChild(new KeyDefinition(KeyType.LEFT, "<")
                    .setApperance(KeyApperanceType.ALT));
                keyboardRow.AddChild(new KeyDefinition(KeyType.DOWN, "v", 1.1f)
                    .setApperance(KeyApperanceType.ALT));
                keyboardRow.AddChild(new KeyDefinition(KeyType.RIGHT, ">")
                    .setApperance(KeyApperanceType.ALT));
                keyboardRow.AddChild(new KeyDefinition(KeyType.ESC, "x", new LayoutParameters(LayoutParameters.FILL_PARENT))
                    .setApperance(KeyApperanceType.EXIT));

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