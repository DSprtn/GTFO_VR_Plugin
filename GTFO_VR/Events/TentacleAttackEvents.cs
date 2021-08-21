using System;
using UnityEngine;
using Agents;

namespace GTFO_VR.Events
{
    public static class TentacleAttackEvents
    {
        public static event Action<float, Agent, Vector3> OnTentacleAttack;

        public static void TentacleAttack(float amount, Agent sourceAgent, Vector3 position)
        {
            OnTentacleAttack?.Invoke(amount, sourceAgent, position);
        }
    }
}