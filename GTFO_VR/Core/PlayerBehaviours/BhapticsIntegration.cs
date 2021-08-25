﻿using UnityEngine;
using Bhaptics.Tact;
using GTFO_VR.Events;
using GTFO_VR.Core.VR_Input;
using System;
using System.IO;
using Player;

namespace GTFO_VR.Core.PlayerBehaviours
{
    public class BhapticsIntegration : MonoBehaviour
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

        private static readonly string PATTERNS_FOLDER = "BepInEx\\plugins\\bhaptics-patterns\\";

        private PlayerAgent m_player;
        private HapticPlayer m_hapticPlayer;

        private float m_nextReloadHapticPatternTime;
        private RotationOption m_lastDamageRotationOption;
        private static readonly float RELOAD_FEEDBACK_DURATION = 1.0f;
        public static float m_cameraYRotation;

        public BhapticsIntegration(IntPtr value) : base(value)
        {
        }

        public void Setup(PlayerAgent player)
        {
            m_player = player;

            m_hapticPlayer = new HapticPlayer();
            RegisterVestTactKey(VEST_DAMAGE_KEY);
            RegisterVestTactKey(VEST_TENTACLE_ATTACK_KEY);
            RegisterVestTactKey(VEST_FIRE_R_KEY);
            RegisterVestTactKey(VEST_FIRE_L_KEY);
            RegisterVestTactKey(VEST_RELOAD_R_KEY);
            RegisterVestTactKey(VEST_RELOAD_L_KEY);
            RegisterVestTactKey(VEST_HAMMER_CHARGING_R_KEY);
            RegisterVestTactKey(VEST_HAMMER_CHARGING_L_KEY);
            RegisterVestTactKey(VEST_HAMMER_FULLY_CHARGED_R_KEY);
            RegisterVestTactKey(VEST_HAMMER_FULLY_CHARGED_L_KEY);
            RegisterVestTactKey(VEST_HAMMER_SMACK_R_KEY);
            RegisterVestTactKey(VEST_HAMMER_SMACK_L_KEY);
            RegisterVestTactKey(VEST_LANDING_KEY);

            RegisterArmsTactKey(ARMS_FIRE_R_KEY);
            RegisterArmsTactKey(ARMS_FIRE_L_KEY);
            RegisterArmsTactKey(ARMS_RELOAD_R_KEY);
            RegisterArmsTactKey(ARMS_RELOAD_L_KEY);
            RegisterArmsTactKey(ARMS_HAMMER_CHARGING_R_KEY);
            RegisterArmsTactKey(ARMS_HAMMER_CHARGING_L_KEY);
            RegisterArmsTactKey(ARMS_HAMMER_FULLY_CHARGED_R_KEY);
            RegisterArmsTactKey(ARMS_HAMMER_FULLY_CHARGED_L_KEY);
            RegisterArmsTactKey(ARMS_HAMMER_SMACK_R_KEY);
            RegisterArmsTactKey(ARMS_HAMMER_SMACK_L_KEY);
            RegisterArmsTactKey(ARMS_INTERACT_ITEM_R_KEY);
            RegisterArmsTactKey(ARMS_INTERACT_ITEM_L_KEY);
            RegisterArmsTactKey(ARMS_FLASHLIGHT_TOGGLE_R_KEY);
            RegisterArmsTactKey(ARMS_FLASHLIGHT_TOGGLE_L_KEY);
            RegisterArmsTactKey(ARMS_CHANGE_ITEM_R_KEY);
            RegisterArmsTactKey(ARMS_CHANGE_ITEM_L_KEY);

            PlayerReceivedDamageEvents.OnPlayerTakeDamage += PlayReceiveDamageHaptics;
            TentacleAttackEvents.OnTentacleAttack += TentacleAttackHaptics;
            PlayerFireWeaponEvents.OnPlayerFireWeapon += PlayWeaponFireHaptics;
            PlayerReloadEvents.OnPlayerReloaded += PlayWeaponReloadedHaptics;
            PlayerTriggerReloadEvents.OnTriggerWeaponReloaded += PlayTriggerWeaponReloadHaptics;
            HeldItemEvents.OnItemCharging += HammerChargingHaptics;
            HammerEvents.OnHammerSmack += HammerSmackHaptics;
            HammerEvents.OnHammerFullyCharged += HammerFullyChargedHaptics;
            FocusStateEvents.OnFocusStateChange += FocusStateChangedHaptics;
            ItemInteractEvents.OnItemInteracted += ItemInteractedHaptics;
            ItemInteractEvents.OnFlashlightToggled += FlashlightToggledHaptics;
            ItemEquippableEvents.OnPlayerWieldItem += PlayerChangedItemHaptics;
        }

