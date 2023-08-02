using Gear;
using GTFO_VR.Core.PlayerBehaviours.Melee;
using GTFO_VR.Core.VR_Input;
using GTFO_VR.Events;
using GTFO_VR.Util;
using Il2CppSystem.Collections.Generic;
using System;
using UnityEngine;

namespace GTFO_VR.Core.PlayerBehaviours
{
    /// <summary>
    /// Disable animations, tweak hitbox size and position
    /// </summary>
    public class VRMeleeWeapon : MonoBehaviour
    {
        public VRMeleeWeapon(IntPtr value) : base(value)
        {
        }

        public static VRMeleeWeapon Current;

        public static float WeaponHitboxSize = .61f;
        public static float WeaponHitDetectionSphereCollisionSize = .61f;
        public MeleeWeaponDamageData m_cachedHit = null;

        public VelocityTracker m_positionTracker = new VelocityTracker();
        private MeleeWeaponFirstPerson m_weapon;
        private Transform m_animatorRoot;
        private Light m_chargeupIndicatorLight;

        public Quaternion m_rotationOffset = Quaternion.EulerAngles(new Vector3(0.78f, 0, 0)); // Weapon up is about 45 degrees off
        public Vector3 m_offset = new Vector3(0, 0, .6f);

        public void Setup(MeleeWeaponFirstPerson weapon)
        {
#if DEBUG_GTFO_VR
            MeleeWeaponFirstPerson.DEBUG_ENABLED = VRConfig.configDebugShowHammerHitbox.Value;
            VRConfig.configDebugShowHammerHitbox.SettingChanged += ToggleDebug;
#endif
            Current = this;

            m_weapon = weapon;
            m_animatorRoot = m_weapon.ModelData.m_damageRefAttack.parent;
            m_chargeupIndicatorLight = new GameObject("VR_Weapon_Chargeup_Light").AddComponent<Light>();
            m_chargeupIndicatorLight.color = Color.white;

            m_chargeupIndicatorLight.enabled = false;
            m_chargeupIndicatorLight.shadows = LightShadows.None;
            VRMeleeWeaponEvents.OnHammerFullyCharged += WeaponFullyCharged;
            VRMeleeWeaponEvents.OnHammerHalfCharged += WeaponHalfCharged;

            switch (weapon.ArchetypeName)
            {
                case "Spear":
                    m_offset = m_rotationOffset * new Vector3(0, 1.25f, 0f );
                    WeaponHitDetectionSphereCollisionSize = 0.2f;
                    WeaponHitboxSize = .25f;
                    break;
                case "Knife":
                    m_offset = m_rotationOffset * new Vector3(0, 0.35f, 0.01f);
                    WeaponHitDetectionSphereCollisionSize = 0.22f;
                    WeaponHitboxSize = .22f;
                    break;
                case "Bat":
                    WeaponHitDetectionSphereCollisionSize = 0.35f;
                    WeaponHitboxSize = .45f;
                    m_offset = m_rotationOffset * new Vector3(0, 0.49f, 0f);
                    break;
                case "Sledgehammer":
                    WeaponHitDetectionSphereCollisionSize = .61f;
                    m_offset = m_rotationOffset * new Vector3(0, 0.74f, 0.13f);
                    break;
                default:
                    Log.Error($"Unknown melee weapon detected {weapon.name}");
                    return;
            }
        }

        private void ToggleDebug(object sender, EventArgs e)
        {
            MeleeWeaponFirstPerson.DEBUG_ENABLED = VRConfig.configDebugShowHammerHitbox.Value;
        }

        private void WeaponHalfCharged()
        {
            if (!VRConfig.configUseVisualHammerIndicator.Value)
            {
                return;
            }

            if (m_chargeupIndicatorLight != null)
            {
                m_chargeupIndicatorLight.range = 1.75f;
                m_chargeupIndicatorLight.intensity = .75f;
                m_chargeupIndicatorLight.enabled = true;
            }
            Invoke(nameof(VRMeleeWeapon.TurnChargeLightOff), 0.10f);
        }

        private void WeaponFullyCharged()
        {
            if (!VRConfig.configUseVisualHammerIndicator.Value)
            {
                return;
            }

            if (m_chargeupIndicatorLight != null)
            {
                m_chargeupIndicatorLight.range = 1.75f;
                m_chargeupIndicatorLight.intensity = 1.75f;
                m_chargeupIndicatorLight.enabled = true;
            }
            Invoke(nameof(VRMeleeWeapon.TurnChargeLightOff), 0.15f);
        }

        private void TurnChargeLightOff()
        {
            m_chargeupIndicatorLight.enabled = false;
        }

        private void Update()
        {
            if (VRConfig.configUseOldHammer.Value || !VRConfig.configUseControllers.Value)
            {
                return;
            }
            if (m_weapon.Owner && m_weapon.Owner.IsLocallyOwned)
            {
                ForceDamageRefPosition();
                m_chargeupIndicatorLight.transform.position = m_weapon.ModelData.m_damageRefAttack.transform.position;
                TrackPositionAndVelocity();
            }
        }

