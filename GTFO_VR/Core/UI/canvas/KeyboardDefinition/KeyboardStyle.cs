using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

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

        public Material keyboardMaterial;
        public Material fontMaterial;
        public Material pointerMaterial;

        public KeyboardStyle(float tileSize = 1, float fontSize = 0.5f, float spacing = 0.1f)
        {
            this.TileSize = tileSize;
            this.FontSize = fontSize;
            this.SpacingVertical = spacing;
            this.SpacingHorizontal = spacing;

           keyColor = new Color(0, 0.30f, 0.30f) * colorBrightnessMultiplier;
           keyColor.a = 1;

           fontColor = new Color(1, 1, 1) * ( colorBrightnessMultiplier * 3) ;

           highlightColor = new Color(1.5f, 1.5f, 1.5f) * colorBrightnessMultiplier;
           highlightColor.a = 1;
    }

    }
}
