using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace GTFO_VR.Events
{

    public static class PlayerFireWeaponEvents
    {
        public static event Action<Weapon> OnPlayerFireWeapon;

        public static void WeaponFired(Weapon weapon)
        {
            OnPlayerFireWeapon?.Invoke(weapon);
        }
    }
}
