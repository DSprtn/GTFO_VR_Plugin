﻿using GTFO_VR.Events;
using HarmonyLib;
using UnityEngine;
using Agents;

namespace GTFO_VR.Injections.Events
{
    /// <summary>
    /// Add event calls for player receiving damage from a tentacle attack
    /// </summary>

    [HarmonyPatch(typeof(Dam_SyncedDamageBase), nameof(Dam_SyncedDamageBase.TentacleAttackDamage))]
    internal class InjectTentacleAttackEvents
    {
        private static void Postfix(float dam, Agent sourceAgent, Vector3 position)
        {
            TentacleAttackEvents.TentacleAttack(dam, sourceAgent, position);
        }
    }
}