using Gear;
using GTFO_VR.Core.VR_Input;
using GTFO_VR.Events;
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

        public static float WeaponHitDetectionSphereSize = .61f;

        private MeleeWeaponFirstPerson m_weapon;
        private Transform m_animatorRoot;
        private Light m_chargeupIndicatorLight;

        public Vector3 m_offset = new Vector3(0, .68f, .45f);

        public void Setup(MeleeWeaponFirstPerson weapon)
        {

            MeleeWeaponFirstPerson.DEBUG_ENABLED = VRConfig.configDebugShowHammerHitbox.Value;
            VRConfig.configDebugShowHammerHitbox.SettingChanged += ToggleDebug;

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
                    WeaponHitDetectionSphereSize = 0.2f;
                    break;
                case "Knife":
                    m_offset = baseOffset * .25f;
                    WeaponHitDetectionSphereSize = 0.2f;
                    break;
                case "Bat":
                    WeaponHitDetectionSphereSize = 0.35f;
                    m_offset = baseOffset * .7f;
                    break;
                case "Sledgehammer":
                    WeaponHitDetectionSphereSize = .61f;
                    m_offset = baseOffset;
                    break;
                default:
                    Log.Warning($"Unknown melee weapon detected {weapon.name}");
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
            }
        }

        private void ForceDamageRefPosition()
        {
            if (VRConfig.configUseControllers.Value && !VRConfig.configUseOldHammer.Value)
            {
                if (m_weapon.ModelData != null)
                {
                    m_weapon.ModelData.m_damageRefAttack.transform.position = Controllers.mainController.transform.TransformPoint(new Vector3(0, m_offset.y, m_offset.z));
                }
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

        private void OnDestroy()
        {
            VRConfig.configDebugShowHammerHitbox.SettingChanged -= ToggleDebug;
            VRMeleeWeaponEvents.OnHammerHalfCharged -= WeaponHalfCharged;
            VRMeleeWeaponEvents.OnHammerFullyCharged -= WeaponFullyCharged;
        }
    }
}