using System;
using Agents;
using ChainedPuzzles;
using GTFO_VR.Core.PlayerBehaviours.BodyHaptics.Bhaptics;
using GTFO_VR.Core.PlayerBehaviours.BodyHaptics.Shockwave;
using GTFO_VR.Events;
using Il2CppSystem.Collections.Generic;
using Player;
using UnityEngine;

namespace GTFO_VR.Core.PlayerBehaviours.BodyHaptics
{
    public class BodyHapticsIntegrator : MonoBehaviour, BodyHapticAgent
    {
        private BhapticsIntegration m_bhapticsIntegration;
        private ShockwaveIntegration m_shockwaveIntegration;

        public BodyHapticsIntegrator(IntPtr value) : base(value)
        {
        }

        public void Setup(LocalPlayerAgent player)
        {

            m_bhapticsIntegration = gameObject.AddComponent<BhapticsIntegration>();
            m_bhapticsIntegration.Setup(player);

            m_shockwaveIntegration = gameObject.AddComponent<ShockwaveIntegration>();
            m_shockwaveIntegration.Setup(player);
        }

        private void Awake()
        {
            PlayerReceivedDamageEvents.OnPlayerTakeDamage += PlayReceiveDamageHaptics;
            PlayerReceivedDamageEvents.OnMineExplosion += MineExplosionHaptics;
            TentacleAttackEvents.OnTentacleAttack += TentacleAttackHaptics;
            PlayerFireWeaponEvents.OnPlayerFireWeapon += PlayWeaponFireHaptics;
            PlayerReloadEvents.OnPlayerReloaded += PlayWeaponReloadedHaptics;
            PlayerTriggerReloadEvents.OnTriggerWeaponReloaded += PlayTriggerWeaponReloadHaptics;
            HeldItemEvents.OnItemCharging += HammerChargingHaptics;
            VRMeleeWeaponEvents.OnHammerSmack += HammerSmackHaptics;
            VRMeleeWeaponEvents.OnHammerFullyCharged += HammerFullyChargedHaptics;
            FocusStateEvents.OnFocusStateChange += FocusStateChangedHaptics;
            PlayerInteractionEvents.OnPlayerInteracted += PlayerInteractedHaptics;
            PlayerInteractionEvents.OnBioscanSetState += PlayerBioscanSetStateHaptics;
            ItemEquippableEvents.OnPlayerWieldItem += PlayerChangedItemHaptics;
            ResourceUpdatedEvents.OnAmmoGained += AmmoGainedHaptics;
            ResourceUpdatedEvents.OnInfectionUpdated += InfectionUpdatedHaptics;
            ResourceUpdatedEvents.OnHealthUpdated += OnHealthUpdated;
            InventoryAmmoEvents.OnInventoryAmmoUpdate += OnAmmoUpdate;
            PlayerLocomotionEvents.OnStateChange += OnPlayerLocomotionStateChanged;
        }

        private void OnDestroy()
        {
            PlayerReceivedDamageEvents.OnPlayerTakeDamage -= PlayReceiveDamageHaptics;
            PlayerReceivedDamageEvents.OnMineExplosion -= MineExplosionHaptics;
            TentacleAttackEvents.OnTentacleAttack -= TentacleAttackHaptics;
            PlayerFireWeaponEvents.OnPlayerFireWeapon -= PlayWeaponFireHaptics;
            PlayerReloadEvents.OnPlayerReloaded -= PlayWeaponReloadedHaptics;
            PlayerTriggerReloadEvents.OnTriggerWeaponReloaded -= PlayTriggerWeaponReloadHaptics;
            HeldItemEvents.OnItemCharging -= HammerChargingHaptics;
            VRMeleeWeaponEvents.OnHammerSmack -= HammerSmackHaptics;
            VRMeleeWeaponEvents.OnHammerFullyCharged -= HammerFullyChargedHaptics;
            FocusStateEvents.OnFocusStateChange -= FocusStateChangedHaptics;
            PlayerInteractionEvents.OnPlayerInteracted -= PlayerInteractedHaptics;
            PlayerInteractionEvents.OnBioscanSetState -= PlayerBioscanSetStateHaptics;
            ItemEquippableEvents.OnPlayerWieldItem -= PlayerChangedItemHaptics;
            ResourceUpdatedEvents.OnAmmoGained -= AmmoGainedHaptics;
            ResourceUpdatedEvents.OnInfectionUpdated -= InfectionUpdatedHaptics;
            ResourceUpdatedEvents.OnHealthUpdated -= OnHealthUpdated;
            InventoryAmmoEvents.OnInventoryAmmoUpdate -= OnAmmoUpdate;
            PlayerLocomotionEvents.OnStateChange -= OnPlayerLocomotionStateChanged;
        }

