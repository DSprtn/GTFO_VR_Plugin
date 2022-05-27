using GTFO_VR.Core.UI.Terminal.Pointer;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace GTFO_VR.Core.UI.Terminal.KeyboardDefinition
{
    public enum KeyType
    {
        INPUT,  // characters or words
        SPACE, 
        ENTER, 
        TAB, 
        SHIFT,
        BACKPSPACE,
        ESC,
        CTRL,
        CAPS_LOCK,
        WIN,
        ALT,
        UP,
        DOWN,
        LEFT,
        RIGHT,
        EMPTY,
    }

    public enum KeyApperanceType
    {
        NORMAL, ALT, EXIT, GONE
    }

    public class KeyDefinition : KeyboardLayout
    {
        public KeyType KeyType = KeyType.INPUT;
        public string Input;       // if character or input
        public string Label;
        public LayoutParameters layoutParameters;
        public KeyboardStyle style;
        public bool RepeatKey = false;
        public KeyApperanceType apperance = KeyApperanceType.NORMAL;

        private PhysicalButton m_button;
        private TextMeshProUGUI m_text;

        public KeyDefinition(string input) : this(input, input) { }

        public KeyDefinition(string input, string label) : this(input, label, new LayoutParameters() ){ }

        public KeyDefinition(string input, string label, float width) : this(input, label, new LayoutParameters(width)) { }

        public KeyDefinition(string input, string label, LayoutParameters layoutParameters)
        {
            this.Input = input;
            this.Label = label;
            this.layoutParameters = layoutParameters;
            populateInput();
        }

        public KeyDefinition(KeyType type, string label) : this(type, label, new LayoutParameters() ){ }

        public KeyDefinition(KeyType type, string label, float width) : this(type, label, new LayoutParameters(width)) { }


        public KeyDefinition(KeyType type, string label, LayoutParameters layoutParameters)
        {
            this.KeyType = type;
            this.Label = label;
            this.layoutParameters = layoutParameters;
            populateInput();
        }

        public KeyDefinition setRepeatKey( bool repeatKey)
        {
            this.RepeatKey = repeatKey;
            return this;
        }

        public KeyDefinition setApperance( KeyApperanceType apperance )
        {
            this.apperance = apperance;
            return this;
        }

        public bool hasInput()
        {
            return Input != null;
        }

        private void populateInput()
        {
            if (Input != null)
                return;

            switch (this.KeyType)
            {
                case KeyType.SPACE:
                    {
                        this.Input = " ";
                        break;
                    }
                case KeyType.ENTER:
                    {
                        this.Input = "\r";
                        break;
                    }
                case KeyType.TAB:
                    {
                        this.Input = "\t";
                        break;
                    }
            }
        }

        public void updateContent( string input, string label )
        {
            this.Input = input;
            this.Label = label;
            m_text.text = this.Label;
        }

        public GameObject GenerateLayout(TerminalKeyboardInterface keyboardRoot, KeyboardStyle style)
        {
            
            if (this.style != null)
                style = this.style;

            GameObject buttonRoot = new GameObject();
            buttonRoot.layer = TerminalKeyboardInterface.LAYER;
            buttonRoot.name = GetName();

            m_button = buttonRoot.AddComponent<PhysicalButton>();

            LayoutElement element = buttonRoot.AddComponent<LayoutElement>();
            this.layoutParameters.populateLayoutElement(element, style);

            /////////////////////
            // Button itself
            /////////////////////

            m_button.m_background.setMaterial(style.getKeyMaterial());
            m_button.m_background.radius = style.keyBackgroundStyle.radius;
            m_button.m_background.cornerVertices = style.keyBackgroundStyle.cornerVertices;
            m_button.m_background.padding = style.keyBackgroundStyle.padding;

            switch (apperance)
            {
                case KeyApperanceType.NORMAL:
                    m_button.setColorStates(style.getNormalKeyStates());
                    break;
                case KeyApperanceType.ALT:
                    m_button.setColorStates(style.getAltKeyStates());
                    break;
                case KeyApperanceType.EXIT:
                    m_button.setColorStates(style.getExitKeyStates());
                    break;
                case KeyApperanceType.GONE:
                    m_button.setBackgroundEnabled(false);
                    break;
            }

            ///////////////////
            // Text 
            ///////////////////

            GameObject textObject = new GameObject();
            textObject.transform.SetParent(buttonRoot.transform);
            RectTransform textMeshRect = textObject.AddComponent<RectTransform>();

            m_text = textObject.AddComponent<TextMeshProUGUI>();
            m_text.text = Label;
            // Center all the things
            m_text.fontSize = style.FontSize;
            m_text.alignment = TextAlignmentOptions.Center;
            m_text.fontSharedMaterial = style.getFontMaterial(m_text.fontMaterial ); ;
            m_text.color = style.getTextColor();

            // Some of these buttons have their sizes resolved at runtime, so text object much grow to fit their content
            ContentSizeFitter sizeFitter = textObject.AddComponent<ContentSizeFitter>();
            sizeFitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
            sizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

            /////////////////////
            // Click listener
            /////////////////////

            m_button.onClick.AddListener( (UnityAction) (() => handleClick(keyboardRoot)) );
            if (RepeatKey)
                m_button.m_repeatKey = true;

            return buttonRoot;
        }

        public void handleClick( TerminalKeyboardInterface keyboardRoot )
        {
            keyboardRoot.HandleInput(this);
        }

        public void AddChild(KeyboardLayout layout)
        {
            // No.
            throw new NotImplementedException();
        }

        public string GetName()
        {
            return "Key[" + this.KeyType.ToString() + ":" + this.Label + "]";
        }

        public void SetStyle(KeyboardStyle style)
        {
            throw new NotImplementedException();
        }

    }
}