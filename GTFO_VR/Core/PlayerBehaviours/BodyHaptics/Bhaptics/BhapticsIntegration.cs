using Bhaptics.Tact;
using GTFO_VR.Core.VR_Input;
using Player;
using UnityEngine;

namespace GTFO_VR.Core.PlayerBehaviours.BodyHaptics.Bhaptics
{
    public class BhapticsIntegration : BodyHapticAgent
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


        private static readonly string VISOR_CAGE_DROP_KEY = "visor_rumble_headfalling";
        private static readonly string VISOR_KNOCKED_DOWN_KEY = "visor_boom1_head";
        private static readonly string VISOR_LICKED_TINTACLE_KEY = "visor_boom6_head";
        private static readonly string VISOR_HEALTH_PACK_KEY = "visor_rumble3_head";
        private static readonly string VISOR_BIOSCAN_KEY = "visor_scan8_head";
        private static readonly string VISOR_REVIVED_KEY = "visor_rumble4_head";
        private static readonly string VISOR_HAMMER_HALF_KEY = "visor_point7_head";
        private static readonly string VISOR_HAMMER_FULL_KEY = "visor_point8_head";
        private static readonly string VISOR_DAMAGED_KEY = "visor_boom4_head";
        private static readonly string VISOR_MINE_EXPLOSION_KEY = "visor_rumble1_head";
        private static readonly string VISOR_DISINFECTION_KEY = "visor_point9_head";
        private static readonly string VISOR_SPLATDROP_KEY = "visor_splatdropping";

        private LocalPlayerAgent m_player;
        private HapticPlayer m_hapticPlayer;

        private float m_nextReloadHapticPatternTime;
        private float m_nextHeartbeatPatternTime;
        private float m_nextBodyscanPatternTime;
        private float m_lastHealth = 1f;
        private RotationOption m_lastDamageRotationOption;
        private PlayerLocomotion.PLOC_State m_lastLocState;

        private static readonly float RELOAD_FEEDBACK_DURATION = 1.0f;
        private static readonly float HEARTBEAT_REPEAT_DELAY = 1.0f;
        private static readonly float BODY_SCAN_REPEAT_DELAY = 13.5f;

        public void Setup(LocalPlayerAgent player, HapticPlayer hapticPlayer)
        {
            m_player = player;
            m_lastLocState = player.Locomotion.m_currentStateEnum;
            m_hapticPlayer = hapticPlayer;

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
        }

