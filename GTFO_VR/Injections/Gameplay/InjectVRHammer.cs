using Agents;
using GameData;
using Gear;
using GTFO_VR.Core;
using GTFO_VR.Core.PlayerBehaviours;
using GTFO_VR.Core.VR_Input;
using GTFO_VR.Events;
using GTFO_VR.Util;
using HarmonyLib;
using Il2CppSystem.Collections.Generic;
using LevelGeneration;
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
            __instance.gameObject.AddComponent<VRMeleeWeapon>().Setup(__instance);
        }
    }

    /// <summary>
    /// Auto-Release charge when smackin something
    /// </summary>
    [HarmonyPatch(typeof(MWS_ChargeUp), nameof(MWS_ChargeUp.Update))]
    static class InjectAutoReleaseHammerSmack
    {
        static void Postfix(MWS_ChargeUp __instance)
        {
            if (VRMeleeWeapon.Current != null)
            {
                if (VRMeleeWeapon.Current.m_positionTracker.GetSmoothVelocity() > 2f)
                {
                    // For the sake of simplicity we discard the hits here and call it again when the original CheckForAttackTargets() is called
                    if (VRMeleeWeapon.Current.CheckForAttackTarget( out _)) 
                    {
                        __instance.OnChargeupRelease();
                        __instance.m_weapon.CurrentState.Update(); // Manually call update so it doesn't delay by a frame
                    }
                }
            }
        }
    }


    /// <summary>
    /// Patch animation data to remove attack delay --- attacks in VR should always hit instantly because the player is swinging the weapon physically instead of with an animation
    /// </summary>
    [HarmonyPatch(typeof(MWS_AttackSwingBase), nameof(MWS_AttackSwingBase.Update))]
    static class InjectAllowInstantDamageInVRSwing
    {

        static void Prefix(MWS_AttackSwingBase __instance)
        {
            __instance.m_data.m_damageStartTime = -1; // elapsed time on first frame ( 0 ) must be greater than this 

            // Do not check for hits from camera
            __instance.m_weapon.MeleeArchetypeData.CameraDamageRayLength = 0f;
        }
    }


    /// <summary>
    /// Sort hits to prioritize them based on proximity to hammer center (only first enemy hit counts)
    /// Light attacks
    /// </summary>
    [HarmonyPatch(typeof(MWS_AttackSwingBase), nameof(MWS_AttackSwingBase.OnAttackHit))]
    static class InjectPrioritizeProximityTargetsOnLightHit
    {
        static void Postfix(MWS_AttackSwingBase __instance)
        {
            __instance.m_weapon.HitsForDamage = VRMeleeWeapon.sortHits(__instance.m_weapon.HitsForDamage, __instance.m_data.m_damageRef.position);
        }
    }

    /// <summary>
    /// Sort hits to prioritize them based on proximity to hammer center (only first enemy hit counts)
    /// Heavy attacks
    /// </summary>
    [HarmonyPatch(typeof(MWS_AttackHeavy), nameof(MWS_AttackHeavy.OnAttackHit))]
    static class InjectPrioritizeProximityTargetsOnHeavyHit
    {
        static void Postfix(MWS_AttackSwingBase __instance)
        {
            __instance.m_weapon.HitsForDamage = VRMeleeWeapon.sortHits(__instance.m_weapon.HitsForDamage, __instance.m_data.m_damageRef.position);
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
            if (!__instance.Owner.IsLocallyOwned)
            {
                return;
            }

            if(isPush)
            {
                VRMeleeWeaponEvents.HammerSmacked(0f);
            }
            else
            {
                VRMeleeWeaponEvents.HammerSmacked(__instance.m_damageToDeal / __instance.MeleeArchetypeData.ChargedAttackDamage);
            }
        }
    }

    [HarmonyPatch(typeof(MWS_ChargeUp), nameof(MWS_ChargeUp.Enter))]
    internal class InjectHammerChargeEnter
    {
        private static void Prefix(MWS_ChargeUp __instance)
        {
            InjectHammerChargeEvents.fullChargeEventFired = false;
            InjectHammerChargeEvents.halfChargeEventFired = false;
        }
    }

    [HarmonyPatch(typeof(MWS_ChargeUp), nameof(MWS_ChargeUp.Update))]
    internal class InjectHammerChargeEvents
    {
        internal static bool fullChargeEventFired;
        internal static bool halfChargeEventFired;
        private static void Prefix(MWS_ChargeUp __instance)
        {
            float progress = Mathf.Min(__instance.m_elapsed / __instance.m_maxDamageTime, 1f);
            if (progress >= 1 && !fullChargeEventFired)
            {
                VRMeleeWeaponEvents.HammerFullyCharged();
                fullChargeEventFired = true;
            }
            if (progress >= .5f && !halfChargeEventFired)
            {
                VRMeleeWeaponEvents.HammerHalfCharged();
                halfChargeEventFired = true;
            }
        }
    }
}