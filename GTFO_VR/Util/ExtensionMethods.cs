using System.Collections.Generic;
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
    }
}