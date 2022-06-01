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

    /// <summary>
    /// Provides all the colors and materials for the terminal keyboard
    /// </summary>
    public class KeyboardStyle
    {
        // Colors, even on unlit materials, appear a lot darker in-game than in unity.
        // Set this to 0.15f in GTFO to make the colors match. 
        // Exception for font. See getFontColor()
        public static float colorBrightnessMultiplier = 0.15f;

        public float TileSize = 1f;
        public float FontSize = 0.5f;

        // Color at max brightness
        public Color BaseKeyColor = new Color(31f / 255f, 158f / 255f, 255f / 255f);
        public Color AltKeyColor = new Color(23f / 255f, 118f / 255f, 191f / 255f);
        public Color ExitKeyColor = new Color(191f / 255f, 23f / 255f, 23f / 255f);

        public float KeyColorBrightness = 0.52f;
        public float KeyColorPressedBrightness = 0.34f;
        public float KeyColorHighlightBrightness = 0.75f;

        public Color FontColor = new Color(1, 1, 1);
        public Color TerminalHighlightColor = new Color(0f, 0.075f, 0.075f);
        public Color BackgroundColor = new Color(26f / 255f, 26f / 255f, 26f / 255f);

        private Material m_keyMaterial;
        private Material m_fontMaterial;
        private Material m_backgroundMaterial;

        public static Color pointerLineColor = new Color(1, 1, 1);

        public KeyboardStyle()
        {

        }

        public static Color GetPointerDotColor()
        {
            Color color = pointerLineColor * colorBrightnessMultiplier;
            color.a = 1;
            return color;
        }

        public static Color GetPointerLineColor()
        {
            // Alpha gradient doesn't look right unless we leave the multiplied alpha value here
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

        public Color GetTextColor()
        {
            // Text colors behaves a bit differently because different material I guess?
            Color color = FontColor * (colorBrightnessMultiplier * 1.5f );
            color.a = 1;
            return color;
        }

        public Material GetKeyMaterial()
        {
            if (m_keyMaterial == null)
            {
                m_keyMaterial = new Material(Shader.Find("UI/Default"));
                m_keyMaterial.renderQueue = (int)RenderQueue.Overlay + 1;  // But still need to render underneath our text
                m_keyMaterial.SetInt("unity_GUIZTestMode", (int)UnityEngine.Rendering.CompareFunction.Always); // Magic no zcheck? zwrite?
            }

            return m_keyMaterial;
        }

        /// <summary>
        /// Adjust the color so it looks the same in-game as it does in the unity editor
        /// </summary>
        private Color AdjustToGameBrightness(Color color)
        {
            return color * colorBrightnessMultiplier;
        }

        /// <summary>
        /// Adjust the color so it looks the same in-game as it does in the unity editor, and apply a brightness modifier
        /// </summary>
        private Color GetAdjustedKeyColor(Color color, float brightness)
        {
            return (AdjustToGameBrightness(color * brightness));
        }

        /// <summary>
        /// Get the set of key colors for the normal/highlighted/pressed states 
        /// </summary>
        public ColorStates GetButtonColorStates( Color baseColor )
        {
            ColorStates states;

            states.normal =         new ColorTransitionState() { destinationColor = GetAdjustedKeyColor(baseColor, KeyColorBrightness),           transitionTime = 0.5f };
            states.highlighted =    new ColorTransitionState() { destinationColor = GetAdjustedKeyColor(baseColor, KeyColorHighlightBrightness),  transitionTime = 0f };
            states.pressed =        new ColorTransitionState() { destinationColor = GetAdjustedKeyColor(baseColor, KeyColorPressedBrightness),    transitionTime = 0f };

            states.normal.destinationColor.a = 1;
            states.highlighted.destinationColor.a = 1;
            states.pressed.destinationColor.a = 1;

            return states;
        }

        public ColorStates GetNormalKeyStates()
        {
            return GetButtonColorStates(BaseKeyColor);
        }

        public ColorStates GetAltKeyStates()
        {
            return GetButtonColorStates(AltKeyColor);
        }

        public ColorStates GetExitKeyStates()
        {
            return GetButtonColorStates(ExitKeyColor);
        }

        public Material GetBackgroundMaterial()
        {
            if (m_backgroundMaterial == null)
            {
                m_backgroundMaterial = new Material(Shader.Find("UI/Default"));
                m_backgroundMaterial.renderQueue = (int)RenderQueue.Overlay ;  // But still need to render underneath our text
                m_backgroundMaterial.SetInt("unity_GUIZTestMode", (int)UnityEngine.Rendering.CompareFunction.Always); // Magic no zcheck? zwrite?

                Color color = BackgroundColor * colorBrightnessMultiplier;
                color.a = 1;

                m_backgroundMaterial.color = color;

            }

            return m_backgroundMaterial;
        }

        public Material GetFontMaterial(Material existingMaterial)
        {
            if (m_fontMaterial == null)
            {
                m_fontMaterial = new Material(existingMaterial);
                m_fontMaterial.shader = Shader.Find("TextMeshPro/Distance Field Overlay"); // Not rendering ontop otherwise?
                m_fontMaterial.renderQueue = (int)RenderQueue.Overlay + 2;
                m_fontMaterial.color = new Color(1, 1, 1, 1);
            }

            return m_fontMaterial;
        }

        /// <summary>
        /// Destroy all the materials we created
        /// </summary>
        public void Cleanup()
        {
            if (m_keyMaterial != null)
                UnityEngine.Object.Destroy(m_keyMaterial);
            if (m_fontMaterial != null)
                UnityEngine.Object.Destroy(m_fontMaterial);
            if (m_backgroundMaterial != null)
                UnityEngine.Object.Destroy(m_backgroundMaterial);

            m_keyMaterial = null;
            m_fontMaterial = null;
            m_backgroundMaterial = null;
        }
    }
}
