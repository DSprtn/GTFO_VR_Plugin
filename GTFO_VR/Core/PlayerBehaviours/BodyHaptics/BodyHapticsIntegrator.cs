using System;
using Agents;
using Bhaptics.Tact;
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
        private int m_bioscanStopFramesCount;
        private bool m_isBioscanning;

        private static readonly float MIN_DISINFECTION_GAIN_FOR_HAPTIC = 0.05f;

        public BodyHapticsIntegrator(IntPtr value) : base(value)
        {
        }

        public void Setup(LocalPlayerAgent player)
        {
            m_player = player;
            m_lastFlashlightEnabledState = player.Inventory.FlashlightEnabled;

            HapticPlayer hapticPlayer = new HapticPlayer();
            m_bhapticsIntegration = new BhapticsIntegration();
            m_bhapticsIntegration.Setup(player, hapticPlayer);

            m_shockwaveIntegration = new ShockwaveIntegration();
            m_shockwaveIntegration.Setup(player);

            var elevatorSequenceIntegrator = gameObject.AddComponent<ElevatorSequenceIntegrator>();
            elevatorSequenceIntegrator.Setup(player, hapticPlayer);
        }

        public static void Initialize()
        {
            ShockwaveManager.Instance.InitializeSuit();
        }

        public static void Destroy()
        {
            ShockwaveManager.Instance.DisconnectSuit();
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

            if (m_bioscanStopFramesCount > 0 && ++m_bioscanStopFramesCount >= 5)
            {
                m_bioscanStopFramesCount = 0;
                m_isBioscanning = false;

                foreach (BodyHapticAgent agent in GetAgents())
                {
                    agent.StopBioscanHaptics();
                }
            }

            foreach (BodyHapticAgent agent in GetAgents())
            {
                agent.Update();
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
            if (FocusStateEvents.lastState.Equals(eFocusState.InElevator) && focusState == eFocusState.FPS)
            {
                foreach (BodyHapticAgent agent in GetAgents())
                {
                    agent.LandedFromElevator(focusState);
                }
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
            if (playersInScan == null)
            {
                return;
            }

            if (status == eBioscanStatus.Scanning && playersInScan.Contains(m_player) && m_player.Alive)
            {
                if (!m_isBioscanning)
                {
                    m_bioscanStopFramesCount = 0;
                    m_isBioscanning = true;

                    foreach (BodyHapticAgent agent in GetAgents())
                    {
                        agent.PlayBioscanHaptics();
                    }
                }
            }
            else if (m_bioscanStopFramesCount == 0 && m_isBioscanning)
            {
                // Indicate that bioscan stopped, and stop haptic pattern only after a few FixedUpdate() calls if we don't receive any other scan activations until then.
                // When multiple players are in different single-person scans, we receive this event every fixed frame for *each* currently scanned player,
                // and m_player is only in a single playersInScan list, so we don't want to stop the scan right when we receive the scan of someone else.
                m_bioscanStopFramesCount = 1;
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
                            agent.WeaponAmmoEmpty(Controllers.MainControllerType == HandType.Left);
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