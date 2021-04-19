using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace GTFO_VR.Events
{

    public static class PlayerReloadEvents
    {
        public static event Action OnPlayerReloaded;

        public static void WeaponReloaded()
        {
            OnPlayerReloaded?.Invoke();
        }
    }
}
