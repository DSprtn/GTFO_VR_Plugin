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
    public class VRHammer : MonoBehaviour
    {
        public VRHammer(IntPtr value) : base(value)
        {
        }

        public static float hammerSizeMult = .63f;

        private MeleeWeaponFirstPerson m_weapon;
        private Transform m_animatorRoot;
        private Light m_chargeupIndicatorLight;

        public void Setup(MeleeWeaponFirstPerson weapon)
        {
            m_weapon = weapon;
            m_animatorRoot = m_weapon.ModelData.m_damageRefAttack.parent;
            m_chargeupIndicatorLight = new GameObject("VR_Hammer_Chargeup_Light").AddComponent<Light>();
            m_chargeupIndicatorLight.color = Color.white;
            m_chargeupIndicatorLight.range = 1.5f;
            m_chargeupIndicatorLight.intensity = 1.5f;
            m_chargeupIndicatorLight.enabled = false;
            m_chargeupIndicatorLight.shadows = LightShadows.None;
            HammerEvents.OnHammerFullyCharged += HammerFullyCharged;
        }

        private void HammerFullyCharged()
        {
            if (!VRConfig.configUseVisualHammerIndicator.Value)
            {
                return;
            }

            if (m_chargeupIndicatorLight != null)
            {
                m_chargeupIndicatorLight.enabled = true;
            }
            Invoke(nameof(VRHammer.TurnChargeLightOff), 0.15f);
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
                    float YOffset = .68f;
                    float zOffset = .45f;
                    m_weapon.ModelData.m_damageRefAttack.transform.position = Controllers.mainController.transform.TransformPoint(new Vector3(0, YOffset, zOffset));
                }
            }
        }

        private void LateUpdate()
        {
            if (VRConfig.configUseOldHammer.Value || !VRConfig.configUseControllers.Value)
            {
                return;
            }

            if (GTFO_VR_Plugin.DEBUG_ENABLED)
            {
                m_weapon._DEBUG_TARGETING_ENABLED_k__BackingField = VRConfig.configDebugShowHammerHitbox.Value;
                m_weapon._DEBUG_TARGETING_ENABLED_k__BackingField = VRConfig.configDebugShowHammerHitbox.Value;

                hammerSizeMult = VRConfig.configDebugHammersizeMult.Value;
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
            HammerEvents.OnHammerFullyCharged -= HammerFullyCharged;
        }
    }
}