using GTFO_VR.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace GTFO_VR.Events
{
    public static class HeldItemEvents
    {
        public static event Action<float> OnItemCharging;

        public static void ItemCharging(float progress)
        {
            OnItemCharging?.Invoke(progress);
        }
    }
}
