﻿using System;
using Bhaptics.Tact;
using ChainedPuzzles;
using GTFO_VR.Core.VR_Input;
using GTFO_VR.Events;
using Il2CppSystem.Collections.Generic;
using Player;
using UnityEngine;

namespace GTFO_VR.Core.PlayerBehaviours.BodyHaptics.Bhaptics
{
    public class BhapticsIntegration : MonoBehaviour, BodyHapticAgent
    {
        private static readonly string VEST_DAMAGE_KEY = "vest_damage";
        private static readonly string VEST_TENTACLE_ATTACK_KEY = "vest_tentacle_attack";
        private static readonly string VEST_FIRE_R_KEY = "vest_fire_r";
        private static readonly string VEST_FIRE_L_KEY = "vest_fire_l";
        private static readonly string VEST_RELOAD_R_KEY = "vest_reload_r";
        private static readonly string VEST_RELOAD_L_KEY = "vest_reload_l";
        private static readonly string VEST_HAMMER_CHARGING_R_KEY = "vest_hammer_charging_r";
        private static readonly string VEST_HAMMER_CHARGING_L_KEY = "vest_hammer_charging_l";
        private static readonly string VEST_HAMMER_SMACK_R_KEY = "vest_hammer_smack_r";
        private static readonly string VEST_HAMMER_SMACK_L_KEY = "vest_hammer_smack_l";
        private static readonly string VEST_HAMMER_FULLY_CHARGED_R_KEY = "vest_hammer_fully_charged_r";
        private static readonly string VEST_HAMMER_FULLY_CHARGED_L_KEY = "vest_hammer_fully_charged_l";
        private static readonly string VEST_LANDING_KEY = "vest_landing";
        private static readonly string VEST_LANDING_SMALL_KEY = "vest_landing_small";
        private static readonly string VEST_GAIN_HEALTH_KEY = "vest_gain_health";
        private static readonly string VEST_GAIN_AMMO_KEY = "vest_gain_ammo";
        private static readonly string VEST_GAIN_TOOL_AMMO_KEY = "vest_gain_tool_ammo";
        private static readonly string VEST_GAIN_DISINFECTION_KEY = "vest_gain_disinfection";
        private static readonly string VEST_NEED_HEALTH_KEY = "vest_need_health";
        private static readonly string VEST_DEATH_KEY = "vest_death";
        private static readonly string VEST_CROUCH_KEY = "vest_crouch";
        private static readonly string VEST_STAND_KEY = "vest_stand";
        private static readonly string VEST_BODY_SCAN_KEY = "vest_body_scan";
        private static readonly string VEST_EXPLOSION_KEY = "vest_explosion";

        private static readonly string ARMS_FIRE_R_KEY = "arms_fire_r";
        private static readonly string ARMS_FIRE_L_KEY = "arms_fire_l";
        private static readonly string ARMS_RELOAD_R_KEY = "arms_reload_r";
        private static readonly string ARMS_RELOAD_L_KEY = "arms_reload_l";
        private static readonly string ARMS_HAMMER_CHARGING_R_KEY = "arms_hammer_charging_r";
        private static readonly string ARMS_HAMMER_CHARGING_L_KEY = "arms_hammer_charging_l";
        private static readonly string ARMS_HAMMER_SMACK_R_KEY = "arms_hammer_smack_r";
        private static readonly string ARMS_HAMMER_SMACK_L_KEY = "arms_hammer_smack_l";
        private static readonly string ARMS_HAMMER_FULLY_CHARGED_R_KEY = "arms_hammer_fully_charged_r";
        private static readonly string ARMS_HAMMER_FULLY_CHARGED_L_KEY = "arms_hammer_fully_charged_l";
        private static readonly string ARMS_INTERACT_ITEM_R_KEY = "arms_interact_item_r";
        private static readonly string ARMS_INTERACT_ITEM_L_KEY = "arms_interact_item_l";
        private static readonly string ARMS_FLASHLIGHT_TOGGLE_R_KEY = "arms_flashlight_toggle_r";
        private static readonly string ARMS_FLASHLIGHT_TOGGLE_L_KEY = "arms_flashlight_toggle_l";
        private static readonly string ARMS_CHANGE_ITEM_R_KEY = "arms_change_item_r";
        private static readonly string ARMS_CHANGE_ITEM_L_KEY = "arms_change_item_l";
        private static readonly string ARMS_OUT_OF_AMMO_R_KEY = "arms_out_of_ammo_r";
        private static readonly string ARMS_OUT_OF_AMMO_L_KEY = "arms_out_of_ammo_l";
        private static readonly string ARMS_LANDING_KEY = "arms_landing";
        private static readonly string ARMS_GAIN_AMMO_KEY = "arms_gain_ammo";
        private static readonly string ARMS_GAIN_TOOL_AMMO_KEY = "arms_gain_tool_ammo";
        private static readonly string ARMS_EXPLOSION_KEY = "arms_explosion";

