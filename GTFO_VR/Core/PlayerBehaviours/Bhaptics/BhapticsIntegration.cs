using UnityEngine;
using Bhaptics.Tact;
using GTFO_VR.Events;
using GTFO_VR.Core.VR_Input;
using System;
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
        private static readonly string VEST_GAIN_HEALTH_KEY = "vest_gain_health";
        private static readonly string VEST_GAIN_AMMO_KEY = "vest_gain_ammo";
        private static readonly string VEST_GAIN_DISINFECTION_KEY = "vest_gain_disinfection";
        private static readonly string VEST_NEED_HEALTH_KEY = "vest_need_health";
        private static readonly string VEST_DEATH_KEY = "vest_death";

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

        private PlayerAgent m_player;
        private HapticPlayer m_hapticPlayer;

        private float m_nextReloadHapticPatternTime;
        private float m_nextHeartbeatPatternTime;
        private float m_lastHealth;
        private bool m_lastFlashlightEnabledState;
        private RotationOption m_lastDamageRotationOption;

        private static readonly float RELOAD_FEEDBACK_DURATION = 1.0f;
        private static readonly float HEARTBEAT_REPEAT_DELAY = 1.0f;
        private static readonly float LOW_HEALTH = 0.20f;
        private static readonly float MIN_HEALTH_GAIN_FOR_HAPTIC = 0.10f;

        public BhapticsIntegration(IntPtr value) : base(value)
        {
        }

        public void Setup(PlayerAgent player)
        {
            m_player = player;
            m_nextReloadHapticPatternTime = 0;
            m_nextHeartbeatPatternTime = 0;
            m_lastHealth = 1f;
            m_lastFlashlightEnabledState = player.Inventory.FlashlightEnabled;
            m_lastDamageRotationOption = null;

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
            BhapticsUtils.RegisterVestTactKey(m_hapticPlayer, VEST_GAIN_HEALTH_KEY);
            BhapticsUtils.RegisterVestTactKey(m_hapticPlayer, VEST_GAIN_AMMO_KEY);
            BhapticsUtils.RegisterVestTactKey(m_hapticPlayer, VEST_GAIN_DISINFECTION_KEY);
            BhapticsUtils.RegisterVestTactKey(m_hapticPlayer, VEST_NEED_HEALTH_KEY);
            BhapticsUtils.RegisterVestTactKey(m_hapticPlayer, VEST_DEATH_KEY);

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
            ItemEquippableEvents.OnPlayerWieldItem += PlayerChangedItemHaptics;
            ResourceUpdatedEvents.OnAmmoGained += AmmoGainedHaptics;
            ResourceUpdatedEvents.OnDisinfectionGained += DisinfectionGainedHaptics;
            ResourceUpdatedEvents.OnHealthUpdated += OnHealthUpdated;
            InventoryAmmoEvents.OnInventoryAmmoUpdate += OnAmmoUpdate;

            var elevatorSequence = gameObject.AddComponent<BhapticsElevatorSequence>();
            elevatorSequence.Setup(m_player, m_hapticPlayer);
        }

        void Update()
        {
            float currentTime = Time.time;

            bool isReloading = (m_nextReloadHapticPatternTime > 0);
            if (isReloading && currentTime >= m_nextReloadHapticPatternTime)
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

            if (m_nextHeartbeatPatternTime > 0f && currentTime >= m_nextHeartbeatPatternTime)
            {
                m_hapticPlayer.SubmitRegistered(VEST_NEED_HEALTH_KEY);
                m_nextHeartbeatPatternTime += HEARTBEAT_REPEAT_DELAY;
            }

            if (m_lastFlashlightEnabledState != m_player.Inventory.FlashlightEnabled)
            {
                FlashlightToggledHaptics();
                m_lastFlashlightEnabledState = m_player.Inventory.FlashlightEnabled;
            }
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
            float cameraYRotation = m_player.FPSCamera.Rotation.eulerAngles.y;
            float offsetAngleX = NormalizeOrientation(angleDegrees + cameraYRotation + 90f);
            float offsetY = BhapticsUtils.Clamp(0.5f - (direction.y * 2), -0.5f, 0.5f);
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

        private void FocusStateChangedHaptics(eFocusState focusState)
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

        private void ItemInteractedHaptics(PlayerAgent source)
        {
            if (!VRConfig.configUseBhaptics.Value || (source != null && source != m_player))
            {
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

        private void FlashlightToggledHaptics()
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

        private void AmmoGainedHaptics(float ammoStandardRel, float ammoSpecialRel, float ammoClassRel)
        {
            if (!VRConfig.configUseBhaptics.Value)
            {
                return;
            }

            m_hapticPlayer.SubmitRegistered(VEST_GAIN_AMMO_KEY);
            m_hapticPlayer.SubmitRegistered(ARMS_GAIN_AMMO_KEY);
        }

        private void DisinfectionGainedHaptics(float amountRel)
        {
            if (!VRConfig.configUseBhaptics.Value)
            {
                return;
            }

            m_hapticPlayer.SubmitRegistered(VEST_GAIN_DISINFECTION_KEY);
        }

        private void OnHealthUpdated(float health)
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
                m_hapticPlayer.SubmitRegistered(VEST_DEATH_KEY);
            }

            m_lastHealth = health;
        }

        private void OnAmmoUpdate(InventorySlotAmmo item, int clipleft)
        {
            AmmoType ammoType = item.AmmoType;
            if (ammoType == AmmoType.Standard || ammoType == AmmoType.Special)
            {
                if (clipleft == 0)
                {
                    if (Controllers.mainControllerType == HandType.Left || Controllers.aimingTwoHanded)
                    {
                        m_hapticPlayer.SubmitRegistered(ARMS_OUT_OF_AMMO_L_KEY);
                    }

                    if (Controllers.mainControllerType == HandType.Right || Controllers.aimingTwoHanded)
                    {
                        m_hapticPlayer.SubmitRegistered(ARMS_OUT_OF_AMMO_R_KEY);
                    }
                }
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
            ItemEquippableEvents.OnPlayerWieldItem -= PlayerChangedItemHaptics;
            ResourceUpdatedEvents.OnAmmoGained -= AmmoGainedHaptics;
            ResourceUpdatedEvents.OnDisinfectionGained -= DisinfectionGainedHaptics;
            ResourceUpdatedEvents.OnHealthUpdated -= OnHealthUpdated;
            InventoryAmmoEvents.OnInventoryAmmoUpdate -= OnAmmoUpdate;
        }
    }
}