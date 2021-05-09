using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

namespace GTFO_VR.Util
{
    public static class ExtensionMethods
    {

        public static float RemapClamped(this float aValue, float aIn1, float aIn2, float aOut1, float aOut2)
        {
            float t = (aValue - aIn1) / (aIn2 - aIn1);
            if (t > 1f)
                return aOut2;
            if (t < 0f)
                return aOut1;
            return aOut1 + (aOut2 - aOut1) * t;
        }

        public static Transform FindDeepChild(this Transform aParent, string aName)
        {
            Queue<Transform> queue = new Queue<Transform>();
            queue.Enqueue(aParent);
            while (queue.Count > 0)
            {
                var c = queue.Dequeue();
                if (c.name == aName)
                    return c;
                foreach (var t in c)
                {
                    Transform transform = t.Cast<Transform>();
                    queue.Enqueue(transform);
                }
            }
            return null;
        }

        public enum ColorSelection
        {
            WHITE = 0,
            RED = 1,
            GREEN = 2,
            BLUE = 3,
            CYAN = 4,
            YELLOW = 5,
            PINK = 6,
            MAGENTA = 7,
            ORANGE = 8,
            BLACK = 9
        }

        public static Color FromString(string color)
        {
            switch (color)
            {
                case ("WHITE"):
                    return Color.white;
                case ("RED"):
                    return ColorExt.RedBright(1f);
                case ("GREEN"):
                    return ColorExt.GreenBright(1f);
                case ("BLUE"):
                    return Color.blue;
                case ("CYAN"):
                    return Color.cyan;
                case ("YELLOW"):
                    return Color.yellow;
                case ("PINK"):
                    return ColorExt.Pink(1f);
                case ("MAGENTA"):
                    return Color.magenta;
                case ("ORANGE"):
                    return ColorExt.OrangeBright(1f);
                case ("BLACK"):
                    return Color.black;
                default:
                    return Color.white;
            }
            
        }
    }
}