        private LocalPlayerAgent m_player;
        private HapticPlayer m_hapticPlayer;

        private float m_nextReloadHapticPatternTime;
        private float m_nextHeartbeatPatternTime;
        private float m_nextBodyscanPatternTime;
        private float m_lastInfection;
        private float m_lastHealth = 1f;
        private bool m_lastFlashlightEnabledState;
        private RotationOption m_lastDamageRotationOption;
        private PlayerLocomotion.PLOC_State m_lastLocState;
        private bool m_lastIsCrouchedPhysically;
        private int m_bioscanStopFramesCount;

        private static readonly float RELOAD_FEEDBACK_DURATION = 1.0f;
        private static readonly float HEARTBEAT_REPEAT_DELAY = 1.0f;
        private static readonly float BODY_SCAN_REPEAT_DELAY = 13.5f;
        private static readonly float LOW_HEALTH = 0.20f;
        private static readonly float MIN_HEALTH_GAIN_FOR_HAPTIC = 0.05f;
        private static readonly float MIN_DISINFECTION_GAIN_FOR_HAPTIC = 0.05f;

        public BhapticsIntegration(IntPtr value) : base(value)
        {
        }

        public void Setup(LocalPlayerAgent player)
        {
            m_player = player;
            m_lastFlashlightEnabledState = player.Inventory.FlashlightEnabled;
            m_lastLocState = player.Locomotion.m_currentStateEnum;

            m_hapticPlayer = new HapticPlayer();
            BhapticsUtils.RegisterVestTactKey(m_hapticPlayer, VEST_DAMAGE_KEY);
            BhapticsUtils.RegisterVestTactKey(m_hapticPlayer, VEST_TENTACLE_ATTACK_KEY);
            BhapticsUtils.RegisterVestTactKey(m_hapticPlayer, VEST_FIRE_R_KEY);
            BhapticsUtils.RegisterVestTactKey(m_hapticPlayer, VEST_FIRE_L_KEY);
            BhapticsUtils.RegisterVestTactKey(m_hapticPlayer, VEST_RELOAD_R_KEY);
            BhapticsUtils.RegisterVestTactKey(m_hapticPlayer, VEST_RELOAD_L_KEY);
            BhapticsUtils.RegisterVestTactKey(m_hapticPlayer, VEST_HAMMER_CHARGING_R_KEY);
            BhapticsUtils.RegisterVestTactKey(m_hapticPlayer, VEST_HAMMER_CHARGING_L_KEY);
            BhapticsUtils.RegisterVestTactKey(m_hapticPlayer, VEST_HAMMER_FULLY_CHARGED_R_KEY);
            BhapticsUtils.RegisterVestTactKey(m_hapticPlayer, VEST_HAMMER_FULLY_CHARGED_L_KEY);
            BhapticsUtils.RegisterVestTactKey(m_hapticPlayer, VEST_HAMMER_SMACK_R_KEY);
            BhapticsUtils.RegisterVestTactKey(m_hapticPlayer, VEST_HAMMER_SMACK_L_KEY);
            BhapticsUtils.RegisterVestTactKey(m_hapticPlayer, VEST_LANDING_KEY);
            BhapticsUtils.RegisterVestTactKey(m_hapticPlayer, VEST_LANDING_SMALL_KEY);
            BhapticsUtils.RegisterVestTactKey(m_hapticPlayer, VEST_GAIN_HEALTH_KEY);
            BhapticsUtils.RegisterVestTactKey(m_hapticPlayer, VEST_GAIN_AMMO_KEY);
            BhapticsUtils.RegisterVestTactKey(m_hapticPlayer, VEST_GAIN_TOOL_AMMO_KEY);
            BhapticsUtils.RegisterVestTactKey(m_hapticPlayer, VEST_GAIN_DISINFECTION_KEY);
            BhapticsUtils.RegisterVestTactKey(m_hapticPlayer, VEST_NEED_HEALTH_KEY);
            BhapticsUtils.RegisterVestTactKey(m_hapticPlayer, VEST_DEATH_KEY);
            BhapticsUtils.RegisterVestTactKey(m_hapticPlayer, VEST_CROUCH_KEY);
            BhapticsUtils.RegisterVestTactKey(m_hapticPlayer, VEST_STAND_KEY);
            BhapticsUtils.RegisterVestTactKey(m_hapticPlayer, VEST_BODY_SCAN_KEY);
            BhapticsUtils.RegisterVestTactKey(m_hapticPlayer, VEST_EXPLOSION_KEY);

            BhapticsUtils.RegisterArmsTactKey(m_hapticPlayer, ARMS_FIRE_R_KEY);
            BhapticsUtils.RegisterArmsTactKey(m_hapticPlayer, ARMS_FIRE_L_KEY);
            BhapticsUtils.RegisterArmsTactKey(m_hapticPlayer, ARMS_RELOAD_R_KEY);
            BhapticsUtils.RegisterArmsTactKey(m_hapticPlayer, ARMS_RELOAD_L_KEY);
            BhapticsUtils.RegisterArmsTactKey(m_hapticPlayer, ARMS_HAMMER_CHARGING_R_KEY);
            BhapticsUtils.RegisterArmsTactKey(m_hapticPlayer, ARMS_HAMMER_CHARGING_L_KEY);
            BhapticsUtils.RegisterArmsTactKey(m_hapticPlayer, ARMS_HAMMER_FULLY_CHARGED_R_KEY);
            BhapticsUtils.RegisterArmsTactKey(m_hapticPlayer, ARMS_HAMMER_FULLY_CHARGED_L_KEY);
            BhapticsUtils.RegisterArmsTactKey(m_hapticPlayer, ARMS_HAMMER_SMACK_R_KEY);
            BhapticsUtils.RegisterArmsTactKey(m_hapticPlayer, ARMS_HAMMER_SMACK_L_KEY);
            BhapticsUtils.RegisterArmsTactKey(m_hapticPlayer, ARMS_INTERACT_ITEM_R_KEY);
            BhapticsUtils.RegisterArmsTactKey(m_hapticPlayer, ARMS_INTERACT_ITEM_L_KEY);
            BhapticsUtils.RegisterArmsTactKey(m_hapticPlayer, ARMS_FLASHLIGHT_TOGGLE_R_KEY);
            BhapticsUtils.RegisterArmsTactKey(m_hapticPlayer, ARMS_FLASHLIGHT_TOGGLE_L_KEY);
            BhapticsUtils.RegisterArmsTactKey(m_hapticPlayer, ARMS_CHANGE_ITEM_R_KEY);
            BhapticsUtils.RegisterArmsTactKey(m_hapticPlayer, ARMS_CHANGE_ITEM_L_KEY);
            BhapticsUtils.RegisterArmsTactKey(m_hapticPlayer, ARMS_OUT_OF_AMMO_R_KEY);
            BhapticsUtils.RegisterArmsTactKey(m_hapticPlayer, ARMS_OUT_OF_AMMO_L_KEY);
            BhapticsUtils.RegisterArmsTactKey(m_hapticPlayer, ARMS_LANDING_KEY);
            BhapticsUtils.RegisterArmsTactKey(m_hapticPlayer, ARMS_GAIN_AMMO_KEY);
            BhapticsUtils.RegisterArmsTactKey(m_hapticPlayer, ARMS_GAIN_TOOL_AMMO_KEY);
            BhapticsUtils.RegisterArmsTactKey(m_hapticPlayer, ARMS_EXPLOSION_KEY);

            var elevatorSequence = gameObject.AddComponent<BhapticsElevatorSequence>();
            elevatorSequence.Setup(m_player, m_hapticPlayer);
        }

