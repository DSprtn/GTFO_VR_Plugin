using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace GTFO_VR.Events
{

    public static class GlueGunEvents
    {
        public static event Action<float> OnPressureUpdate;

        public static void PressureBuilding(float pressure)
        {
            OnPressureUpdate?.Invoke(pressure);
        }
    }
}
