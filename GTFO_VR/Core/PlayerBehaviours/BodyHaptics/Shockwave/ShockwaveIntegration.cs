using System;
using System.Collections.Generic;
using Agents;
using ChainedPuzzles;
using GTFO_VR.Core.VR_Input;
using Player;
using UnityEngine;

namespace GTFO_VR.Core.PlayerBehaviours.BodyHaptics.Shockwave
{
    public class ShockwaveIntegration : MonoBehaviour, BodyHapticAgent
    {
        private LocalPlayerAgent m_player;

        public ShockwaveIntegration(IntPtr value) : base(value)
        {
        }

        public void Setup(LocalPlayerAgent player)
        {
            m_player = player;

            ShockwaveManager.Instance.InitializeSuit();
        }

        public void HammerSmackHaptics(float dmg)
        {
        }

        public void HammerFullyChargedHaptics()
        {
        }

        public void HammerChargingHaptics(float pressure)
        {
        }

        public void PlayWeaponReloadedHaptics()
        {
        }

        public void StopWeaponReloadHaptics()
        {
        }

        public void PlayTriggerWeaponReloadHaptics()
        {
        }

        public void PlayWeaponFireHaptics(Weapon weapon)
        {
        }

        public void PlayReceiveDamageHaptics(float dmg, Vector3 direction)
        {
        }

        public void MineExplosionHaptics(Vector3 explosionPosition)
        {
        }

        public void TentacleAttackHaptics(float dmg, Agent sourceAgent, Vector3 position)
        {
        }

        public void FocusStateChangedHaptics(eFocusState focusState)
        {
        }

        public void PlayerInteractedHaptics(PlayerAgent source)
        {
        }

        public void PlayerBioscanSetStateHaptics(eBioscanStatus status, float progress, Il2CppSystem.Collections.Generic.List<PlayerAgent> playersInScan)
        {
        }

        public void FlashlightToggledHaptics()
        {
        }

        public void PlayerChangedItemHaptics(ItemEquippable item)
        {
        }

        public void AmmoGainedHaptics(float ammoStandardRel, float ammoSpecialRel, float ammoClassRel)
        {
        }

        public void InfectionUpdatedHaptics(float infection)
        {
        }

        public void OnHealthUpdated(float health)
        {
        }

        public void OnAmmoUpdate(InventorySlotAmmo item, int clipleft)
        {
        }

        public void OnPlayerLocomotionStateChanged(PlayerLocomotion.PLOC_State state)
        {
        }

        public void CrouchToggleHaptics(bool isCrouched)
        {
        }
    }
}