        private BodyHapticAgent[] GetAgents()
        {
            return new BodyHapticAgent[] { m_bhapticsIntegration, m_shockwaveIntegration };
        }

        public void HammerSmackHaptics(float dmg)
        {
            foreach (BodyHapticAgent agent in GetAgents())
            {
                agent.HammerSmackHaptics(dmg);
            }
        }

        public void HammerFullyChargedHaptics()
        {
            foreach (BodyHapticAgent agent in GetAgents())
            {
                agent.HammerFullyChargedHaptics();
            }
        }

        public void HammerChargingHaptics(float pressure)
        {
            foreach (BodyHapticAgent agent in GetAgents())
            {
                agent.HammerChargingHaptics(pressure);
            }
        }

        public void PlayWeaponReloadedHaptics()
        {
            foreach (BodyHapticAgent agent in GetAgents())
            {
                agent.PlayWeaponReloadedHaptics();
            }
        }

        public void StopWeaponReloadHaptics()
        {
            foreach (BodyHapticAgent agent in GetAgents())
            {
                agent.StopWeaponReloadHaptics();
            }
        }

        public void PlayTriggerWeaponReloadHaptics()
        {
            foreach (BodyHapticAgent agent in GetAgents())
            {
                agent.PlayTriggerWeaponReloadHaptics();
            }
        }

        public void PlayWeaponFireHaptics(Weapon weapon)
        {
            foreach (BodyHapticAgent agent in GetAgents())
            {
                agent.PlayWeaponFireHaptics(weapon);
            }
        }

        public void PlayReceiveDamageHaptics(float dmg, Vector3 direction)
        {
            foreach (BodyHapticAgent agent in GetAgents())
            {
                agent.PlayReceiveDamageHaptics(dmg, direction);
            }
        }

        public void MineExplosionHaptics(Vector3 explosionPosition)
        {
        }

        public void TentacleAttackHaptics(float dmg, Agent sourceAgent, Vector3 position)
        {
            foreach (BodyHapticAgent agent in GetAgents())
            {
                agent.TentacleAttackHaptics(dmg, sourceAgent, position);
            }
        }

        public void FocusStateChangedHaptics(eFocusState focusState)
        {
            foreach (BodyHapticAgent agent in GetAgents())
            {
                agent.FocusStateChangedHaptics(focusState);
            }
        }

        public void PlayerInteractedHaptics(PlayerAgent source)
        {
            foreach (BodyHapticAgent agent in GetAgents())
            {
                agent.PlayerInteractedHaptics(source);
            }
        }

        public void PlayerBioscanSetStateHaptics(eBioscanStatus status, float progress, List<PlayerAgent> playersInScan)
        {
            foreach (BodyHapticAgent agent in GetAgents())
			{
                agent.PlayerBioscanSetStateHaptics(status, progress, playersInScan);
			}
        }

        public void FlashlightToggledHaptics()
        {
            foreach (BodyHapticAgent agent in GetAgents())
			{
                agent.FlashlightToggledHaptics();
			}
        }

        public void PlayerChangedItemHaptics(ItemEquippable item)
        {
            foreach (BodyHapticAgent agent in GetAgents())
			{
                agent.PlayerChangedItemHaptics(item);
			}
        }

        public void AmmoGainedHaptics(float ammoStandardRel, float ammoSpecialRel, float ammoClassRel)
        {
            foreach (BodyHapticAgent agent in GetAgents())
			{
                agent.AmmoGainedHaptics(ammoStandardRel, ammoSpecialRel, ammoClassRel);
			}
        }

        public void InfectionUpdatedHaptics(float infection)
        {
            foreach (BodyHapticAgent agent in GetAgents())
			{
                agent.InfectionUpdatedHaptics(infection);
			}
        }

        public void OnHealthUpdated(float health)
        {
            foreach (BodyHapticAgent agent in GetAgents())
			{
                agent.OnHealthUpdated(health);
			}
        }

        public void OnAmmoUpdate(InventorySlotAmmo item, int clipleft)
        {
        }

        public void OnPlayerLocomotionStateChanged(PlayerLocomotion.PLOC_State state)
        {
            foreach (BodyHapticAgent agent in GetAgents())
			{
                agent.OnPlayerLocomotionStateChanged(state);
			}
        }

        public void CrouchToggleHaptics(bool isCrouched)
        {
            foreach (BodyHapticAgent agent in GetAgents())
			{
                agent.CrouchToggleHaptics(isCrouched);
			}
        }
    }
}