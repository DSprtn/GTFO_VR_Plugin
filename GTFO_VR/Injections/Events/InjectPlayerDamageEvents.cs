﻿using GTFO_VR.Events;
using HarmonyLib;
using UnityEngine;

namespace GTFO_VR.Injections.Events
{
    /// <summary>
    /// Add event calls for the player taking damage
    /// </summary>

    [HarmonyPatch(typeof(Dam_PlayerDamageLocal), nameof(Dam_PlayerDamageLocal.Hitreact))]
    internal class InjectPlayerDamageEvents
    {
        private static void Postfix(float damage, Vector3 direction)
        {
            PlayerReceivedDamageEvents.DamageTaken(damage, direction);
        }
    }

    [HarmonyPatch(typeof(MineDeployerInstance_Detonate_Explosive), nameof(MineDeployerInstance_Detonate_Explosive.DoExplode))]
    internal class InjectMineExplosionEvents
    {
        private static void Postfix(MineDeployerInstance_Detonate_Explosive __instance)
        {
            PlayerReceivedDamageEvents.MineExplosion(__instance.transform.position);
        }
    }
}