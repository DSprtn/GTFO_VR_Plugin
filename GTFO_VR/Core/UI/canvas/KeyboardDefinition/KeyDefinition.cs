﻿using GTFO_VR.UI.CANVAS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
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
        EMPTY,
    }

    class KeyDefinition : KeyboardLayout
    {
        KeyType KeyType = KeyType.INPUT;
        string Input;       // if character or input
        string Label;
        KeyboardLayoutParameters layoutParameters;
        KeyboardStyle style;

        public KeyDefinition(string input) : this(input, input) { }

        public KeyDefinition(string input, string label) : this(input, label, new KeyboardLayoutParameters() ){ }

        public KeyDefinition(string input, string label, KeyboardLayoutParameters layoutParameters)
        {
            this.Input = input;
            this.Label = label;
            this.layoutParameters = layoutParameters;
        }

        public KeyDefinition(KeyType type, string label) : this(type, label, new KeyboardLayoutParameters() ){ }

        public KeyDefinition(KeyType type, string label, float width) : this(type, label, new KeyboardLayoutParameters(width, false)) { }


        public KeyDefinition(KeyType type, string label, KeyboardLayoutParameters layoutParameters)
        {
            this.KeyType = type;
            this.Label = label;
            this.layoutParameters = layoutParameters;
        }

        public GameObject GenerateLayout(TerminalKeyboardInterface keyboardRoot, KeyboardStyle style)
        {
            
            if (this.style != null)
                style = this.style;

            GameObject buttonRoot = new GameObject();
            buttonRoot.name = GetName();

            // Temporary until I can figure out a better way of adding borders
            buttonRoot.transform.localScale = new Vector3(0.99f, 0.99f, 0.99f);

            Image image = buttonRoot.AddComponent<Image>();
            image.color = style.keyColor;
            image.material.renderQueue = 3002;  // But still need to render underneath our text
            image.material.SetInt("unity_GUIZTestMode", (int)UnityEngine.Rendering.CompareFunction.Always); // Magic no zcheck? zwrite?
            
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
            textMesh.fontSharedMaterial.shader = Shader.Find("TextMeshPro/Distance Field Overlay"); // Not rendering ontop otherwise?
            textMesh.fontSharedMaterial.renderQueue = 3003;

            // Some of these buttons have their sizes resolved at runtime, so have them grow to fit their content
            ContentSizeFitter sizeFitter = textObject.AddComponent<ContentSizeFitter>();
            sizeFitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
            sizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
               
            
            return buttonRoot;
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