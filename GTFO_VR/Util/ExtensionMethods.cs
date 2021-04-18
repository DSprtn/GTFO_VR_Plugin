using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

namespace GTFO_VR.Util
{
    public static class ExtensionMethods
    {
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

        public static bool Hex(string hexString, ref Color color)
        {
            if (hexString.StartsWith("#"))
                hexString = hexString.Substring(1);
            if (hexString.Length == 6)
                hexString += "FF";
            if (hexString.Length != 8)
                return false;
            int[] numArray = new int[4];
            try
            {
                numArray[0] = int.Parse(hexString.Substring(0, 2), NumberStyles.HexNumber);
                numArray[1] = int.Parse(hexString.Substring(2, 2), NumberStyles.HexNumber);
                numArray[2] = int.Parse(hexString.Substring(4, 2), NumberStyles.HexNumber);
                numArray[3] = int.Parse(hexString.Substring(6, 2), NumberStyles.HexNumber);


                color = new Color((float)numArray[0] / (float)byte.MaxValue, (float)numArray[1] / (float)byte.MaxValue, (float)numArray[2] / (float)byte.MaxValue, (float)numArray[3] / (float)byte.MaxValue);
                return true;
            }
            catch
            {
                return false;
            }
        }

    }
}