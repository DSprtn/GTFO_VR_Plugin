using Bhaptics.SDK2;
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

        private float m_nextReloadHapticPatternTime;
        private float m_nextHeartbeatPatternTime;
        private float m_nextBodyscanPatternTime;
        private float m_lastHealth = 1f;
        private OrientationSettings m_lastDamageRotationOption;
        private PlayerLocomotion.PLOC_State m_lastLocState;

        private static readonly float RELOAD_FEEDBACK_DURATION = 1.0f;
        private static readonly float HEARTBEAT_REPEAT_DELAY = 1.0f;
        private static readonly float BODY_SCAN_REPEAT_DELAY = 13.5f;

        public void Setup(LocalPlayerAgent player)
        {
            m_player = player;
            m_lastLocState = player.Locomotion.m_currentStateEnum;
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
                    BhapticsSDK2.Play(VEST_RELOAD_L_KEY);
                    BhapticsSDK2.Play(ARMS_RELOAD_L_KEY);
                }
                else
                {
                    BhapticsSDK2.Play(VEST_RELOAD_R_KEY);
                    BhapticsSDK2.Play(ARMS_RELOAD_R_KEY);
                }
                m_nextReloadHapticPatternTime += RELOAD_FEEDBACK_DURATION;
            }

            if (m_nextHeartbeatPatternTime > 0f && currentTime >= m_nextHeartbeatPatternTime)
            {
                BhapticsSDK2.Play(VEST_NEED_HEALTH_KEY);
                m_nextHeartbeatPatternTime += HEARTBEAT_REPEAT_DELAY;
            }

            if (m_nextBodyscanPatternTime > 0f && currentTime >= m_nextBodyscanPatternTime)
            {
                BhapticsSDK2.Play(VEST_BODY_SCAN_KEY);
                BhapticsSDK2.Play(VISOR_BIOSCAN_KEY);
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
                BhapticsSDK2.Play(VEST_HAMMER_SMACK_L_KEY);
                BhapticsSDK2.Play(ARMS_HAMMER_SMACK_L_KEY);
            }
            else
            {
                BhapticsSDK2.Play(VEST_HAMMER_SMACK_R_KEY);
                BhapticsSDK2.Play(ARMS_HAMMER_SMACK_R_KEY);
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
                BhapticsSDK2.Play(VEST_HAMMER_FULLY_CHARGED_L_KEY);
                BhapticsSDK2.Play(ARMS_HAMMER_FULLY_CHARGED_L_KEY);
            }
            else
            {
                BhapticsSDK2.Play(VEST_HAMMER_FULLY_CHARGED_R_KEY);
                BhapticsSDK2.Play(ARMS_HAMMER_FULLY_CHARGED_R_KEY);
            }
            // tactvisor doesn't need left and right differenciation
            BhapticsSDK2.Play(VISOR_HAMMER_FULL_KEY);
        }

        public void HammerHalfChargedHaptics()
        {
            if (!AgentActive())
            {
                return;
            }
            BhapticsSDK2.Play(VISOR_HAMMER_HALF_KEY);
        }

        public void HammerChargingHaptics(float pressure)
        {
            if (!AgentActive())
            {
                return;
            }

            // pressure goes from 0 to 1

            if (Controllers.MainControllerType == HandType.Left)
            {
                BhapticsSDK2.Play(VEST_HAMMER_CHARGING_L_KEY, pressure, 1f, 0f, 0f);
                BhapticsSDK2.Play(ARMS_HAMMER_CHARGING_L_KEY, pressure, 1f, 0f, 0f);
            }
            else
            {
                BhapticsSDK2.Play(VEST_HAMMER_CHARGING_R_KEY, pressure, 1f, 0f, 0f);
                BhapticsSDK2.Play(ARMS_HAMMER_CHARGING_R_KEY, pressure, 1f, 0f, 0f);
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
            BhapticsSDK2.Stop(VEST_RELOAD_R_KEY);
            BhapticsSDK2.Stop(VEST_RELOAD_L_KEY);
            BhapticsSDK2.Stop(ARMS_RELOAD_R_KEY);
            BhapticsSDK2.Stop(ARMS_RELOAD_L_KEY);
        }

        public void PlayWeaponFireHaptics(Weapon weapon)
        {
            if (!AgentActive())
            {
                return;
            }

            float intensity = Haptics.GetFireHapticStrength(weapon);
            //var scaleOption = new ScaleOption(intensity, 1.0f);

            if (Controllers.MainControllerType == HandType.Left || Controllers.AimingTwoHanded)
            {
                BhapticsSDK2.Play(VEST_FIRE_L_KEY, intensity, 1.0f, 0f, 0f);
                BhapticsSDK2.Play(ARMS_FIRE_L_KEY, intensity, 1.0f, 0f, 0f);
            }

            if (Controllers.MainControllerType == HandType.Right || Controllers.AimingTwoHanded)
            {
                BhapticsSDK2.Play(VEST_FIRE_R_KEY, intensity, 1.0f, 0f, 0f);
                BhapticsSDK2.Play(ARMS_FIRE_R_KEY, intensity, 1.0f, 0f, 0f);
            }
        }

        private OrientationSettings GetRotationOptionFromDirection(Vector3 direction)
        {
            return BodyHapticsUtils.GetOrientationSettingsFromDirection(m_player, direction);
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
            //var scaleOption = new ScaleOption(intensity, duration);

            BhapticsSDK2.Play(VEST_DAMAGE_KEY, intensity, duration, rotationOption.OffsetAngleX, rotationOption.OffsetY);
            BhapticsSDK2.Play(VISOR_DAMAGED_KEY, intensity, duration, 0f, 0f);

            m_lastDamageRotationOption = rotationOption;
        }

        public void MineExplosionHaptics(OrientationSettings orientationSettings, float intensity)
        {
            if (!AgentActive())
            {
                return;
            }

            //RotationOption rotationOption = ToRotationOption(orientationSettings);
            //var scaleOption = new ScaleOption(intensity, 1f);

            BhapticsSDK2.Play(VEST_EXPLOSION_KEY, intensity, 1f, orientationSettings.OffsetAngleX, orientationSettings.OffsetY);
            BhapticsSDK2.Play(ARMS_EXPLOSION_KEY, intensity, 1f, 0f, 0f);
            BhapticsSDK2.Play(VISOR_MINE_EXPLOSION_KEY, intensity, 1f, 0f, 0f);
        }

        public void TentacleAttackHaptics(float dmg, Agents.Agent sourceAgent, Vector3 position)
        {
            if (!AgentActive() || sourceAgent != m_player)
            {
                return;
            }

            if (m_lastDamageRotationOption != null)
            {
                OrientationSettings rotationOption = m_lastDamageRotationOption;
                //var rotationOption = GetRotationOptionFromDirection(position - sourceAgent.TentacleTarget.position); // could maybe calculate direction with this, but offsetY is not right
                BhapticsSDK2.Play(VEST_TENTACLE_ATTACK_KEY, 1f, 1f, rotationOption.OffsetAngleX, rotationOption.OffsetY);
                BhapticsSDK2.Play(VISOR_LICKED_TINTACLE_KEY);
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

            BhapticsSDK2.Play(VEST_LANDING_KEY);
            BhapticsSDK2.Play(ARMS_LANDING_KEY);
        }

        public void PlayerInteractedHaptics(PlayerAgent source)
        {
            if (!AgentActive() || (source != null && source != m_player))
            {
                return;
            }

            if (Controllers.MainControllerType == HandType.Left)
            {
                BhapticsSDK2.Play(ARMS_INTERACT_ITEM_L_KEY);
            }
            else
            {
                BhapticsSDK2.Play(ARMS_INTERACT_ITEM_R_KEY);
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
            BhapticsSDK2.Stop(VEST_BODY_SCAN_KEY);
            BhapticsSDK2.Stop(VISOR_BIOSCAN_KEY);
        }

        public void FlashlightToggledHaptics()
        {
            if (!AgentActive() || !m_player.Alive)
            {
                return;
            }

            if (Controllers.MainControllerType == HandType.Left)
            {
                BhapticsSDK2.Play(ARMS_FLASHLIGHT_TOGGLE_L_KEY);
            }
            else
            {
                BhapticsSDK2.Play(ARMS_FLASHLIGHT_TOGGLE_R_KEY);
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
                BhapticsSDK2.Play(ARMS_CHANGE_ITEM_L_KEY);
            }
            else
            {
                BhapticsSDK2.Play(ARMS_CHANGE_ITEM_R_KEY);
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
                BhapticsSDK2.Play(VEST_GAIN_AMMO_KEY);
                BhapticsSDK2.Play(ARMS_GAIN_AMMO_KEY);
            }
            else if (ammoClassRel > 0)
            {
                BhapticsSDK2.Play(VEST_GAIN_TOOL_AMMO_KEY);
                BhapticsSDK2.Play(ARMS_GAIN_TOOL_AMMO_KEY);
            }
        }

        public void InfectionHealed(float infection)
        {
            if (!AgentActive())
            {
                return;
            }

            BhapticsSDK2.Play(VEST_GAIN_DISINFECTION_KEY);
            BhapticsSDK2.Play(VISOR_DISINFECTION_KEY);
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
                    BhapticsSDK2.Play(VEST_GAIN_HEALTH_KEY);
                    BhapticsSDK2.Play(VISOR_HEALTH_PACK_KEY);
                }

                if (health <= 0 && m_lastHealth > 0)
                {
                    StopWeaponReloadHaptics();
                    m_nextBodyscanPatternTime = 0;
                    BhapticsSDK2.Stop(VEST_BODY_SCAN_KEY);
                    BhapticsSDK2.Stop(VISOR_BIOSCAN_KEY);

                    BhapticsSDK2.Play(VEST_DEATH_KEY);
                    BhapticsSDK2.Play(VISOR_KNOCKED_DOWN_KEY);
                }
            }

            m_lastHealth = health;
        }

        public void WeaponAmmoEmpty(bool leftArm)
        {
            if (leftArm || Controllers.AimingTwoHanded)
            {
                BhapticsSDK2.Play(ARMS_OUT_OF_AMMO_L_KEY);
            }

            if (!leftArm || Controllers.AimingTwoHanded)
            {
                BhapticsSDK2.Play(ARMS_OUT_OF_AMMO_R_KEY);
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
                BhapticsSDK2.Play(VEST_LANDING_SMALL_KEY);
            }

            m_lastLocState = state;
        }
        public void OnLiquidSplat()
        {
            if (!AgentActive())
            {
                return;
            }
            BhapticsSDK2.Play(VISOR_SPLATDROP_KEY);
        }

        public void CrouchToggleHaptics(bool isCrouched)
        {
            if (!AgentActive() || m_lastLocState == PlayerLocomotion.PLOC_State.InElevator)
            {
                return;
            }

            if (isCrouched)
            {
                BhapticsSDK2.Play(VEST_CROUCH_KEY);
            }
            else
            {
                BhapticsSDK2.Play(VEST_STAND_KEY);
            }
        }
        public bool AgentActive()
        {
            return VRConfig.configUseBhaptics.Value;
        }
    }
}