        private void ForceDamageRefPosition()
        {
            if (VRConfig.configUseControllers.Value && !VRConfig.configUseOldHammer.Value)
            {
                if (m_weapon.ModelData != null)
                {
                    m_weapon.ModelData.m_damageRefAttack.transform.position = Controllers.MainController.transform.TransformPoint( m_offset);
                }
            }
        }

        public void TrackPositionAndVelocity()
        {
            if (m_weapon.ModelData != null)
            {
                // Position of melee damage ref is used to calculate velocity.
                // This must be done in local space as the units are tiny and rounding errors throw off the velocity otherwise.
                // This is basically a reapeat of ForceDamageRefPosition() but in local space of the controller

                // Global position. Used for accurate hit detection.
                Vector3 damageRefPosition = m_weapon.ModelData.m_damageRefAttack.position;
                // Local position. Used for accurate velocity calculation
                Vector3 localPosition = Controllers.MainControllerPose.transform.localPosition;     
                localPosition = localPosition + (Controllers.MainControllerPose.transform.rotation * m_offset); 

                m_positionTracker.AddPosition(damageRefPosition, localPosition, Time.deltaTime);
            }
        }

        private void LateUpdate()
        {
            if (VRConfig.configUseOldHammer.Value || !VRConfig.configUseControllers.Value)
            {
                return;
            }

            if (m_weapon.Owner && m_weapon.Owner.IsLocallyOwned)
            {
                if (FocusStateEvents.currentState == eFocusState.FPS)
                {
                    m_animatorRoot.transform.localPosition = Vector3.zero;
                    m_animatorRoot.transform.localRotation = Quaternion.identity;
                }
            }
        }

        public static List<MeleeWeaponDamageData> sortHits(List<MeleeWeaponDamageData> hits, Vector3 damageRefPos)
        {
            List<MeleeWeaponDamageData> sortedHits = new List<MeleeWeaponDamageData>();

            while (hits.Count > 0)
            {
                float lowest = 999999f;
                MeleeWeaponDamageData closestData = null;
                foreach (var hit in hits)
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

            return sortedHits;
        }

        public MeleeWeaponDamageData CheckForAttackTarget()
        {
            m_cachedHit = null;

            Vector3 weaponPosCurrent = m_positionTracker.getLatestPosition();
            Vector3 weaponPosPrev = m_positionTracker.getPreviousPosition();

            DebugDraw3D.DrawCone(weaponPosCurrent, weaponPosPrev, VRMeleeWeapon.WeaponHitDetectionSphereCollisionSize * 0.03f, ColorExt.Blue(0.5f), 0.5f);

            Vector3 velocity = (weaponPosCurrent - weaponPosPrev);

            RaycastHit rayHit;
            // cast a sphere from where the the hitbox was, to where it is, and get the first thing it collides with along the way
            if (Physics.SphereCast(weaponPosPrev, VRMeleeWeapon.WeaponHitDetectionSphereCollisionSize * 0.1f, velocity.normalized, out rayHit, velocity.magnitude, LayerManager.MASK_MELEE_ATTACK_TARGETS_WITH_STATIC, QueryTriggerInteraction.Ignore))
            {
                m_cachedHit = new MeleeWeaponDamageData
                {
                    damageGO = rayHit.collider.gameObject,
                    hitPos = rayHit.point, // vector from sourcePos to hitPos, and source to enemy spine used to determine backstab
                    hitNormal = rayHit.normal, // Only used for gore
                    sourcePos = m_weapon.Owner.FPSCamera.Position,
                    damageTargetFound = true // Not actually used for anything
                };

                // non-statics things we can hit should have an IDamageable
                IDamageable damageable = rayHit.collider.GetComponent<IDamageable>();
                if (damageable != null)
                {
                    m_cachedHit.damageComp = damageable;

                    // SearchID is incremented at the beginning of the original CheckForAttackTarget().
                    // Predict what the next value will be and assign it instead, so it will ignore this collider.
                    // This assumes CheckForAttackTarget() will be called soon after this function returns
                    uint searchID = DamageUtil.SearchID;
                    if (searchID < uint.MaxValue)
                    { searchID++; }
                    else
                    { searchID = 1u; }

                    damageable.GetBaseDamagable().TempSearchID = searchID;
                }

#if DEBUG_GTFO_VR
            VRConfig.configDebugShowHammerHitbox.SettingChanged -= ToggleDebug;
#endif
                }
            }

            return m_cachedHit;
        }


        private void OnDestroy()
        {
            VRMeleeWeaponEvents.OnHammerHalfCharged -= WeaponHalfCharged;
            VRMeleeWeaponEvents.OnHammerFullyCharged -= WeaponFullyCharged;
        }
    }
}