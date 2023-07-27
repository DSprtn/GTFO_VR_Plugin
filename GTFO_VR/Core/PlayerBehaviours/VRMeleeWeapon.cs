using Gear;
using GTFO_VR.Core.PlayerBehaviours.Melee;
using GTFO_VR.Core.UI.Terminal.Pointer;
using GTFO_VR.Core.VR_Input;
using GTFO_VR.Events;
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
        public MeleeTracker VelocityTracker = new MeleeTracker();

        private MeleeWeaponFirstPerson m_weapon;
        private Transform m_animatorRoot;
        private Light m_chargeupIndicatorLight;

        public Vector3 m_offset = new Vector3(0, .68f, .45f);

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

            Vector3 baseOffset = new Vector3(0, .68f, .45f);
            switch (weapon.ArchetypeName)
            {
                case "Spear":
                    m_offset = baseOffset * 1.5f;
                    WeaponHitDetectionSphereCollisionSize = 0.2f;
                    WeaponHitboxSize = .25f;
                    break;
                case "Knife":
                    m_offset = baseOffset * .35f;
                    WeaponHitDetectionSphereCollisionSize = 0.22f;
                    WeaponHitboxSize = .22f;
                    break;
                case "Bat":
                    WeaponHitDetectionSphereCollisionSize = 0.35f;
                    WeaponHitboxSize = .45f;
                    m_offset = baseOffset * .7f;
                    break;
                case "Sledgehammer":
                    WeaponHitDetectionSphereCollisionSize = .61f;
                    m_offset = baseOffset;
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
                    m_weapon.ModelData.m_damageRefAttack.transform.position = Controllers.MainController.transform.TransformPoint(new Vector3(0, m_offset.y, m_offset.z));
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

                VelocityTracker.AddPosition(damageRefPosition, localPosition, Time.deltaTime);
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

        private void OnDestroy()
        {
#if DEBUG_GTFO_VR
            VRConfig.configDebugShowHammerHitbox.SettingChanged -= ToggleDebug;
#endif
            VRMeleeWeaponEvents.OnHammerHalfCharged -= WeaponHalfCharged;
            VRMeleeWeaponEvents.OnHammerFullyCharged -= WeaponFullyCharged;
        }
    }
}