using System;
using UnityEngine;

namespace GTFO_VR.Events
{

    public static class PlayerReceivedDamageEvents
    {
        public static event Action<float, Vector3> OnPlayerTakeDamage;
        public static event Action<Vector3> OnMineExplosion;

        public static void DamageTaken(float amount, Vector3 direction)
        {
            OnPlayerTakeDamage?.Invoke(amount, direction);
        }

        public static void MineExplosion(Vector3 position)
        {
            OnMineExplosion?.Invoke(position);
        }
    }
}