        void Update()
        {
            bool isReloading = (m_nextReloadHapticPatternTime > 0);
            if (isReloading && Time.time >= m_nextReloadHapticPatternTime)
            {
                if (Controllers.mainControllerType == HandType.Left)
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
        }

        public static void SetCameraYRotation(float cameraYRotation)
        {
            m_cameraYRotation = cameraYRotation;
        }

        private void HammerSmackHaptics(float dmg)
        {
            if (!VRConfig.configUseBhaptics.Value)
            {
                return;
            }

			if (Controllers.mainControllerType == HandType.Left)
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

        private void HammerFullyChargedHaptics()
        {
            if (!VRConfig.configUseBhaptics.Value)
            {
                return;
            }

			if (Controllers.mainControllerType == HandType.Left)
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

        private void HammerChargingHaptics(float pressure)
        {
            if (!VRConfig.configUseBhaptics.Value)
            {
                return;
            }

			var scaleOption = new ScaleOption(pressure, 1f); // pressure goes from 0 to 1

			if (Controllers.mainControllerType == HandType.Left)
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
        private void StopWeaponReloadHaptics()
        {
            m_nextReloadHapticPatternTime = 0;
            m_hapticPlayer.TurnOff(VEST_RELOAD_R_KEY);
            m_hapticPlayer.TurnOff(VEST_RELOAD_L_KEY);
            m_hapticPlayer.TurnOff(ARMS_RELOAD_R_KEY);
            m_hapticPlayer.TurnOff(ARMS_RELOAD_L_KEY);
        }

        private void PlayWeaponReloadedHaptics()
        {
            if (!VRConfig.configUseBhaptics.Value)
            {
                return;
            }

            StopWeaponReloadHaptics();
        }

        private void PlayTriggerWeaponReloadHaptics()
        {
            if (!VRConfig.configUseBhaptics.Value)
            {
                return;
            }

			m_nextReloadHapticPatternTime = Time.time;
        }

        private void PlayWeaponFireHaptics(Weapon weapon)
        {
            if (!VRConfig.configUseBhaptics.Value)
            {
                return;
            }

			float intensity = Haptics.GetFireHapticStrength(weapon);
			var scaleOption = new ScaleOption(intensity, 1.0f);

			if (Controllers.mainControllerType == HandType.Left || Controllers.aimingTwoHanded)
			{
				m_hapticPlayer.SubmitRegistered(VEST_FIRE_L_KEY, scaleOption);
				m_hapticPlayer.SubmitRegistered(ARMS_FIRE_L_KEY, scaleOption);
			}

			if (Controllers.mainControllerType == HandType.Right || Controllers.aimingTwoHanded)
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
            float offsetAngleX = NormalizeOrientation(angleDegrees + m_cameraYRotation + 90f);
            float offsetY = Clamp(0.5f - (direction.y * 2), -0.5f, 0.5f);
            return new RotationOption(offsetAngleX, offsetY);
        }

        private void PlayReceiveDamageHaptics(float dmg, Vector3 direction)
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

        private void TentacleAttackHaptics(float dmg, Agents.Agent sourceAgent, Vector3 position)
        {
            if (!VRConfig.configUseBhaptics.Value)
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

        private void FocusStateChangedHaptics(eFocusState focusState)
        {
            if (!VRConfig.configUseBhaptics.Value)
            {
                return;
            }

			if (FocusStateEvents.lastState.Equals(eFocusState.InElevator) && focusState == eFocusState.FPS)
			{
				m_hapticPlayer.SubmitRegistered(VEST_LANDING_KEY);
			}
        }

        private void ItemInteractedHaptics(PlayerAgent player)
        {
            if (!VRConfig.configUseBhaptics.Value)
            {
                return;
            }

            if (player != m_player)
            {
                Log.Info("Another player did an interaction, return");
                return;
            }

			if (Controllers.mainControllerType == HandType.Left)
			{
				m_hapticPlayer.SubmitRegistered(ARMS_INTERACT_ITEM_L_KEY);
			}
			else
			{
				m_hapticPlayer.SubmitRegistered(ARMS_INTERACT_ITEM_R_KEY);
			}
        }

        private void FlashlightToggledHaptics(bool enabled)
        {
            if (!VRConfig.configUseBhaptics.Value)
            {
                return;
            }

			if (Controllers.mainControllerType == HandType.Left)
			{
				m_hapticPlayer.SubmitRegistered(ARMS_FLASHLIGHT_TOGGLE_L_KEY);
			}
			else
			{
				m_hapticPlayer.SubmitRegistered(ARMS_FLASHLIGHT_TOGGLE_R_KEY);
			}
        }

        private void PlayerChangedItemHaptics(ItemEquippable item)
        {
            if (!VRConfig.configUseBhaptics.Value)
            {
                return;
            }

            StopWeaponReloadHaptics();

            if (Controllers.mainControllerType == HandType.Left)
            {
                m_hapticPlayer.SubmitRegistered(ARMS_CHANGE_ITEM_L_KEY);
            }
			else
            {
                m_hapticPlayer.SubmitRegistered(ARMS_CHANGE_ITEM_R_KEY);
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

        private float Clamp(float v, float min, float max)
        {
            return Math.Min(Math.Max(v, min), max);
        }

        private void RegisterVestTactKey(string key)
        {
            RegisterArmsTactKey(PATTERNS_FOLDER + "vest\\", key);
        }

        private void RegisterArmsTactKey(string key)
        {
            RegisterArmsTactKey(PATTERNS_FOLDER + "arms\\", key);
        }

        private void RegisterArmsTactKey(string folder, string key)
        {
            string fileName = key.Substring(key.IndexOf("_") + 1);
            string patternFileContent = File.ReadAllText(folder + fileName + ".tact");
            m_hapticPlayer.RegisterTactFileStr(key, patternFileContent);
        }

        private void OnDestroy()
        {
            PlayerReceivedDamageEvents.OnPlayerTakeDamage -= PlayReceiveDamageHaptics;
            TentacleAttackEvents.OnTentacleAttack -= TentacleAttackHaptics;
            PlayerFireWeaponEvents.OnPlayerFireWeapon -= PlayWeaponFireHaptics;
            PlayerReloadEvents.OnPlayerReloaded -= PlayWeaponReloadedHaptics;
            PlayerTriggerReloadEvents.OnTriggerWeaponReloaded -= PlayTriggerWeaponReloadHaptics;
            HeldItemEvents.OnItemCharging -= HammerChargingHaptics;
            HammerEvents.OnHammerSmack -= HammerSmackHaptics;
            HammerEvents.OnHammerFullyCharged -= HammerFullyChargedHaptics;
            FocusStateEvents.OnFocusStateChange -= FocusStateChangedHaptics;
            ItemInteractEvents.OnItemInteracted -= ItemInteractedHaptics;
            ItemInteractEvents.OnFlashlightToggled -= FlashlightToggledHaptics;
            ItemEquippableEvents.OnPlayerWieldItem -= PlayerChangedItemHaptics;
        }
    }
}