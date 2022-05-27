using GTFO_VR.Core.UI.Terminal.Pointer;
using UnityEngine;
using UnityEngine.Rendering;

namespace GTFO_VR.Core.UI.Terminal.KeyboardDefinition
{
    public struct BackgroundStyle
    {
        public float radius;
        public int cornerVertices;
        public float padding;
    }

    public class KeyboardStyle
    {
        // Colors, even on unlit materials, appear a lot darker in-game than in unity.
        // Set this to 0.15f in GTFO to make the colors match. 
        // Exception for font. See getFontColor()
        public static float colorBrightnessMultiplier = 0.15f;

        public float TileSize = 1f;
        public float FontSize = 0.5f;

        // Color at max brightness
        public Color baseKeyColor = new Color(31f / 255f, 158f / 255f, 255f / 255f);
        public Color altKeyColor = new Color(23f / 255f, 118f / 255f, 191f / 255f);
        public Color exitKeyColor = new Color(191f / 255f, 23f / 255f, 23f / 255f);

        float KeyColorBrightness = 0.52f;
        float KeyColorPressedBrightness = 0.34f;
        float KeyColorHighlightBrightness = 0.75f;

        private Color fontColor = new Color(1, 1, 1);

        public Color textHighlightColor = new Color(0f, 0.075f, 0.075f);
        public Color backgroundColor = new Color(34f / 255f, 34f / 255f, 34f / 255f);

        private Material keyMaterial;
        private Material fontMaterial;
        private Material backgroundMaterial;

        public static Color pointerLineColor = new Color(1, 1, 1);

        public static Color getPointerLineColor()
        {
            return pointerLineColor * colorBrightnessMultiplier;
        }

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
            padding = 0.06f,
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


        public KeyboardStyle()
        {

        }

        public Color getTextColor()
        {
            // Text colors behaves a bit differently because different material I guess?
            Color color = fontColor * (colorBrightnessMultiplier * 1.5f );
            color.a = 1;
            return color;
        }

        public Material getKeyMaterial()
        {
            if (keyMaterial == null)
            {
                keyMaterial = new Material(Shader.Find("UI/Default"));
                keyMaterial.renderQueue = (int)RenderQueue.Overlay + 1;  // But still need to render underneath our text
                keyMaterial.SetInt("unity_GUIZTestMode", (int)UnityEngine.Rendering.CompareFunction.Always); // Magic no zcheck? zwrite?
            }

            return keyMaterial;
        }

        private Color adjustToGameBrightness(Color color)
        {
            return color * colorBrightnessMultiplier;
        }

        private Color getAdjustedKeyColor(Color color, float brightness)
        {
            return (adjustToGameBrightness(color * brightness));
        }

        public ColorStates getButtonColorStates( Color baseColor )
        {
            ColorStates states;

            states.normal =         new ColorTransitionState() { destinationColor = getAdjustedKeyColor(baseColor, KeyColorBrightness),           transitionTime = 0.5f };
            states.highlighted =    new ColorTransitionState() { destinationColor = getAdjustedKeyColor(baseColor, KeyColorHighlightBrightness),  transitionTime = 0f };
            states.pressed =        new ColorTransitionState() { destinationColor = getAdjustedKeyColor(baseColor, KeyColorPressedBrightness),    transitionTime = 0f };

            states.normal.destinationColor.a = 1;
            states.highlighted.destinationColor.a = 1;
            states.pressed.destinationColor.a = 1;

            return states;
        }

        public ColorStates getNormalKeyStates()
        {
            return getButtonColorStates(baseKeyColor);
        }

        public ColorStates getAltKeyStates()
        {
            return getButtonColorStates(altKeyColor);
        }

        public ColorStates getExitKeyStates()
        {
            return getButtonColorStates(exitKeyColor);
        }

        public Material getBackgroundMaterial()
        {
            if (backgroundMaterial == null)
            {
                backgroundMaterial = new Material(Shader.Find("UI/Default"));
                backgroundMaterial.renderQueue = (int)RenderQueue.Overlay ;  // But still need to render underneath our text
                backgroundMaterial.SetInt("unity_GUIZTestMode", (int)UnityEngine.Rendering.CompareFunction.Always); // Magic no zcheck? zwrite?

                Color color = backgroundColor * colorBrightnessMultiplier;
                color.a = 1;

                backgroundMaterial.color = color;

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
                fontMaterial.color = new Color(1, 1, 1, 1);
            }

            return fontMaterial;
        }
    }
}
