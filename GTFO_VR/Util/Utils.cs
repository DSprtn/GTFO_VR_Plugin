using GTFO_VR.Core;
using GTFO_VR.Events;
using GTFO_VR.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GTFO_VR.Util
{
    public static class Utils
    {

        public static bool IsFiringFromADS()
        {
            return !VRSettings.twoHandedAimingEnabled || (Controllers.aimingTwoHanded || !WeaponArchetypeVRData.GetVRWeaponData(ItemEquippableEvents.currentItem).allowsDoubleHanded);
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