        void FixedUpdate()
        {
            if (VRConfig.configUseBhaptics.Value)
            {
                UpdateBhapticsState();
            }
        }

        private void UpdateBhapticsState()
        {
            float currentTime = Time.time;

            bool isReloading = (m_nextReloadHapticPatternTime > 0);
            if (isReloading && currentTime >= m_nextReloadHapticPatternTime)
            {
                if (Controllers.MainControllerType == HandType.Left)
                {
                    m_hapticPlayer.SubmitRegistered(VEST_RELOAD_L_KEY);
                    m_hapticPlayer.SubmitRegistered(ARMS_RELOAD_L_KEY);
                }
                else
                {
                    m_hapticPlayer.SubmitRegistered(VEST_RELOAD_R_KEY);
                    m_hapticPlayer.SubmitRegistered(ARMS_RELOAD_R_KEY);
                }
                m_nextReloadHapticPatternTime += RELOAD_FEEDBACK_DURATION;
            }

            if (m_nextHeartbeatPatternTime > 0f && currentTime >= m_nextHeartbeatPatternTime)
            {
                m_hapticPlayer.SubmitRegistered(VEST_NEED_HEALTH_KEY);
                m_nextHeartbeatPatternTime += HEARTBEAT_REPEAT_DELAY;
            }
            
            if (m_nextBodyscanPatternTime > 0f && currentTime >= m_nextBodyscanPatternTime)
            {
                m_hapticPlayer.SubmitRegistered(VEST_BODY_SCAN_KEY);
                m_nextBodyscanPatternTime += BODY_SCAN_REPEAT_DELAY;
            }

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
                m_nextBodyscanPatternTime = 0f;
                m_hapticPlayer.TurnOff(VEST_BODY_SCAN_KEY);
                m_bioscanStopFramesCount = 0;
            }
        }

