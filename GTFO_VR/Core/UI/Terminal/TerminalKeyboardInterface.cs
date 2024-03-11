using GTFO_VR.Core.UI.Terminal.KeyboardDefinition;
using GTFO_VR.Core.VR_Input;
using Il2CppInterop.Runtime.Attributes;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace GTFO_VR.Core.UI.Terminal
{

    enum CanvasPosition
    {
        left, right, bottom
    }

    /// <summary>
    /// Root of the terminal keyboard. Handles creating and positioning the layout, as well as interacting with the terminal.
    /// </summary>
    public class TerminalKeyboardInterface : MonoBehaviour
    {
        public TerminalKeyboardInterface(IntPtr value) : base(value) { }

        private LevelGeneration.LG_ComputerTerminal m_terminal;

        private GameObject m_leftKeyboard;
        private GameObject m_rightKeyboard;
        private GameObject m_bottomKeyboard;

        private KeyDefinition m_ZoneButton;
        private TerminalReader m_Reader;

        private KeyboardStyle m_keyboardStyle = new KeyboardStyle();

        public static readonly int LAYER = 2;   // ignore raycast, by defaul at least.
        public static readonly int LAYER_MASK = 1 << LAYER;
        public static readonly float CANVAS_SCALE = 0.045f; // Same scaling used by GTFO, because otherwise units are silly.

        private static string m_currentFrameInput = "";
        private static string m_prevFrameInput = "";

        private static HashSet<KeyCode> m_currentFrameKeycode = new HashSet<KeyCode>();
        private static HashSet<KeyCode> m_prevFrameKeycode = new HashSet<KeyCode>();

        private static HashSet<KeyCode> m_activeModifiers = new HashSet<KeyCode>();

        public static TerminalKeyboardInterface create()
        {
            GameObject go = new GameObject();
            go.name = "KeyboardRoot";
            go.layer = LAYER;
            TerminalKeyboardInterface inf = go.AddComponent<TerminalKeyboardInterface>();
            go.SetActive(false); // let Awake() run then deactivate until needed.
            return inf;
        }

        private void Awake()
        {
            GenerateKeyboards();

            m_Reader = TerminalReader.Instantiate(this);
            m_Reader.transform.SetParent(this.transform);
        }

        [HideFromIl2Cpp]
        public KeyboardStyle getStyle()
        {
            return m_keyboardStyle;
        }

        /// <summary>
        /// Attach the already instantiated terminal keyboard to a terminal
        /// </summary>
        [HideFromIl2Cpp]
        public void AttachToTerminal(LevelGeneration.LG_ComputerTerminal terminal )
        {
            m_terminal = terminal;

            GameObject terminalCanvas = m_terminal.m_text.gameObject;
            RectTransform terminalRect = terminalCanvas.GetComponent<RectTransform>();

            this.transform.SetPositionAndRotation(terminalCanvas.transform.position, terminalCanvas.transform.rotation); 

            PositionKeyboard(m_leftKeyboard, terminalRect, CanvasPosition.left);
            PositionKeyboard(m_rightKeyboard, terminalRect, CanvasPosition.right);
            PositionKeyboard(m_bottomKeyboard, terminalRect, CanvasPosition.bottom);

            if (m_ZoneButton != null)
            {
                String zone = GetZone();
                m_ZoneButton.UpdateContent(zone + " ", zone);
            }

            m_Reader.AttachToTerminal(terminalCanvas);

            this.gameObject.SetActive(true);
        }

        public bool IsAttachedToTerminal()
        {
            return m_terminal != null;
        }

        public void DetatchFromTerminal()
        {
            m_terminal = null;
            m_activeModifiers.Clear();

            if (m_Reader != null)
            {
                m_Reader.DetatchFromTerminal();

            }

            this.gameObject.SetActive(false);
        }

        /// <summary>
        /// Get the zone of the terminal this keyboard is attached to.
        /// </summary>
        private string GetZone()
        {
            // Do terminals outside have these values? ( Yes: Zero_0 it turns out )
            if (m_terminal?.m_terminalItem?.FloorItemLocation != null)
            {
                string zone = m_terminal.m_terminalItem.FloorItemLocation;
                // As of R8, the zone provided here will often include a description.
                // There is always a comma followed by a space before the regular ZONEXXX
                int lastSpace = zone.LastIndexOf(" ");
                if (lastSpace > 0 && lastSpace + 1 < zone.Length )  // found and not final string char
                {
                    return zone.Substring(lastSpace + 1);
                }

                return zone;
            }
            else
            {
                return "ZONE_0";
            }
        }

        /// <summary>
        /// Generate the keyboard layouts. This happens only once, long before the keyboard is attached to an actual terminal.
        /// </summary>
        private void GenerateKeyboards()
        {
            m_bottomKeyboard = new GameObject();
            m_bottomKeyboard.name = "bottomKeyboard";
            m_bottomKeyboard.transform.SetParent(this.gameObject.transform);
            GenerateKeyboard(m_bottomKeyboard, 14, 16, TextAnchor.UpperCenter, GetBottomKeyboardLayout(), m_keyboardStyle);

            m_leftKeyboard = new GameObject();
            m_leftKeyboard.name = "leftKeyboard";
            m_leftKeyboard.transform.SetParent(this.gameObject.transform);
            GenerateKeyboard(m_leftKeyboard, 14, 16, TextAnchor.LowerRight, GetLeftKeyboardLayout(), m_keyboardStyle);

            m_rightKeyboard = new GameObject();
            m_rightKeyboard.name = "rightKeyboard";
            m_rightKeyboard.transform.SetParent(this.gameObject.transform);
            GenerateKeyboard(m_rightKeyboard, 14, 16, TextAnchor.LowerLeft, GetRightKeyboardLayout(out m_ZoneButton), m_keyboardStyle);
            
        }

        [HideFromIl2Cpp]
        private void GenerateKeyboard(GameObject go, float rawHeight, float rawWidth, TextAnchor gravity, KeyboardLayout layout, KeyboardStyle style)
        {
            TerminalKeyboardCanvas newKeyboardCanvas = TerminalKeyboardCanvas.Instantiate(go, rawWidth, rawHeight, gravity);
            newKeyboardCanvas.InflateLayout(this, layout, style);
        }

        /// <summary>
        /// Position the pre-generated keyboard layout to align with the attached terminal
        /// </summary>
        [HideFromIl2Cpp]
        private void PositionKeyboard( GameObject go, RectTransform terminalCanvasRect, CanvasPosition position)
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
        public void HandleModifier(KeyDefinition key, bool activate)
        {
            if (activate)
                m_activeModifiers.Add(key.KeyCode);
            else
                m_activeModifiers.Remove(key.KeyCode);
        }

            [HideFromIl2Cpp]
        public void HandleInput( KeyDefinition key )
        {
            if (key.IsModifier())
            {
                // See HandleModifier();
                return;
            }

            if (!key.HasKeyCode() && !key.HasInput())
            {
                // Modifiers will be cleared on next keycode or input in lateUpdate(), so that it can be applied to them.
                // If the input has neither of these, we just clear it now.
                m_activeModifiers.Clear();
            }

            // A keycode down does not result in text being added to the terminal, but is required for some single-key actions
            // Always performing it, in addition to adding its input, seems to work fine.
            if (key.HasKeyCode())
            {
                m_currentFrameKeycode.Add(key.KeyCode);
            }

            if (key.HasInput())
            {
                m_currentFrameInput += key.Input;
            }
            else
            {
                switch(key.KeyType)
                {
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

        public void LateUpdate()
        {
            // If there was a string/keycode input prev frame (and thus presented this frame), the active modifier will have been applied to it.
            // Afterwards we can clear the modifier. Button press will have caused the modifier button to visually clear its state.
            if (!String.IsNullOrEmpty(m_prevFrameInput) || m_prevFrameKeycode.Count > 0)
            {
                m_activeModifiers.Clear();
            }

            // We receive input during normal update, but it is read earlier.
            // Make a copy of the input this frame, and serve it as input for the entirety of next frame.
            m_prevFrameInput = m_currentFrameInput;
            m_currentFrameInput = "";

            // Swap the hashsets so current is prev, and prev is current and cleared. 
            (m_prevFrameKeycode, m_currentFrameKeycode) = (m_currentFrameKeycode, m_prevFrameKeycode);
            m_currentFrameKeycode.Clear();

        }

        public static string GetKeyboardInput()
        {
            return m_prevFrameInput;
        }

        public static bool GetKeycodeDown( KeyCode key )
        {
            return m_prevFrameKeycode.Contains(key) || m_activeModifiers.Contains(key);
        }

        public static bool HasKeyboardInput()
        {
            return !String.IsNullOrEmpty(m_prevFrameInput);
        }

        [HideFromIl2Cpp]
        public void HandleInput(string str)
        {
            m_currentFrameInput += str;
        }

        [HideFromIl2Cpp]
        private static KeyboardLayout GetRightKeyboardLayout( out KeyDefinition zoneButton )
        {
            LinearLayout bottomKeyboardLayout = new LinearLayout(LinearOrientation.VERTICAL, TextAnchor.LowerLeft, LayoutParameters.WrapContent());
            bottomKeyboardLayout.ShowBackground = true;

            LayoutParameters rowParams = new LayoutParameters( LayoutParameters.WRAP_CONTENT, 1);

            bottomKeyboardLayout.AddChild(new KeyDefinition("TERMINAL_ ", "TERMINAL", 4).SetApperance(KeyApperanceType.ALT));
            
            bottomKeyboardLayout.AddChild(new KeyDefinition("BULKHEAD ", "BULKHEAD", 4).SetApperance(KeyApperanceType.ALT));

            bottomKeyboardLayout.AddChild(new KeyDefinition("SECURITY ", "SECURITY", 4).SetApperance(KeyApperanceType.ALT));

            bottomKeyboardLayout.AddChild(new KeyDefinition("UNKNOWN ", "UNKNOWN", 4).SetApperance(KeyApperanceType.ALT));

            {
                LinearLayout keyboardRow = new LinearLayout(LinearOrientation.HORIZONTAL, TextAnchor.LowerLeft, rowParams);
                keyboardRow.AddChild(new KeyDefinition("AMMOPACK ", "AMMO ", 2).SetApperance(KeyApperanceType.ALT));
                keyboardRow.AddChild(new KeyDefinition("TOOL_REFILL ", "TOOL ", 2).SetApperance(KeyApperanceType.ALT));
                bottomKeyboardLayout.AddChild(keyboardRow);
            }

            {
                LinearLayout keyboardRow = new LinearLayout(LinearOrientation.HORIZONTAL, TextAnchor.LowerLeft, rowParams);
                keyboardRow.AddChild(new KeyDefinition("MEDIPACK ", "MEDI", 2).SetApperance(KeyApperanceType.ALT));
                keyboardRow.AddChild(new KeyDefinition("DISINFECT ", "DISIN", 2).SetApperance(KeyApperanceType.ALT));
                bottomKeyboardLayout.AddChild(keyboardRow);
            }

            bottomKeyboardLayout.AddChild(new KeyDefinition("RESOURCES ", "RESOURCES", 4).SetApperance(KeyApperanceType.ALT));

            {
                // This will be updated when assigned to a terminal
                zoneButton = new KeyDefinition("ZONE_-1 ", "ZONE_-1", 4).SetApperance(KeyApperanceType.ALT);
                bottomKeyboardLayout.AddChild(zoneButton);
            }

            return bottomKeyboardLayout;
        }

        [HideFromIl2Cpp]
        private static KeyboardLayout GetLeftKeyboardLayout()
        {
            LinearLayout bottomKeyboardLayout = new LinearLayout(LinearOrientation.VERTICAL, TextAnchor.LowerRight, LayoutParameters.WrapContent());
            bottomKeyboardLayout.ShowBackground = true;

            LayoutParameters rowParams = new LayoutParameters(LayoutParameters.WRAP_CONTENT, 1);
 
            bottomKeyboardLayout.AddChild(new KeyDefinition("UPLINK_VERIFY ", "UPLINK_VERIFY", 5).SetApperance(KeyApperanceType.ALT));

            bottomKeyboardLayout.AddChild(new KeyDefinition("UPLINK_CONNECT ", "UPLINK_CONNECT", 5).SetApperance(KeyApperanceType.ALT));
            
            bottomKeyboardLayout.AddChild(new KeyDefinition("REACTOR_VERIFY ", "REACTOR_VERIFY", 5).SetApperance(KeyApperanceType.ALT));

            {
                LinearLayout keyboardRow = new LinearLayout(LinearOrientation.HORIZONTAL, TextAnchor.LowerRight, rowParams);
                keyboardRow.AddChild(new KeyDefinition("ZONE_", "ZONE_", 3.5f).SetApperance(KeyApperanceType.ALT));
                keyboardRow.AddChild(new KeyDefinition("-T ", "-T", 1.5f).SetApperance(KeyApperanceType.ALT));
                bottomKeyboardLayout.AddChild(keyboardRow);
            }

            {
                LinearLayout keyboardRow = new LinearLayout(LinearOrientation.HORIZONTAL, TextAnchor.LowerRight, rowParams);
                keyboardRow.AddChild(new KeyDefinition("LOGS\r", "LOGS", 2.5f).SetApperance(KeyApperanceType.ALT));
                keyboardRow.AddChild(new KeyDefinition("LIST ", "LIST", 2.5f).SetApperance(KeyApperanceType.ALT));
                bottomKeyboardLayout.AddChild(keyboardRow);
            }

            {
                LinearLayout keyboardRow = new LinearLayout(LinearOrientation.HORIZONTAL, TextAnchor.LowerRight, rowParams);
                keyboardRow.AddChild(new KeyDefinition("READ ", "READ", 2.5f).SetApperance(KeyApperanceType.ALT));
                keyboardRow.AddChild(new KeyDefinition("PING ", "PING", 2.5f).SetApperance(KeyApperanceType.ALT));
                bottomKeyboardLayout.AddChild(keyboardRow);
            }

            {
                LinearLayout keyboardRow = new LinearLayout(LinearOrientation.HORIZONTAL, TextAnchor.LowerRight, rowParams);
                keyboardRow.AddChild(new KeyDefinition("INFO\r", "INFO", 2.5f).SetApperance(KeyApperanceType.ALT));
                keyboardRow.AddChild(new KeyDefinition("QUERY ", "QUERY", 2.5f).SetApperance(KeyApperanceType.ALT));
                bottomKeyboardLayout.AddChild(keyboardRow);
            }

            {
                bottomKeyboardLayout.AddChild(new KeyDefinition("COMMANDS\r", "COMMANDS", 5f).SetApperance(KeyApperanceType.ALT));
            }

            return bottomKeyboardLayout;
        }

        [HideFromIl2Cpp]
        private static KeyboardLayout GetBottomKeyboardLayout()
        {
            LinearLayout bottomKeyboardLayout = new LinearLayout(LinearOrientation.VERTICAL, TextAnchor.UpperCenter, LayoutParameters.WrapContent() );
            bottomKeyboardLayout.ShowBackground = true;

            LayoutParameters rowParams = new LayoutParameters(15, 1);

            {
                LinearLayout keyboardRow = new LinearLayout(LinearOrientation.HORIZONTAL, TextAnchor.UpperCenter, rowParams);

                keyboardRow.AddChild(new KeyDefinition(KeyType.ESC, "x")
                    .SetApperance(KeyApperanceType.EXIT));
                keyboardRow.AddChild(new KeyDefinition("1").SetKeycode(KeyCode.Alpha1));
                keyboardRow.AddChild(new KeyDefinition("2").SetKeycode(KeyCode.Alpha2));
                keyboardRow.AddChild(new KeyDefinition("3").SetKeycode(KeyCode.Alpha3));
                keyboardRow.AddChild(new KeyDefinition("4").SetKeycode(KeyCode.Alpha4));
                keyboardRow.AddChild(new KeyDefinition("5").SetKeycode(KeyCode.Alpha5));
                keyboardRow.AddChild(new KeyDefinition("6").SetKeycode(KeyCode.Alpha6));
                keyboardRow.AddChild(new KeyDefinition("7").SetKeycode(KeyCode.Alpha7));
                keyboardRow.AddChild(new KeyDefinition("8").SetKeycode(KeyCode.Alpha8));
                keyboardRow.AddChild(new KeyDefinition("9").SetKeycode(KeyCode.Alpha9));
                keyboardRow.AddChild(new KeyDefinition("0").SetKeycode(KeyCode.Alpha0));
                keyboardRow.AddChild(new KeyDefinition("["));
                keyboardRow.AddChild(new KeyDefinition("\u0008", "Backspace", new LayoutParameters( LayoutParameters.FILL_PARENT ))
                    .SetKeycode(KeyCode.Backspace)
                    .SetRepeatKey(true)
                    .SetApperance(KeyApperanceType.ALT));

                bottomKeyboardLayout.AddChild(keyboardRow);
            }

            {
                LinearLayout shortRowHorizontal = new LinearLayout(LinearOrientation.HORIZONTAL, TextAnchor.UpperCenter, new LayoutParameters(15, 3));
                LinearLayout shortRowVertical = new LinearLayout(LinearOrientation.VERTICAL, TextAnchor.UpperLeft, new LayoutParameters(12.5f, 3));
                LayoutParameters shortRowParams = new LayoutParameters(12.5f, 1);

                {
                    LinearLayout keyboardRow = new LinearLayout(LinearOrientation.HORIZONTAL, TextAnchor.UpperLeft, shortRowParams);

                    keyboardRow.AddChild(new KeyDefinition(KeyType.EMPTY, "", 1.5f) // Tab
                        .SetApperance(KeyApperanceType.ALT));
                    keyboardRow.AddChild(new KeyDefinition("q").SetKeycode(KeyCode.Q));
                    keyboardRow.AddChild(new KeyDefinition("w").SetKeycode(KeyCode.W));
                    keyboardRow.AddChild(new KeyDefinition("e").SetKeycode(KeyCode.E));
                    keyboardRow.AddChild(new KeyDefinition("r").SetKeycode(KeyCode.R));
                    keyboardRow.AddChild(new KeyDefinition("t").SetKeycode(KeyCode.T));
                    keyboardRow.AddChild(new KeyDefinition("y").SetKeycode(KeyCode.Y));
                    keyboardRow.AddChild(new KeyDefinition("u").SetKeycode(KeyCode.U));
                    keyboardRow.AddChild(new KeyDefinition("i").SetKeycode(KeyCode.I));
                    keyboardRow.AddChild(new KeyDefinition("o").SetKeycode(KeyCode.O));
                    keyboardRow.AddChild(new KeyDefinition("p").SetKeycode(KeyCode.P));
                    keyboardRow.AddChild(new KeyDefinition("]"));

                    shortRowVertical.AddChild(keyboardRow);
                }


                {
                    LinearLayout keyboardRow = new LinearLayout(LinearOrientation.HORIZONTAL, TextAnchor.UpperLeft, shortRowParams);

                    keyboardRow.AddChild(new KeyDefinition(KeyType.EMPTY, "", 1.5f)
                        .SetApperance(KeyApperanceType.ALT));
                    keyboardRow.AddChild(new KeyDefinition(KeyType.EMPTY, "", 0.35f)
                    .SetApperance(KeyApperanceType.GONE));
                    keyboardRow.AddChild(new KeyDefinition("a").SetKeycode(KeyCode.A));
                    keyboardRow.AddChild(new KeyDefinition("s").SetKeycode(KeyCode.S));
                    keyboardRow.AddChild(new KeyDefinition("d").SetKeycode(KeyCode.D));
                    keyboardRow.AddChild(new KeyDefinition("f").SetKeycode(KeyCode.F));
                    keyboardRow.AddChild(new KeyDefinition("g").SetKeycode(KeyCode.G));
                    keyboardRow.AddChild(new KeyDefinition("h").SetKeycode(KeyCode.H));
                    keyboardRow.AddChild(new KeyDefinition("j").SetKeycode(KeyCode.J));
                    keyboardRow.AddChild(new KeyDefinition("k").SetKeycode(KeyCode.K));
                    keyboardRow.AddChild(new KeyDefinition("l").SetKeycode(KeyCode.L));
                    keyboardRow.AddChild(new KeyDefinition("-"));
                    keyboardRow.AddChild(new KeyDefinition(KeyType.ENTER, "", new LayoutParameters(LayoutParameters.FILL_PARENT, 1, 0.01f))
                        .SetApperance(KeyApperanceType.GONE));

                    shortRowVertical.AddChild(keyboardRow);
                }


                {
                    LinearLayout keyboardRow = new LinearLayout(LinearOrientation.HORIZONTAL, TextAnchor.UpperLeft, rowParams);

                    keyboardRow.AddChild(new KeyDefinition(KeyType.EMPTY, "", 2.4f)
                        .SetApperance(KeyApperanceType.ALT));
                    keyboardRow.AddChild(new KeyDefinition("z").SetKeycode(KeyCode.Z));
                    keyboardRow.AddChild(new KeyDefinition("x").SetKeycode(KeyCode.X));
                    keyboardRow.AddChild(new KeyDefinition("c").SetKeycode(KeyCode.C));
                    keyboardRow.AddChild(new KeyDefinition("v").SetKeycode(KeyCode.V));
                    keyboardRow.AddChild(new KeyDefinition("b").SetKeycode(KeyCode.B));
                    keyboardRow.AddChild(new KeyDefinition("n").SetKeycode(KeyCode.N));
                    keyboardRow.AddChild(new KeyDefinition("m").SetKeycode(KeyCode.M));
                    keyboardRow.AddChild(new KeyDefinition("."));
                    keyboardRow.AddChild(new KeyDefinition("_"));
                    keyboardRow.AddChild(new KeyDefinition(KeyType.UP, "^", 1.1f)
                        .SetRepeatKey(true)
                        .SetKeycode(KeyCode.UpArrow)
                        .SetApperance(KeyApperanceType.ALT));
                    //keyboardRow.AddChild(new KeyDefinition(KeyType.EMPTY, "", new KeyboardLayoutParameters(1f, true)));

                    shortRowVertical.AddChild(keyboardRow);
                }

                shortRowHorizontal.AddChild(shortRowVertical);
                shortRowHorizontal.AddChild(new KeyDefinition(KeyType.ENTER, "Enter", new LayoutParameters(LayoutParameters.FILL_PARENT, 3f))
                    .SetApperance(KeyApperanceType.ALT));
                bottomKeyboardLayout.AddChild(shortRowHorizontal);
            }

            {
                LinearLayout keyboardRow = new LinearLayout(LinearOrientation.HORIZONTAL, TextAnchor.UpperCenter, rowParams);

                keyboardRow.AddChild(new KeyDefinition(KeyType.CTRL, "Ctrl", 1.6f)
                    .SetApperance(KeyApperanceType.ALT));
                keyboardRow.AddChild(new KeyDefinition(KeyType.EMPTY, "", 1.6f)
                    .SetApperance(KeyApperanceType.ALT));
                keyboardRow.AddChild(new KeyDefinition(KeyType.SPACE, "Space", 7.2f));
                keyboardRow.AddChild(new KeyDefinition(KeyType.LEFT, "<")
                    .SetRepeatKey(true)
                    .SetKeycode(KeyCode.LeftArrow)
                    .SetApperance(KeyApperanceType.ALT));
                keyboardRow.AddChild(new KeyDefinition(KeyType.DOWN, "v", 1.1f)
                    .SetRepeatKey(true)
                    .SetKeycode(KeyCode.DownArrow)
                    .SetApperance(KeyApperanceType.ALT));
                keyboardRow.AddChild(new KeyDefinition(KeyType.RIGHT, ">")
                    .SetRepeatKey(true)
                    .SetKeycode(KeyCode.RightArrow)
                    .SetApperance(KeyApperanceType.ALT));
                keyboardRow.AddChild(new KeyDefinition(KeyType.ESC, "x", new LayoutParameters(LayoutParameters.FILL_PARENT))
                    .SetKeycode(KeyCode.Escape)
                    .SetApperance(KeyApperanceType.EXIT));

                bottomKeyboardLayout.AddChild(keyboardRow);
            }

            return bottomKeyboardLayout;
        }

        private void OnDestroy()
        {
            // Let's not leak materials
            m_keyboardStyle.Cleanup();

        }

    }


}
