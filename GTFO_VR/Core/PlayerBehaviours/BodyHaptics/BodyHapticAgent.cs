using Player;
using UnityEngine;

namespace GTFO_VR.Core.PlayerBehaviours.BodyHaptics
{
    public interface BodyHapticAgent
    {
        void Update();
        void HammerSmackHaptics(float dmg);
        void HammerFullyChargedHaptics();
        void HammerChargingHaptics(float pressure);
        void PlayWeaponReloadHaptics();
        void StopWeaponReloadHaptics();
        void PlayWeaponFireHaptics(Weapon weapon);
        void PlayReceiveDamageHaptics(float dmg, Vector3 direction);
        void MineExplosionHaptics(OrientationSettings orientationSettings, float intensity);
        void TentacleAttackHaptics(float dmg, Agents.Agent sourceAgent, Vector3 position);
        void LandedFromElevator(eFocusState focusState);
        void PlayerInteractedHaptics(PlayerAgent source);
        void PlayBioscanHaptics();
        void StopBioscanHaptics();
        void FlashlightToggledHaptics();
        void PlayerChangedItemHaptics(ItemEquippable item);
        void AmmoGainedHaptics(float ammoStandardRel, float ammoSpecialRel, float ammoClassRel);
        void InfectionHealed(float infection);
        void OnHealthUpdated(float health);
        void WeaponAmmoEmpty(bool leftArm);
        void OnPlayerLocomotionStateChanged(PlayerLocomotion.PLOC_State state);
        void CrouchToggleHaptics(bool isCrouched);
    }
}