        private bool IsCrouchedPhysically()
        {
            return HMD.Hmd.transform.localPosition.y + VRConfig.configFloorOffset.Value / 100f < VRConfig.configCrouchHeight.Value / 100f;
        }

        public void HammerSmackHaptics(float dmg)
        {
            if (!VRConfig.configUseBhaptics.Value)
            {
                return;
            }

			if (Controllers.MainControllerType == HandType.Left)
			{
				m_hapticPlayer.SubmitRegistered(VEST_HAMMER_SMACK_L_KEY);
				m_hapticPlayer.SubmitRegistered(ARMS_HAMMER_SMACK_L_KEY);
			}
			else
			{
				m_hapticPlayer.SubmitRegistered(VEST_HAMMER_SMACK_R_KEY);
				m_hapticPlayer.SubmitRegistered(ARMS_HAMMER_SMACK_R_KEY);
			}
        }

        public void HammerFullyChargedHaptics()
        {
            if (!VRConfig.configUseBhaptics.Value)
            {
                return;
            }

			if (Controllers.MainControllerType == HandType.Left)
			{
				m_hapticPlayer.SubmitRegistered(VEST_HAMMER_FULLY_CHARGED_L_KEY);
				m_hapticPlayer.SubmitRegistered(ARMS_HAMMER_FULLY_CHARGED_L_KEY);
			}
			else
			{
				m_hapticPlayer.SubmitRegistered(VEST_HAMMER_FULLY_CHARGED_R_KEY);
				m_hapticPlayer.SubmitRegistered(ARMS_HAMMER_FULLY_CHARGED_R_KEY);
			}
        }

        public void HammerChargingHaptics(float pressure)
        {
            if (!VRConfig.configUseBhaptics.Value)
            {
                return;
            }

			var scaleOption = new ScaleOption(pressure, 1f); // pressure goes from 0 to 1

			if (Controllers.MainControllerType == HandType.Left)
			{
				m_hapticPlayer.SubmitRegistered(VEST_HAMMER_CHARGING_L_KEY, scaleOption);
				m_hapticPlayer.SubmitRegistered(ARMS_HAMMER_CHARGING_L_KEY, scaleOption);
			}
			else
			{
				m_hapticPlayer.SubmitRegistered(VEST_HAMMER_CHARGING_R_KEY, scaleOption);
				m_hapticPlayer.SubmitRegistered(ARMS_HAMMER_CHARGING_R_KEY, scaleOption);
			}
        }

