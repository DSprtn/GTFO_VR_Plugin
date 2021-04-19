using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace GTFO_VR.Events
{

    public static class PlayerReceivedDamageEvents
    {
        public static event Action<float, Vector3> OnPlayerTakeDamage;

        public static void DamageTaken(float amount, Vector3 direction)
        {
            OnPlayerTakeDamage?.Invoke(amount, direction);
        }
    }
}
