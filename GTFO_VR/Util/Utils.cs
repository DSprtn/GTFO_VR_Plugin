using GTFO_VR.Core;
using GTFO_VR.Events;
using GTFO_VR.Input;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace GTFO_VR.Util
{
    public static class Utils
    {
        public static Transform FindDeepChild(Transform aParent, string aName)
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

        public static bool IsFiringFromADS()
        {
            return !VR_Settings.twoHandedAimingEnabled || (Controllers.aimingTwoHanded || !WeaponArchetypeVRData.GetVRWeaponData(ItemEquippableEvents.currentItem).allowsDoubleHanded);
        }

        public static bool CheckEnemyOverlap(Vector3 position, float radius)
        {
            return Physics.OverlapSphere(position, radius, LayerManager.MASK_MELEE_ATTACK_TARGETS).Length > 0;
        }

        public static int LargestDivisor(int n)
        {
            if (n % 2 == 0)
            {
                return n / 2;
            }
            int sqrtn = (int)Math.Sqrt(n);
            for (int i = 3; i <= sqrtn; i += 2)
            {
                if (n % i == 0)
                {
                    return n / i;
                }
            }
            return 1;
        }
    }
}
