using GTFO_VR.Core.UI.canvas.KeyboardDefinition;
using GTFO_VR.UI.CANVAS;
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

namespace Assets.scripts.KeyboardDefinition
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
        EMPTY
    }

    public class KeyDefinition : KeyboardLayout
    {
        public KeyType KeyType = KeyType.INPUT;
        public string Input;       // if character or input
        public string Label;
        public KeyboardLayoutParameters layoutParameters;
        public KeyboardStyle style;

        public KeyDefinition(string input) : this(input, input) { }

        public KeyDefinition(string input, string label) : this(input, label, new KeyboardLayoutParameters() ){ }

        public KeyDefinition(string input, string label, int width) : this(input, label, new KeyboardLayoutParameters(width)) { }

        public KeyDefinition(string input, string label, KeyboardLayoutParameters layoutParameters)
        {
            this.Input = input;
            this.Label = label;
            this.layoutParameters = layoutParameters;
            populateInput();
        }

        public KeyDefinition(KeyType type, string label) : this(type, label, new KeyboardLayoutParameters() ){ }

        public KeyDefinition(KeyType type, string label, float width) : this(type, label, new KeyboardLayoutParameters(width, false)) { }


        public KeyDefinition(KeyType type, string label, KeyboardLayoutParameters layoutParameters)
        {
            this.KeyType = type;
            this.Label = label;
            this.layoutParameters = layoutParameters;
            populateInput();
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

            // Temporary until I can figure out a better way of adding borders
            buttonRoot.transform.localScale = new Vector3(
                TerminalKeyboardInterface.KEY_SCALE,
                TerminalKeyboardInterface.KEY_SCALE,
                TerminalKeyboardInterface.KEY_SCALE);

            Image image = buttonRoot.AddComponent<Image>();
            image.color = style.keyColor;

            if (style.keyMaterial == null)
            {
                style.keyMaterial = image.material;
                style.keyMaterial.renderQueue = (int)RenderQueue.Overlay;  // But still need to render underneath our text
                style.keyMaterial.SetInt("unity_GUIZTestMode", (int)UnityEngine.Rendering.CompareFunction.Always); // Magic no zcheck? zwrite?
            }

            image.material = style.keyMaterial;      
            
            Button button = buttonRoot.AddComponent<Button>();
            ColorBlock cb = button.colors;
            cb.highlightedColor = style.highlightColor;
            button.colors = cb;

            LayoutElement element = buttonRoot.AddComponent<LayoutElement>();
            this.layoutParameters.populateLayoutElement(element, style);

            
            // Add actual text. bounds same size as parent.
            GameObject textObject = new GameObject();
            textObject.transform.SetParent(buttonRoot.transform);
            RectTransform textMeshRect = textObject.AddComponent<RectTransform>();

            

            TextMeshProUGUI textMesh = textObject.AddComponent<TextMeshProUGUI>();
            textMesh.text = Label;
            // Center all the things
            textMesh.fontSize = style.FontSize;
            textMesh.alignment = TextAlignmentOptions.Center;
            textMesh.color = style.fontColor;


            //textMesh.fontSharedMaterial.SetInt("unity_GUIZTestMode", (int)UnityEngine.Rendering.CompareFunction.Always);
            //textMesh.fontSharedMaterial.shader = Shader.Find("TextMeshPro/Distance Field Overlay"); // Not rendering ontop otherwise?
            if(style.fontMaterial == null)
            {
                style.fontMaterial = textMesh.fontMaterial;
                style.fontMaterial.renderQueue = (int)RenderQueue.Overlay + 1;
            }

            textMesh.fontSharedMaterial = style.fontMaterial; 
            

            // Some of these buttons have their sizes resolved at runtime, so have them grow to fit their content
            ContentSizeFitter sizeFitter = textObject.AddComponent<ContentSizeFitter>();
            sizeFitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
            sizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;


            /////////////////////
            // Click listener
            /////////////////////

            button.onClick.AddListener( (UnityAction) (() => handleClick(keyboardRoot)) );

            ////////////////////
            /// Box collider
            /// /////////////////

            RectTransform trans = buttonRoot.GetComponent<RectTransform>();
            BoxCollider collider = buttonRoot.AddComponent<BoxCollider>();

            //collider.size = new Vector3(trans.sizeDelta.x, trans.sizeDelta.y, 0.01f);
            if (element.flexibleWidth >= 0 || element.flexibleHeight >= 0)
            {
                // Size not known yet, add measuring thing too, but also set size because we'll keep the z axis
                collider.size = new Vector3(element.preferredWidth * TerminalKeyboardInterface.HITBOX_SCALE, element.preferredHeight * TerminalKeyboardInterface.HITBOX_SCALE, 0.01f);
                buttonRoot.AddComponent<RectColliderSizer>();
            }
            else
            {
                // Constant size, just set
                collider.size = new Vector3(element.preferredWidth * TerminalKeyboardInterface.HITBOX_SCALE, element.preferredHeight * TerminalKeyboardInterface.HITBOX_SCALE, 0.01f);
            }

            //collider.attachedRigidbody

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