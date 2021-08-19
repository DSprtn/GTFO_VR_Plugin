using UnityEngine;
using Bhaptics.Tact;
using GTFO_VR.Events;
using System;
using System.IO;

namespace GTFO_VR.Core.PlayerBehaviours
{
    public class BhapticsIntegration : MonoBehaviour
    {
        private static readonly string VEST_DAMAGE_KEY = "vest_damage";
        private static readonly string VEST_FIRE_R_KEY = "vest_fire_r";
        private static readonly string VEST_RELOAD_R_KEY = "vest_reload_r";
        private static readonly string VEST_RELOAD_L_KEY = "vest_reload_l";
        private static readonly string VEST_HAMMER_CHARGING_R_KEY = "vest_hammer_charging_r";
        private static readonly string VEST_HAMMER_SMACK_R_KEY = "vest_hammer_smack_r";

        private static readonly string ARMS_FIRE_R_KEY = "arms_fire_r";
        private static readonly string ARMS_RELOAD_R_KEY = "arms_reload_r";
        private static readonly string ARMS_RELOAD_L_KEY = "arms_reload_l";
        private static readonly string ARMS_HAMMER_CHARGING_R_KEY = "arms_hammer_charging_r";
        private static readonly string ARMS_HAMMER_SMACK_R_KEY = "arms_hammer_smack_r";

        private static readonly string PATTERNS_FOLDER = "BepInEx\\plugins\\bhaptics-patterns\\";

        private HapticPlayer m_hapticPlayer;
        private float m_nextReloadHapticPatternTime;
        private static readonly float RELOAD_FEEDBACK_DURATION = 1.0f;
        public static float m_cameraYRotation;

        public BhapticsIntegration(IntPtr value) : base(value)
        {
        }

        public void Setup()
        {
            m_hapticPlayer = new HapticPlayer();
            RegisterVestTactKey(VEST_DAMAGE_KEY);
            RegisterVestTactKey(VEST_FIRE_R_KEY);
            RegisterVestTactKey(VEST_RELOAD_R_KEY);
            //RegisterVestTactKey(VEST_RELOAD_L_KEY);
            RegisterVestTactKey(VEST_HAMMER_CHARGING_R_KEY);
            RegisterVestTactKey(VEST_HAMMER_SMACK_R_KEY);

            RegisterArmsTactKey(ARMS_FIRE_R_KEY);
            RegisterArmsTactKey(ARMS_RELOAD_R_KEY);
            RegisterArmsTactKey(ARMS_HAMMER_CHARGING_R_KEY);
            RegisterArmsTactKey(ARMS_HAMMER_SMACK_R_KEY);

            PlayerReceivedDamageEvents.OnPlayerTakeDamage += PlayReceiveDamageHaptics;
            PlayerFireWeaponEvents.OnPlayerFireWeapon += PlayWeaponFireHaptics;
            PlayerReloadEvents.OnPlayerReloaded += PlayWeaponReloadedHaptics;
            PlayerTriggerReloadEvents.OnTriggerWeaponReloaded += PlayTriggerWeaponReloadHaptics;
            HeldItemEvents.OnItemCharging += HammerChargingHaptics;
            HammerEvents.OnHammerSmack += HammerSmackHaptics;
        }
        void Update()
        {
            bool isReloading = (m_nextReloadHapticPatternTime > 0);
            if (isReloading && Time.time >= m_nextReloadHapticPatternTime)
            {
                m_hapticPlayer.SubmitRegistered(VEST_RELOAD_R_KEY);
                m_hapticPlayer.SubmitRegistered(ARMS_RELOAD_R_KEY);
                m_nextReloadHapticPatternTime += RELOAD_FEEDBACK_DURATION;
            }
        }

        public static void SetCameraYRotation(float cameraYRotation)
        {
            m_cameraYRotation = cameraYRotation;
        }

        private void HammerSmackHaptics(float dmg)
        {
            if (VRConfig.configUseBhaptics.Value)
            {
                m_hapticPlayer.SubmitRegistered(VEST_HAMMER_SMACK_R_KEY);
                m_hapticPlayer.SubmitRegistered(ARMS_HAMMER_SMACK_R_KEY);
            }
        }

        private void HammerChargingHaptics(float pressure)
        {
            if (VRConfig.configUseBhaptics.Value)
            {
                var scaleOption = new ScaleOption(pressure, 1f); // pressure goes from 0 to 1
                m_hapticPlayer.SubmitRegistered(VEST_HAMMER_CHARGING_R_KEY, scaleOption);
                m_hapticPlayer.SubmitRegistered(ARMS_HAMMER_CHARGING_R_KEY, scaleOption);
            }
        }

        private void PlayWeaponReloadedHaptics()
        {
            if (VRConfig.configUseBhaptics.Value)
            {
                m_nextReloadHapticPatternTime = 0;
                m_hapticPlayer.TurnOff(VEST_RELOAD_R_KEY);
                m_hapticPlayer.TurnOff(VEST_RELOAD_L_KEY);
                m_hapticPlayer.TurnOff(ARMS_RELOAD_R_KEY);
                m_hapticPlayer.TurnOff(ARMS_RELOAD_L_KEY);
            }
        }

        private void PlayTriggerWeaponReloadHaptics()
        {
            if (VRConfig.configUseBhaptics.Value)
            {
                m_nextReloadHapticPatternTime = Time.time;
            }
        }

        private void PlayWeaponFireHaptics(Weapon weapon)
        {
            if (VRConfig.configUseBhaptics.Value)
            {
                float intensity = Haptics.GetFireHapticStrength(weapon);
                var scaleOption = new ScaleOption(intensity, 1.0f);
                m_hapticPlayer.SubmitRegistered(VEST_FIRE_R_KEY, scaleOption);
                m_hapticPlayer.SubmitRegistered(ARMS_FIRE_R_KEY, scaleOption);
            }
        }

        private void PlayReceiveDamageHaptics(float dmg, Vector3 direction)
        {
            if (VRConfig.configUseBhaptics.Value)
            {
                /*
                 * direction coordinates are [-1, 1]
                 * offsetAngleX: [0, 360]
                 * offsetY: [-0.5, 0.5]
                 */
                float angleRadians = (float) Math.Atan2(direction.z, direction.x);
                float angleDegrees = (float) (angleRadians * 180 / Math.PI);
                float offsetAngleX = NormalizeOrientation(angleDegrees + m_cameraYRotation + 90f);
                float offsetY = Clamp(0.5f - (direction.y * 2), -0.5f, 0.5f);
                var rotationOption = new RotationOption(offsetAngleX, offsetY);

                float intensity = dmg * 0.25f + 0.25f;
                float duration = 1f;
                var scaleOption = new ScaleOption(intensity, duration);

                m_hapticPlayer.SubmitRegisteredVestRotation(VEST_DAMAGE_KEY, "", rotationOption, scaleOption);
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
            PlayerFireWeaponEvents.OnPlayerFireWeapon -= PlayWeaponFireHaptics;
            PlayerReloadEvents.OnPlayerReloaded -= PlayWeaponReloadedHaptics;
            PlayerTriggerReloadEvents.OnTriggerWeaponReloaded -= PlayTriggerWeaponReloadHaptics;
            HeldItemEvents.OnItemCharging -= HammerChargingHaptics;
            HammerEvents.OnHammerSmack -= HammerSmackHaptics;
        }
    }
}