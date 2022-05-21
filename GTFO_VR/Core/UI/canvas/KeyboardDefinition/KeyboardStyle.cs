using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Rendering;

namespace Assets.scripts.KeyboardDefinition
{
    public class KeyboardStyle
    {
        public static float colorBrightnessMultiplier = 1;

        public float TileSize = 2;
        public float FontSize = 0.8f;
        public float SpacingVertical = 0.0f;
        public float SpacingHorizontal = 0.0f;
        public float keyPadding = 0.01f;

        public Color keyColor = new Color(0, 0.45f, 0.43f);
        public Color fontColor = new Color(1, 1, 1);
        public Color highlightColor = new Color(0, 0.20f, 0.23f);


        public static Color pointerLineColor = new Color(0, 0.20f, 0.23f);
        public static Color pointerColor = new Color(0, 0.20f, 0.23f);
        public Color textHighlightColor = new Color(0f, 0.075f, 0.075f);

        private Material keyMaterial;
        private Material fontMaterial;

        public void cleanup()
        {
            if (keyMaterial != null)
                UnityEngine.Object.Destroy(keyMaterial);
            if (fontMaterial != null)
                UnityEngine.Object.Destroy(fontMaterial);
        }


        public KeyboardStyle(float tileSize = 1, float fontSize = 0.5f, float spacing = 0.1f)
        {
            this.TileSize = tileSize;
            this.FontSize = fontSize;
            this.SpacingVertical = spacing;
            this.SpacingHorizontal = spacing;

           keyColor = new Color(0, 0.30f, 0.30f) * colorBrightnessMultiplier;
           keyColor.a = 1;

           fontColor = new Color(1, 1, 1) * ( colorBrightnessMultiplier * 3) ;

           // Button tints multiply the existing color so don't need to be adjusted?
           highlightColor = new Color(1.5f, 1.5f, 1.5f);
           highlightColor.a = 1;
        }

        public Material getKeyMaterial()
        {
            if (keyMaterial == null)
            {
                keyMaterial = new Material(Shader.Find("UI/Default"));
                keyMaterial.renderQueue = (int)RenderQueue.Overlay;  // But still need to render underneath our text
                keyMaterial.SetInt("unity_GUIZTestMode", (int)UnityEngine.Rendering.CompareFunction.Always); // Magic no zcheck? zwrite?
                keyMaterial.color = keyColor;
            }

            return keyMaterial;
        }

        public Material getFontMaterial(Material existingMaterial)
        {
            if (fontMaterial == null)
            {
                fontMaterial = new Material(existingMaterial);
                fontMaterial.shader = Shader.Find("TextMeshPro/Distance Field Overlay"); // Not rendering ontop otherwise?
                fontMaterial.renderQueue = (int)RenderQueue.Overlay + 1;
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
