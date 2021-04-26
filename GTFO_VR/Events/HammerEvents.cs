using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace GTFO_VR.Events
{

    public static class HammerEvents
    {
        public static event Action<float> OnHammerSmack;
        public static event Action<float> OnHammerCharging;

        public static void HammerSmacked(float damage)
        {
            OnHammerSmack?.Invoke(damage);
        }

        public static void HammerCharging(float progress)
        {
            OnHammerCharging?.Invoke(progress);
        }
    }
}