        public void StopWeaponReloadHaptics()
        {
            m_nextReloadHapticPatternTime = 0;
            m_hapticPlayer.TurnOff(VEST_RELOAD_R_KEY);
            m_hapticPlayer.TurnOff(VEST_RELOAD_L_KEY);
            m_hapticPlayer.TurnOff(ARMS_RELOAD_R_KEY);
            m_hapticPlayer.TurnOff(ARMS_RELOAD_L_KEY);
        }

        public void PlayWeaponReloadedHaptics()
        {
            if (!VRConfig.configUseBhaptics.Value)
            {
                return;
            }

            StopWeaponReloadHaptics();
        }

        public void PlayTriggerWeaponReloadHaptics()
        {
            if (!VRConfig.configUseBhaptics.Value)
            {
                return;
            }

			m_nextReloadHapticPatternTime = Time.time;
        }

        public void PlayWeaponFireHaptics(Weapon weapon)
        {
            if (!VRConfig.configUseBhaptics.Value)
            {
                return;
            }

			float intensity = Haptics.GetFireHapticStrength(weapon);
			var scaleOption = new ScaleOption(intensity, 1.0f);

			if (Controllers.MainControllerType == HandType.Left || Controllers.AimingTwoHanded)
			{
				m_hapticPlayer.SubmitRegistered(VEST_FIRE_L_KEY, scaleOption);
				m_hapticPlayer.SubmitRegistered(ARMS_FIRE_L_KEY, scaleOption);
			}

			if (Controllers.MainControllerType == HandType.Right || Controllers.AimingTwoHanded)
			{
				m_hapticPlayer.SubmitRegistered(VEST_FIRE_R_KEY, scaleOption);
				m_hapticPlayer.SubmitRegistered(ARMS_FIRE_R_KEY, scaleOption);
			}
        }

        private RotationOption GetRotationOptionFromDirection(Vector3 direction)
        {
            /*
             * direction coordinates are [-1, 1]
             * offsetAngleX: [0, 360]
             * offsetY: [-0.5, 0.5]
             */
            float angleRadians = (float)Math.Atan2(direction.z, direction.x);
            float angleDegrees = (float)(angleRadians * 180 / Math.PI);
            float cameraYRotation = m_player.FPSCamera.Rotation.eulerAngles.y;
            float offsetAngleX = NormalizeOrientation(angleDegrees + cameraYRotation + 90f);
            float offsetY = BhapticsUtils.Clamp(0.5f - (direction.y * 2), -0.5f, 0.5f);
            return new RotationOption(offsetAngleX, offsetY);
        }

        public void PlayReceiveDamageHaptics(float dmg, Vector3 direction)
        {
            if (!VRConfig.configUseBhaptics.Value)
            {
                return;
            }

			var rotationOption = GetRotationOptionFromDirection(direction);

			float intensity = dmg * 0.3f + 0.3f;
			float duration = 1f;
			var scaleOption = new ScaleOption(intensity, duration);

			m_hapticPlayer.SubmitRegisteredVestRotation(VEST_DAMAGE_KEY, "", rotationOption, scaleOption);

			m_lastDamageRotationOption = rotationOption;
        }

        public void MineExplosionHaptics(Vector3 explosionPosition)
        {
            if (!VRConfig.configUseBhaptics.Value)
            {
                return;
            }

            const float MAX_DISTANCE = 30f;
            Vector3 playerPosition = m_player.transform.position;
            playerPosition.y = 1f; // for directional haptic (a mine height of 1 will hit horizontally)
            Vector3 direction = playerPosition - explosionPosition;
            float distance = direction.magnitude;

            if (distance < MAX_DISTANCE)
            {
                var rotationOption = GetRotationOptionFromDirection(direction);
                float intensity = 1 - (Math.Max(0, distance - 5) / MAX_DISTANCE);
                var scaleOption = new ScaleOption(intensity, 1f);

                m_hapticPlayer.SubmitRegisteredVestRotation(VEST_EXPLOSION_KEY, "", rotationOption, scaleOption);
                m_hapticPlayer.SubmitRegistered(ARMS_EXPLOSION_KEY, scaleOption);
            }
        }

