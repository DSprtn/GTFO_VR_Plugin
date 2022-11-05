using System.Collections.Generic;
using System.Globalization;
using TMPro;
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
                case ("ALABASTER_RED"):
                    return ColorExt.Hex("C4302B");
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

        public static Vector2 closestPointOnLine(this Vector2 point, Vector2 line0, Vector2 line1)
        {
            Vector2 line0_to_line1 = line1 - line0;
            float length = line0_to_line1.magnitude;

            // Infinite line
            Vector2 lineVector = line0_to_line1.normalized;
            Vector2 pointOnLine = lineVector * Vector2.Dot(point - line0, lineVector);
            pointOnLine += line0;


            // Cap to ends
            float distanceA = Vector2.Distance(pointOnLine, line0);
            float distanceB = Vector2.Distance(pointOnLine, line1);

            if (distanceA > length || distanceB > length)
            {
                if (distanceA < distanceB)
                    pointOnLine = line0;
                else
                    pointOnLine = line1;
            }

            return pointOnLine;
        }

        // This replaces TMP_TextUtilities.FindNearestCharacter(), which will output incorrect data in some situations.
        public static int FindNearestCharacterInWorldSpace(this TextMeshPro textMesh, Vector3 position)
        {
            // Translate our position into the local space of the textmesh
            // after this the z axis can be ignored.
            position = textMesh.transform.InverseTransformPoint(position);
            Vector2 position2d = new Vector2(position.x, position.y);

            // Find closest position to point on each side of character bounds, use to determine closest character
            int closestIndex = -1;
            float closestDistance = float.MaxValue;

            TMP_CharacterInfo[] characters = textMesh.textInfo.characterInfo;

            // The CharacterInfo array does not (always?) shrink when fewer characters are displayed.
            // Anything beyond m_totalCharacterCount will be old data that is not actually displayed.
            int actualCharacterCount = Mathf.Min(characters.Length, textMesh.m_totalCharacterCount); 

            for (int index = 0; index < actualCharacterCount; index++)
            {
                TMP_CharacterInfo character = characters[index];
                if (!character.isVisible)
                {
                    continue;
                }

                Vector2[] corners = new[]   // loops around, size 5
                {
                    new Vector2(character.topLeft.x, character.topLeft.y),
                    new Vector2(character.topRight.x, character.topRight.y),
                    new Vector2(character.bottomRight.x, character.bottomRight.y),
                    new Vector2(character.bottomLeft.x, character.bottomLeft.y),
                    new Vector2(character.topLeft.x, character.topLeft.y),
                };

                for (int i = 0; i < 4; i++)
                {
                    Vector2 closestPoint = position2d.closestPointOnLine(corners[i], corners[i + 1]);
                    float distance = Vector2.Distance(position2d, closestPoint);

                    if (distance < closestDistance)
                    {
                        closestDistance = distance;
                        closestIndex = index;
                    }
                }
            }

            return closestIndex;
        }
    }
}