﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Rendering;

namespace Assets.scripts.KeyboardDefinition
{
    public struct BackgroundStyle
    {
        public float radius;
        public int cornerVertices;
        public float padding;
    }

    public class KeyboardStyle
    {
        public static float colorBrightnessMultiplier = 1;

        public float TileSize = 2;
        public float FontSize = 0.8f;
        public float SpacingVertical = 0.0f;
        public float SpacingHorizontal = 0.0f;
        public float keyPadding = 0.01f;

        private Color keyColor = new Color( 13f / 255f, 83f / 255f, 103f / 255f);
        private Color fontColor = new Color(1, 1, 1);
        private Color highlightColor = new Color(0, 0.20f, 0.23f);


        public static Color pointerLineColor = new Color(0, 0.20f, 0.23f);
        public static Color pointerColor = new Color(0, 0.20f, 0.23f);
        public Color textHighlightColor = new Color(0f, 0.075f, 0.075f);

        public Color backgroundColor = new Color(34f / 255f, 34f / 255f, 34f / 255f);

        private Material keyMaterial;
        private Material fontMaterial;
        private Material backgroundMaterial;

        public BackgroundStyle keyboardBackgroundStyle = new BackgroundStyle()
        {
            radius = 0.1f,
            cornerVertices = 4,
            padding = -0.04f,

        };

        public BackgroundStyle keyBackgroundStyle = new BackgroundStyle()
        {
            radius = 0.1f,
            cornerVertices = 4,
            padding = 0.04f,
    };

        public void cleanup()
        {
            if (keyMaterial != null)
                UnityEngine.Object.Destroy(keyMaterial);
            if (fontMaterial != null)
                UnityEngine.Object.Destroy(fontMaterial);
            if (backgroundMaterial != null)
                UnityEngine.Object.Destroy(backgroundMaterial);

            keyMaterial = null;
            fontMaterial = null;
            backgroundMaterial = null;
        }


        public KeyboardStyle(float tileSize = 1, float fontSize = 0.5f, float spacing = 0.1f)
        {
            this.TileSize = tileSize;
            this.FontSize = fontSize;
            this.SpacingVertical = spacing;
            this.SpacingHorizontal = spacing;
        }

        public Material getKeyMaterial()
        {
            if (keyMaterial == null)
            {
                keyMaterial = new Material(Shader.Find("UI/Default"));
                keyMaterial.renderQueue = (int)RenderQueue.Overlay +1;  // But still need to render underneath our text
                keyMaterial.SetInt("unity_GUIZTestMode", (int)UnityEngine.Rendering.CompareFunction.Always); // Magic no zcheck? zwrite?
                keyMaterial.color = keyColor * colorBrightnessMultiplier;
            }

            return keyMaterial;
        }

        public Material getBackgroundMaterial()
        {
            if (backgroundMaterial == null)
            {
                backgroundMaterial = new Material(Shader.Find("UI/Default"));
                backgroundMaterial.renderQueue = (int)RenderQueue.Overlay ;  // But still need to render underneath our text
                backgroundMaterial.SetInt("unity_GUIZTestMode", (int)UnityEngine.Rendering.CompareFunction.Always); // Magic no zcheck? zwrite?
                backgroundMaterial.color = backgroundColor * colorBrightnessMultiplier;
            }

            return backgroundMaterial;
        }

        public Material getFontMaterial(Material existingMaterial)
        {
            if (fontMaterial == null)
            {
                fontMaterial = new Material(existingMaterial);
                fontMaterial.shader = Shader.Find("TextMeshPro/Distance Field Overlay"); // Not rendering ontop otherwise?
                fontMaterial.renderQueue = (int)RenderQueue.Overlay + 2;
                fontMaterial.color = fontColor * (colorBrightnessMultiplier * 3);
            }

            return fontMaterial;
        }

        public static Color getPointerLineColor()
        {
            pointerLineColor = new Color(0f, 1f, 1f);// * colorBrightnessMultiplier;
            pointerLineColor.a = 1;

            return pointerLineColor;
        }

        public static Color getPointerColor()
        {
            pointerColor = new Color(1f, 1f, 1f);// * colorBrightnessMultiplier;
            pointerColor.a = 1;

            return pointerColor;
        }

    }
}