        public void TentacleAttackHaptics(float dmg, Agents.Agent sourceAgent, Vector3 position)
        {
            if (!VRConfig.configUseBhaptics.Value || sourceAgent != m_player)
            {
                return;
            }

			if (m_lastDamageRotationOption != null)
			{
				var rotationOption = m_lastDamageRotationOption;
				//var rotationOption = GetRotationOptionFromDirection(position - sourceAgent.TentacleTarget.position); // could maybe calculate direction with this, but offsetY is not right
				m_hapticPlayer.SubmitRegisteredVestRotation(VEST_TENTACLE_ATTACK_KEY, rotationOption);
			}
			else
			{
				Log.Error("Received tentacle attack with no last damage rotation option!");
			}
        }

        public void FocusStateChangedHaptics(eFocusState focusState)
        {
            if (!VRConfig.configUseBhaptics.Value)
            {
                return;
            }

			if (FocusStateEvents.lastState.Equals(eFocusState.InElevator) && focusState == eFocusState.FPS)
			{
                m_hapticPlayer.SubmitRegistered(VEST_LANDING_KEY);
                m_hapticPlayer.SubmitRegistered(ARMS_LANDING_KEY);
            }
        }

        public void PlayerInteractedHaptics(PlayerAgent source)
        {
            if (!VRConfig.configUseBhaptics.Value || (source != null && source != m_player))
            {
                return;
            }

			if (Controllers.MainControllerType == HandType.Left)
			{
				m_hapticPlayer.SubmitRegistered(ARMS_INTERACT_ITEM_L_KEY);
			}
			else
			{
				m_hapticPlayer.SubmitRegistered(ARMS_INTERACT_ITEM_R_KEY);
			}
        }

        public void PlayerBioscanSetStateHaptics(eBioscanStatus status, float progress, List<PlayerAgent> playersInScan)
        {
            if (!VRConfig.configUseBhaptics.Value || playersInScan == null)
            {
                return;
            }

            if (status == eBioscanStatus.Scanning && playersInScan.Contains(m_player) && m_player.Alive)
            {
                if (m_nextBodyscanPatternTime <= 0)
                {
                    m_nextBodyscanPatternTime = Time.time;
                }
                
                m_bioscanStopFramesCount = 0;
            }
            else if (m_bioscanStopFramesCount == 0 && m_nextBodyscanPatternTime > 0)
            {
                // Indicate that bioscan stopped, and stop haptic pattern only after a few FixedUpdate() calls if we don't receive any other scan activations until then.
                // When multiple players are in different single-person scans, we receive this event every fixed frame for *each* currently scanned player,
                // and m_player is only in a single playersInScan list, so we don't want to stop the scan right when we receive the scan of someone else.
                m_bioscanStopFramesCount = 1;
            }
        }

        public void FlashlightToggledHaptics()
        {
            if (!VRConfig.configUseBhaptics.Value || !m_player.Alive)
            {
                return;
            }

            if (Controllers.MainControllerType == HandType.Left)
            {
                m_hapticPlayer.SubmitRegistered(ARMS_FLASHLIGHT_TOGGLE_L_KEY);
            }
            else
            {
                m_hapticPlayer.SubmitRegistered(ARMS_FLASHLIGHT_TOGGLE_R_KEY);
            }
        }

        public void PlayerChangedItemHaptics(ItemEquippable item)
        {
            if (!VRConfig.configUseBhaptics.Value)
            {
                return;
            }

            StopWeaponReloadHaptics();

            if (Controllers.MainControllerType == HandType.Left)
            {
                m_hapticPlayer.SubmitRegistered(ARMS_CHANGE_ITEM_L_KEY);
            }
			else
            {
                m_hapticPlayer.SubmitRegistered(ARMS_CHANGE_ITEM_R_KEY);
            }
        }

        public void AmmoGainedHaptics(float ammoStandardRel, float ammoSpecialRel, float ammoClassRel)
        {
            if (!VRConfig.configUseBhaptics.Value)
            {
                return;
            }

            if (ammoStandardRel > 0 || ammoSpecialRel > 0)
            {
                m_hapticPlayer.SubmitRegistered(VEST_GAIN_AMMO_KEY);
                m_hapticPlayer.SubmitRegistered(ARMS_GAIN_AMMO_KEY);
            }
            else if (ammoClassRel > 0)
            {
                m_hapticPlayer.SubmitRegistered(VEST_GAIN_TOOL_AMMO_KEY);
                m_hapticPlayer.SubmitRegistered(ARMS_GAIN_TOOL_AMMO_KEY);
            }
        }

