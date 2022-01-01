using Agents;
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

            if (Controllers.mainControllerPose.GetVelocity().magnitude > 0.5f)
            {
               
                Collider[] enemyColliders = Physics.OverlapSphere(__instance.m_weapon.ModelData.m_damageRefAttack.position, VRMeleeWeapon.hammerSizeMult * .75f, LayerManager.MASK_ENEMY_DAMAGABLE);
                bool shouldReleaseCharge = enemyColliders.Length > 0;
                if (GTFO_VR_Plugin.DEBUG_ENABLED)
                {
                    if(VRConfig.configDebugShowHammerHitbox.Value)
                    {
                        DebugDraw3D.DrawSphere(__instance.m_weapon.ModelData.m_damageRefAttack.position, VRMeleeWeapon.hammerSizeMult * .75f, ColorExt.Blue(0.2f));
                        DebugDraw3D.DrawSphere(__instance.m_weapon.ModelData.m_damageRefAttack.position, VRMeleeWeapon.hammerSizeMult * .25f, ColorExt.Red(0.2f));
                    }
                }
                if(Controllers.mainControllerPose.GetVelocity().magnitude > 1.6f) {
                    Collider[] staticColliders = Physics.OverlapSphere(__instance.m_weapon.ModelData.m_damageRefAttack.position, VRMeleeWeapon.hammerSizeMult * .25f, LayerManager.MASK_MELEE_ATTACK_TARGETS_WITH_STATIC);
                    shouldReleaseCharge = shouldReleaseCharge || staticColliders.Length > 0;
                }

                if (shouldReleaseCharge)
                {
                    __instance.OnChargeupRelease();
                }
            }
        }
    }


    /// <summary>
    /// Sort hits to prioritize them based on proximity to hammer center (only first enemy hit counts)
    /// </summary>
    [HarmonyPatch(typeof(MWS_AttackSwingBase), nameof(MWS_AttackSwingBase.OnAttackHit))]
    static class InjectPrioritizeProximityTargetsOnHit
    {

        static void Postfix(MWS_AttackSwingBase __instance)
        {
            var hits = __instance.m_weapon.HitsForDamage;

            List<MeleeWeaponDamageData> sortedHits = new List<MeleeWeaponDamageData>();
            
            Vector3 damageRefPos = __instance.m_data.m_damageRef.position;

            
            while(hits.Count > 0)
            {
                float lowest = 999999f;
                MeleeWeaponDamageData closestData = null;
                foreach(var hit in hits)
                {
                    float sqrDst = (hit.hitPos - damageRefPos).sqrMagnitude;
                    if (sqrDst <= lowest)
                    {
                        closestData = hit;
                        lowest = sqrDst;
                    }
                }
                sortedHits.Add(closestData);
                hits.Remove(closestData);
            }
            __instance.m_weapon.HitsForDamage = sortedHits;
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
            Vector3 velocity = Controllers.mainControllerPose.GetVelocity() * 3f;
            data.sourcePos = data.hitPos - data.hitNormal * velocity.magnitude;
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

    /// <summary>
    /// Enable door smacking on VR hammer
    /// </summary>
    [HarmonyPatch(typeof(MeleeWeaponFirstPerson), nameof(MeleeWeaponFirstPerson.DoAttackDamage))]
    static class InjectVRHammerSmackDoors
    {
        static void Prefix(MeleeWeaponFirstPerson __instance)
        {
            if(!__instance.Owner.IsLocallyOwned)
            {
                return;
            }
            if (__instance.Owner.FPSCamera.CameraRayDist <= 3f && __instance.Owner.FPSCamera.CameraRayObject != null && __instance.Owner.FPSCamera.CameraRayObject.layer == LayerManager.LAYER_DYNAMIC)
            {
                iLG_WeakDoor_Destruction componentInParent = __instance.Owner.FPSCamera.CameraRayObject.GetComponentInParent<iLG_WeakDoor_Destruction>();
                if (componentInParent != null && !componentInParent.SkinnedDoorEnabled)
                {
                    componentInParent.EnableSkinnedDoor();
                }
            }
        }
    }

}