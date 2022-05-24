using Assets.scripts.KeyboardDefinition;
using GTFO_VR.Core.UI.Canvas.Pointer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;
using UnityEngine.UI;

namespace GTFO_VR.Core.UI.Canvas.KeyboardDefinition
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
        NORMAL, ALT, GONE
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

        public KeyDefinition(string input) : this(input, input) { }

        public KeyDefinition(string input, string label) : this(input, label, new LayoutParameters() ){ }

        public KeyDefinition(string input, string label, int width) : this(input, label, new LayoutParameters(width)) { }

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

        public GameObject GenerateLayout(TerminalKeyboardInterface keyboardRoot, KeyboardStyle style)
        {
            
            if (this.style != null)
                style = this.style;

            GameObject buttonRoot = new GameObject();
            buttonRoot.layer = TerminalKeyboardInterface.LAYER;
            buttonRoot.name = GetName();

            PhysicalButton button = buttonRoot.AddComponent<PhysicalButton>();

            LayoutElement element = buttonRoot.AddComponent<LayoutElement>();
            this.layoutParameters.populateLayoutElement(element, style);

            /////////////////////
            // Button itself
            /////////////////////

            button.m_background.setSize(element.preferredWidth, element.preferredHeight);
            button.m_background.setMaterial(style.getKeyMaterial());
            button.m_background.radius = style.keyBackgroundStyle.radius;
            button.m_background.cornerVertices = style.keyBackgroundStyle.cornerVertices;
            button.m_background.padding = style.keyBackgroundStyle.padding;
            button.m_background.regenerate();

            switch (apperance)
            {
                case KeyApperanceType.NORMAL:
                    button.setColorStates(style.getButtonColorStates());
                    break;
                case KeyApperanceType.ALT:
                    button.setColorStates(style.getAltButtonColorStates());
                    break;
                case KeyApperanceType.GONE:
                    button.setBackgroundEnabled(false);
                    break;
            }

            ///////////////////
            // Text 
            ///////////////////

            GameObject textObject = new GameObject();
            textObject.transform.SetParent(buttonRoot.transform);
            RectTransform textMeshRect = textObject.AddComponent<RectTransform>();

            TextMeshProUGUI textMesh = textObject.AddComponent<TextMeshProUGUI>();
            textMesh.text = Label;
            // Center all the things
            textMesh.fontSize = style.FontSize;
            textMesh.alignment = TextAlignmentOptions.Center;
            textMesh.fontSharedMaterial = style.getFontMaterial( textMesh.fontMaterial ); ;
            textMesh.color = style.getTextColor();

            // Some of these buttons have their sizes resolved at runtime, so text object much grow to fit their content
            ContentSizeFitter sizeFitter = textObject.AddComponent<ContentSizeFitter>();
            sizeFitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
            sizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;


            /////////////////////
            // Click listener
            /////////////////////

            button.onClick.AddListener( (UnityAction) (() => handleClick(keyboardRoot)) );
            if (RepeatKey)
                button.m_repeatKey = true;

            ////////////////////
            /// Box collider
            /// /////////////////

            button.setSize(element.preferredWidth, element.preferredHeight, 0.01f);
            if (element.flexibleWidth >= 0 || element.flexibleHeight >= 0)
            {
                // Size not known yet, add measuring thing.
                // Shouldn't this be flexible?
                buttonRoot.AddComponent<RectColliderSizer>();
            }

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