        public void InfectionUpdatedHaptics(float infection)
        {
            if (!VRConfig.configUseBhaptics.Value)
            {
                return;
            }

            if (m_lastInfection - infection > MIN_DISINFECTION_GAIN_FOR_HAPTIC) // Gained some disinfection
            {
                m_hapticPlayer.SubmitRegistered(VEST_GAIN_DISINFECTION_KEY);
            }

            m_lastInfection = infection;
        }

        public void OnHealthUpdated(float health)
        {
            if (VRConfig.configUseBhaptics.Value)
            {
                if (health <= LOW_HEALTH && m_nextHeartbeatPatternTime <= 0)
                {
                    m_nextHeartbeatPatternTime = Time.time;
                }
                else if (health > LOW_HEALTH && m_nextHeartbeatPatternTime > 0)
                {
                    m_nextHeartbeatPatternTime = 0;
                }

                if (health - m_lastHealth > MIN_HEALTH_GAIN_FOR_HAPTIC) // Gained some health
                {
                    m_hapticPlayer.SubmitRegistered(VEST_GAIN_HEALTH_KEY);
                }

                if (health <= 0 && m_lastHealth > 0)
                {
                    StopWeaponReloadHaptics();
                    m_nextBodyscanPatternTime = 0;
                    m_hapticPlayer.TurnOff(VEST_BODY_SCAN_KEY);

                    m_hapticPlayer.SubmitRegistered(VEST_DEATH_KEY);
                }
            }

            m_lastHealth = health;
        }

        public void OnAmmoUpdate(InventorySlotAmmo item, int clipleft)
        {
            if (!VRConfig.configUseBhaptics.Value)
            {
                return;
            }

            if (ItemEquippableEvents.IsCurrentItemShootableWeapon() &&
                ItemEquippableEvents.currentItem.ItemDataBlock.inventorySlot.Equals(item.Slot))
            {
                AmmoType ammoType = item.AmmoType;
                if (ammoType == AmmoType.Standard || ammoType == AmmoType.Special)
                {
                    if (clipleft == 0)
                    {
                        if (Controllers.MainControllerType == HandType.Left || Controllers.AimingTwoHanded)
                        {
                            m_hapticPlayer.SubmitRegistered(ARMS_OUT_OF_AMMO_L_KEY);
                        }

                        if (Controllers.MainControllerType == HandType.Right || Controllers.AimingTwoHanded)
                        {
                            m_hapticPlayer.SubmitRegistered(ARMS_OUT_OF_AMMO_R_KEY);
                        }
                    }
                }
            }
        }

        public void OnPlayerLocomotionStateChanged(PlayerLocomotion.PLOC_State state)
        {
            if (!VRConfig.configUseBhaptics.Value)
            {
                return;
            }

            if ((m_lastLocState == PlayerLocomotion.PLOC_State.Fall || m_lastLocState == PlayerLocomotion.PLOC_State.Jump)
                && (state == PlayerLocomotion.PLOC_State.Stand || state == PlayerLocomotion.PLOC_State.Crouch))
            {
                m_hapticPlayer.SubmitRegistered(VEST_LANDING_SMALL_KEY);
            }

            m_lastLocState = state;
        }

        public void CrouchToggleHaptics(bool isCrouched)
        {
            if (!VRConfig.configUseBhaptics.Value || m_lastLocState == PlayerLocomotion.PLOC_State.InElevator)
            {
                return;
            }

            if (isCrouched)
            {
                m_hapticPlayer.SubmitRegistered(VEST_CROUCH_KEY);
            }
            else
            {
                m_hapticPlayer.SubmitRegistered(VEST_STAND_KEY);
            }
        }

        private float NormalizeOrientation(float orientation)
        {
            float result = orientation % 360;

            if (result < 0)
            {
                result += 360;
            }

            return result;
        }
    }
}