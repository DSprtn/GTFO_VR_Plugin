using System;
using Agents;
using ChainedPuzzles;
using GTFO_VR.Core.PlayerBehaviours.BodyHaptics.Bhaptics;
using GTFO_VR.Core.PlayerBehaviours.BodyHaptics.Shockwave;
using GTFO_VR.Core.VR_Input;
using GTFO_VR.Events;
using Il2CppSystem.Collections.Generic;
using Player;
using UnityEngine;

namespace GTFO_VR.Core.PlayerBehaviours.BodyHaptics
{
    public class BodyHapticsIntegrator : MonoBehaviour
    {
        private LocalPlayerAgent m_player;
        private BhapticsIntegration m_bhapticsIntegration;
        private ShockwaveIntegration m_shockwaveIntegration;

        private bool m_lastFlashlightEnabledState;
        private bool m_lastIsCrouchedPhysically;
        private float m_lastInfection;

        private static readonly float MIN_DISINFECTION_GAIN_FOR_HAPTIC = 0.05f;

        public BodyHapticsIntegrator(IntPtr value) : base(value)
        {
        }

        public void Setup(LocalPlayerAgent player)
        {
            m_player = player;
            m_lastFlashlightEnabledState = player.Inventory.FlashlightEnabled;

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
            PlayerTriggerReloadEvents.OnTriggerWeaponReloaded += PlayWeaponReloadHaptics;
            PlayerReloadEvents.OnPlayerReloaded += StopWeaponReloadHaptics;
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
            PlayerTriggerReloadEvents.OnTriggerWeaponReloaded -= PlayWeaponReloadHaptics;
            PlayerReloadEvents.OnPlayerReloaded -= StopWeaponReloadHaptics;
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

        private void FixedUpdate()
        {
            if (m_lastFlashlightEnabledState != m_player.Inventory.FlashlightEnabled)
            {
                FlashlightToggledHaptics();
                m_lastFlashlightEnabledState = m_player.Inventory.FlashlightEnabled;
            }

            bool isCrouchedPhysically = IsCrouchedPhysically();
            if (m_lastIsCrouchedPhysically != isCrouchedPhysically)
            {
                CrouchToggleHaptics(isCrouchedPhysically);
                m_lastIsCrouchedPhysically = isCrouchedPhysically;
            }
        }

        private bool IsCrouchedPhysically()
        {
            return HMD.Hmd.transform.localPosition.y + VRConfig.configFloorOffset.Value / 100f < VRConfig.configCrouchHeight.Value / 100f;
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

        public void PlayWeaponReloadHaptics()
        {
            foreach (BodyHapticAgent agent in GetAgents())
            {
                agent.PlayWeaponReloadHaptics();
            }
        }

        public void StopWeaponReloadHaptics()
        {
            foreach (BodyHapticAgent agent in GetAgents())
            {
                agent.StopWeaponReloadHaptics();
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
            const float MAX_DISTANCE = 30f;
            Vector3 playerPosition = m_player.transform.position;
            playerPosition.y = 1f; // for directional haptic (a mine height of 1 will hit horizontally)
            Vector3 direction = playerPosition - explosionPosition;
            float distance = direction.magnitude;

            if (distance < MAX_DISTANCE)
            {
                OrientationSettings orientationSettings = BodyHapticsUtils.GetOrientationSettingsFromDirection(m_player, direction);
                float intensity = 1 - (Math.Max(0, distance - 5) / MAX_DISTANCE);

                foreach (BodyHapticAgent agent in GetAgents())
                {
                    agent.MineExplosionHaptics(orientationSettings, intensity);
                }
            }
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
            Log.Debug($"Bioscan state: {status}, progress: {progress}, playersCount: {playersInScan.Count}, hasLocalPlayer? {playersInScan.Contains(m_player)}");

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
            if (m_lastInfection - infection > MIN_DISINFECTION_GAIN_FOR_HAPTIC) // Gained some disinfection
            {
                foreach (BodyHapticAgent agent in GetAgents())
                {
                    agent.InfectionHealed(infection);
                }
            }

            m_lastInfection = infection;
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
            if (ItemEquippableEvents.IsCurrentItemShootableWeapon() &&
                ItemEquippableEvents.currentItem.ItemDataBlock.inventorySlot.Equals(item.Slot))
            {
                AmmoType ammoType = item.AmmoType;
                if (ammoType == AmmoType.Standard || ammoType == AmmoType.Special)
                {
                    if (clipleft == 0)
                    {
                        foreach (BodyHapticAgent agent in GetAgents())
                        {
                            agent.WeaponAmmoEmpty(Controllers.mainControllerType == HandType.Left);
                        }
                    }
                }
            }
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