        public void Update()
        {
            if (AgentActive())
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
                m_hapticPlayer.SubmitRegistered(VISOR_BIOSCAN_KEY);
                m_nextBodyscanPatternTime += BODY_SCAN_REPEAT_DELAY;
            }
        }

        public void HammerSmackHaptics(float dmg)
        {
            if (!AgentActive())
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
            if (!AgentActive())
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
            if (!AgentActive())
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

        public void PlayWeaponReloadHaptics()
        {
            if (!AgentActive())
            {
                return;
            }

            m_nextReloadHapticPatternTime = Time.time;
        }

        public void StopWeaponReloadHaptics()
        {
            m_nextReloadHapticPatternTime = 0;
            m_hapticPlayer.TurnOff(VEST_RELOAD_R_KEY);
            m_hapticPlayer.TurnOff(VEST_RELOAD_L_KEY);
            m_hapticPlayer.TurnOff(ARMS_RELOAD_R_KEY);
            m_hapticPlayer.TurnOff(ARMS_RELOAD_L_KEY);
        }

        public void PlayWeaponFireHaptics(Weapon weapon)
        {
            if (!AgentActive())
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

        RotationOption ToRotationOption(OrientationSettings orientationSettings)
        {
            return new RotationOption(orientationSettings.OffsetAngleX, orientationSettings.OffsetY);
        }

        private RotationOption GetRotationOptionFromDirection(Vector3 direction)
        {
            var orientationSettings = BodyHapticsUtils.GetOrientationSettingsFromDirection(m_player, direction);
            return ToRotationOption(orientationSettings);
        }

        public void PlayReceiveDamageHaptics(float dmg, Vector3 direction)
        {
            if (!AgentActive())
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

        public void MineExplosionHaptics(OrientationSettings orientationSettings, float intensity)
        {
            if (!AgentActive())
            {
                return;
            }

            RotationOption rotationOption = ToRotationOption(orientationSettings);
            var scaleOption = new ScaleOption(intensity, 1f);

            m_hapticPlayer.SubmitRegisteredVestRotation(VEST_EXPLOSION_KEY, "", rotationOption, scaleOption);
            m_hapticPlayer.SubmitRegistered(ARMS_EXPLOSION_KEY, scaleOption);
        }

        public void TentacleAttackHaptics(float dmg, Agents.Agent sourceAgent, Vector3 position)
        {
            if (!AgentActive() || sourceAgent != m_player)
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

        public void LandedFromElevator(eFocusState focusState)
        {
            if (!AgentActive())
            {
                return;
            }

            m_hapticPlayer.SubmitRegistered(VEST_LANDING_KEY);
            m_hapticPlayer.SubmitRegistered(ARMS_LANDING_KEY);
        }

        public void PlayerInteractedHaptics(PlayerAgent source)
        {
            if (!AgentActive() || (source != null && source != m_player))
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

        public void PlayBioscanHaptics()
        {
            if (!AgentActive())
            {
                return;
            }

            if (m_nextBodyscanPatternTime <= 0)
            {
                m_nextBodyscanPatternTime = Time.time;
            }
        }

        public void StopBioscanHaptics()
        {
            m_nextBodyscanPatternTime = 0f;
            m_hapticPlayer.TurnOff(VEST_BODY_SCAN_KEY);
        }

        public void FlashlightToggledHaptics()
        {
            if (!AgentActive() || !m_player.Alive)
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
            if (!AgentActive())
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
            if (!AgentActive())
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

        public void InfectionHealed(float infection)
        {
            if (!AgentActive())
            {
                return;
            }

            m_hapticPlayer.SubmitRegistered(VEST_GAIN_DISINFECTION_KEY);
        }

        public void OnHealthUpdated(float health)
        {
            if (AgentActive())
            {
                if (health <= BodyHapticsUtils.LOW_HEALTH && m_nextHeartbeatPatternTime <= 0)
                {
                    m_nextHeartbeatPatternTime = Time.time;
                }
                else if (health > BodyHapticsUtils.LOW_HEALTH && m_nextHeartbeatPatternTime > 0)
                {
                    m_nextHeartbeatPatternTime = 0;
                }

                if (health - m_lastHealth > BodyHapticsUtils.MIN_HEALTH_GAIN_FOR_HAPTIC) // Gained some health
                {
                    m_hapticPlayer.SubmitRegistered(VEST_GAIN_HEALTH_KEY);
                    m_hapticPlayer.SubmitRegistered(VISOR_HEALTH_PACK_KEY);
                }

                if (health <= 0 && m_lastHealth > 0)
                {
                    StopWeaponReloadHaptics();
                    m_nextBodyscanPatternTime = 0;
                    m_hapticPlayer.TurnOff(VEST_BODY_SCAN_KEY);
                    m_hapticPlayer.TurnOff(VISOR_BIOSCAN_KEY);

                    m_hapticPlayer.SubmitRegistered(VEST_DEATH_KEY);
                    m_hapticPlayer.SubmitRegistered(VISOR_KNOCKED_DOWN_KEY);
                }
            }

            m_lastHealth = health;
        }

        public void WeaponAmmoEmpty(bool leftArm)
        {
            if (leftArm || Controllers.AimingTwoHanded)
            {
                m_hapticPlayer.SubmitRegistered(ARMS_OUT_OF_AMMO_L_KEY);
            }

            if (!leftArm || Controllers.AimingTwoHanded)
            {
                m_hapticPlayer.SubmitRegistered(ARMS_OUT_OF_AMMO_R_KEY);
            }
        }

        public void OnPlayerLocomotionStateChanged(PlayerLocomotion.PLOC_State state)
        {
            if (!AgentActive())
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
            if (!AgentActive() || m_lastLocState == PlayerLocomotion.PLOC_State.InElevator)
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
        public bool AgentActive()
        {
            return VRConfig.configUseBhaptics.Value;
        }
    }
}