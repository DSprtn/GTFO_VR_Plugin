using ChainedPuzzles;
using Il2CppSystem.Collections.Generic;
using Player;
using UnityEngine;

namespace GTFO_VR.Core.PlayerBehaviours.BodyHaptics
{
    public interface BodyHapticAgent
    {
        void HammerSmackHaptics(float dmg);
        void HammerFullyChargedHaptics();
        void HammerChargingHaptics(float pressure);
        void PlayWeaponReloadedHaptics();
        void StopWeaponReloadHaptics();
        void PlayTriggerWeaponReloadHaptics();
        void PlayWeaponFireHaptics(Weapon weapon);
        void PlayReceiveDamageHaptics(float dmg, Vector3 direction);
        void MineExplosionHaptics(Vector3 explosionPosition);
        void TentacleAttackHaptics(float dmg, Agents.Agent sourceAgent, Vector3 position);
        void FocusStateChangedHaptics(eFocusState focusState);
        void PlayerInteractedHaptics(PlayerAgent source);
        void PlayerBioscanSetStateHaptics(eBioscanStatus status, float progress, List<PlayerAgent> playersInScan);
        void FlashlightToggledHaptics();
        void PlayerChangedItemHaptics(ItemEquippable item);
        void AmmoGainedHaptics(float ammoStandardRel, float ammoSpecialRel, float ammoClassRel);
        void InfectionUpdatedHaptics(float infection);
        void OnHealthUpdated(float health);
        void OnAmmoUpdate(InventorySlotAmmo item, int clipleft);
        void OnPlayerLocomotionStateChanged(PlayerLocomotion.PLOC_State state);
        void CrouchToggleHaptics(bool isCrouched);
    }
}