using Agents;
using Gear;
using GTFO_VR.Core;
using GTFO_VR.Core.PlayerBehaviours;
using GTFO_VR.Core.VR_Input;
using GTFO_VR.Events;
using HarmonyLib;
using Il2CppSystem.Collections.Generic;
using UnityEngine;

namespace GTFO_VR.Injections
{
    /// <summary>
    /// Do not play any animations
    /// </summary>
    [HarmonyPatch(typeof(MeleeWeaponFirstPerson), nameof(MeleeWeaponFirstPerson.Setup))]
    static class InjectVRHammer
    {
        static void Postfix(MeleeWeaponFirstPerson __instance)
        {
            __instance.gameObject.AddComponent<VRHammer>().Setup(__instance);

        }
    }

    /// <summary>
    /// Get controller velocity for smack
    /// </summary>
    [HarmonyPatch(typeof(MeleeWeaponFirstPerson), nameof(MeleeWeaponFirstPerson.DoAttackDamage))]
    static class InjectVRHammerKillVelocity
    {
        static void Postfix(MeleeWeaponFirstPerson __instance, MeleeWeaponDamageData data, bool isPush)
        {
            Vector3 velocity = Controllers.mainControllerPose.GetVelocity() * 3f;
            data.sourcePos = data.hitPos - data.hitNormal * velocity.magnitude;
            if(isPush)
            {
                HammerEvents.HammerSmacked(0f);
            }
            else
            {
                HammerEvents.HammerSmacked(__instance.m_damageToDeal / __instance.m_damageHeavy);
            }
        }
    }
}