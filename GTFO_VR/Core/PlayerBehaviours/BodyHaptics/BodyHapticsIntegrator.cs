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
        private List<BodyHapticAgent> m_hapticAgents = new List<BodyHapticAgent>();

        public void Setup(LocalPlayerAgent player)
        {
            BhapticsIntegration bhapticsIntegration = gameObject.AddComponent<BhapticsIntegration>();
            bhapticsIntegration.Setup(player);
            m_hapticAgents.Add(bhapticsIntegration);

            ShockwaveIntegration shockwaveIntegration = gameObject.AddComponent<ShockwaveIntegration>();
            shockwaveIntegration.Setup(player);
            m_hapticAgents.Add(shockwaveIntegration);
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

        private List<BodyHapticAgent> GetAgents()
        {
            return m_hapticAgents;
        }

        private void Execute(Action<BodyHapticAgent> fn)
        {
            foreach (BodyHapticAgent agent in GetAgents())
            {
                fn(agent);
            }
        }

        public void HammerSmackHaptics(float dmg)
        {
            Execute((agent) => agent.HammerSmackHaptics(dmg));
        }

        public void HammerFullyChargedHaptics()
        {
            Execute((agent) => agent.HammerFullyChargedHaptics());
        }

        public void HammerChargingHaptics(float pressure)
        {
            Execute((agent) => agent.HammerChargingHaptics(pressure));
        }

        public void PlayWeaponReloadedHaptics()
        {
            Execute((agent) => agent.PlayWeaponReloadedHaptics());
        }

        public void StopWeaponReloadHaptics()
        {
            Execute((agent) => agent.StopWeaponReloadHaptics());
        }

        public void PlayTriggerWeaponReloadHaptics()
        {
            Execute((agent) => agent.PlayTriggerWeaponReloadHaptics());
        }

        public void PlayWeaponFireHaptics(Weapon weapon)
        {
            Execute((agent) => agent.PlayWeaponFireHaptics(weapon));
        }

        public void PlayReceiveDamageHaptics(float dmg, Vector3 direction)
        {
            Execute((agent) => agent.PlayReceiveDamageHaptics(dmg, direction));
        }

        public void MineExplosionHaptics(Vector3 explosionPosition)
        {
            Execute((agent) => agent.MineExplosionHaptics(explosionPosition));
        }

        public void TentacleAttackHaptics(float dmg, Agent sourceAgent, Vector3 position)
        {
            Execute((agent) => agent.TentacleAttackHaptics(dmg, sourceAgent, position));
        }

        public void FocusStateChangedHaptics(eFocusState focusState)
        {
            Execute((agent) => agent.FocusStateChangedHaptics(focusState));
        }

        public void PlayerInteractedHaptics(PlayerAgent source)
        {
            Execute((agent) => agent.PlayerInteractedHaptics(source));
        }

        public void PlayerBioscanSetStateHaptics(eBioscanStatus status, float progress, List<PlayerAgent> playersInScan)
        {
            Execute((agent) => agent.PlayerBioscanSetStateHaptics(status, progress, playersInScan));
        }

        public void FlashlightToggledHaptics()
        {
            Execute((agent) => agent.FlashlightToggledHaptics());
        }

        public void PlayerChangedItemHaptics(ItemEquippable item)
        {
            Execute((agent) => agent.PlayerChangedItemHaptics(item));
        }

        public void AmmoGainedHaptics(float ammoStandardRel, float ammoSpecialRel, float ammoClassRel)
        {
            Execute((agent) => agent.AmmoGainedHaptics(ammoStandardRel, ammoSpecialRel, ammoClassRel));
        }

        public void InfectionUpdatedHaptics(float infection)
        {
            Execute((agent) => agent.InfectionUpdatedHaptics(infection));
        }

        public void OnHealthUpdated(float health)
        {
            Execute((agent) => agent.OnHealthUpdated(health));
        }

        public void OnAmmoUpdate(InventorySlotAmmo item, int clipleft)
        {
            Execute((agent) => agent.OnAmmoUpdate(item, clipleft));
        }

        public void OnPlayerLocomotionStateChanged(PlayerLocomotion.PLOC_State state)
        {
            Execute((agent) => agent.OnPlayerLocomotionStateChanged(state));
        }

        public void CrouchToggleHaptics(bool isCrouched)
        {
            Execute((agent) => agent.CrouchToggleHaptics(isCrouched));
        }
    }
}