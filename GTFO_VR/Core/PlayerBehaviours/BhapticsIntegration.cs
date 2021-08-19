using UnityEngine;
using Bhaptics.Tact;
using GTFO_VR.Events;
using System;
using System.IO;
using System.Collections;

namespace GTFO_VR.Core.PlayerBehaviours
{
    public class BhapticsIntegration : MonoBehaviour
    {
        private static readonly string DAMAGE_KEY = "damage";
        private static readonly string FIRE_R_KEY = "fire_r";
        private static readonly string RELOAD_R_KEY = "reload_r";
        private static readonly string RELOAD_L_KEY = "reload_l";
        private static readonly string HAMMER_CHARGING_R_KEY = "hammer_charging_r";
        private static readonly string HAMMER_SMACK_R_KEY = "hammer_smack_r";

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
            RegisterTactKey(DAMAGE_KEY);
            RegisterTactKey(FIRE_R_KEY);
            RegisterTactKey(RELOAD_R_KEY);
            //RegisterTactKey(RELOAD_L_KEY);
            RegisterTactKey(HAMMER_CHARGING_R_KEY);
            RegisterTactKey(HAMMER_SMACK_R_KEY);

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
                Log.Info("Launch reload haptic at " + Time.time);
                m_hapticPlayer.SubmitRegistered(RELOAD_R_KEY);
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
                m_hapticPlayer.SubmitRegistered(HAMMER_SMACK_R_KEY);
            }
        }

        private void HammerChargingHaptics(float pressure)
        {
            if (VRConfig.configUseBhaptics.Value)
            {
                var scaleOption = new ScaleOption(pressure, 1f); // pressure goes from 0 to 1
                m_hapticPlayer.SubmitRegistered(HAMMER_CHARGING_R_KEY, scaleOption);
            }
        }

        private void PlayWeaponReloadedHaptics()
        {
            if (VRConfig.configUseBhaptics.Value)
            {
                m_nextReloadHapticPatternTime = 0;
                m_hapticPlayer.TurnOff(RELOAD_R_KEY);
                m_hapticPlayer.TurnOff(RELOAD_L_KEY);
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
                m_hapticPlayer.SubmitRegistered(FIRE_R_KEY, scaleOption);
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

                m_hapticPlayer.SubmitRegisteredVestRotation(DAMAGE_KEY, "", rotationOption, scaleOption);
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

        private void RegisterTactKey(string key)
        {
            string patternFileContent = File.ReadAllText("BepInEx\\plugins\\bhaptics-patterns\\vest\\" + key + ".tact");
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