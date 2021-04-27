using GTFO_VR.Core.VR_Input;
using GTFO_VR.Events;
using GTFO_VR.Util;
using System;
using UnityEngine;

namespace GTFO_VR.Core.PlayerBehaviours
{
    public class Haptics : MonoBehaviour
    {
        public Haptics(IntPtr value) : base(value)
        {
        }

        private float lastVibrateTime;
        private float vibrationDelay = 0.1f;

        public void Setup()
        {
            PlayerReceivedDamageEvents.OnPlayerTakeDamage += PlayReceiveDamageHaptics;
            PlayerFireWeaponEvents.OnPlayerFireWeapon += PlayWeaponFireHaptics;
            PlayerReloadEvents.OnPlayerReloaded += PlayWeaponReloadHaptics;
            GlueGunEvents.OnPressureUpdate += GlueGunPressureHaptics;
            HammerEvents.OnHammerCharging += HammerCharging;
            HammerEvents.OnHammerSmack += HammerSmack;
        }

        private void HammerSmack(float dmg)
        {
            if (!VRSettings.useHapticForShooting)
            {
                return;
            }

            float duration = 0.2f;
            float frequency = 55f;

            dmg = dmg.RemapClamped(0, 1, 0.10f, VRSettings.shootingHapticsStrength);
            
            SteamVR_InputHandler.TriggerHapticPulse(Mathf.Lerp(duration, duration * 2.5f, dmg),
                Mathf.Lerp(frequency, frequency * 1.3f, dmg),
                Mathf.Lerp(0.1f, 1f, dmg),
                Controllers.GetDeviceFromHandType(Controllers.mainControllerType));
        }

        private void HammerCharging(float pressure)
        {
            if (!VRSettings.useHapticForShooting)
            {
                return;
            }

            if (pressure > 0.02f && Time.time > lastVibrateTime)
            {
                float intensity = pressure;
                float duration = 0.1f;
                float frequency = Mathf.Lerp(20, 30, pressure);
                float vibrateDelay = vibrationDelay;
                intensity *= intensity;

                if (pressure >= 0.99f)
                {
                    intensity = 2f;
                    duration = .08f;
                    frequency = 80;
                    vibrateDelay *= 2f;
                }

                SteamVR_InputHandler.TriggerHapticPulse(
              Mathf.Lerp(duration, duration * 1.5f, intensity),
              Mathf.Lerp(frequency, frequency * 1.5f, intensity),
              2f,
              Controllers.GetDeviceFromHandType(Controllers.mainControllerType));

                lastVibrateTime = Time.time + vibrateDelay;
            }
        }

        private void GlueGunPressureHaptics(float pressure)
        {
            if (!VRSettings.useHapticForShooting)
            {
                return;
            }


            if (pressure > 0.05f && Time.time > lastVibrateTime)
            {
                //hapticDelay = Mathf.Lerp(baseHapticDelay, baseHapticDelay / 2f, strength);
                float intensity = pressure;
                intensity *= intensity;
                float duration = 0.1f;
                float frequency = Mathf.Lerp(20, 35, pressure);
                float vibrateDelay = vibrationDelay;

                if (pressure >= 0.99f)
                {
                    intensity = 2f;
                    duration = .08f;
                    frequency = 80;
                    vibrateDelay *= 2f;
                }

                SteamVR_InputHandler.TriggerHapticPulse(
              Mathf.Lerp(duration, duration * 1.5f, intensity),
              Mathf.Lerp(frequency, frequency * 1.5f, intensity),
              intensity,
              Controllers.GetDeviceFromHandType(Controllers.mainControllerType));

                lastVibrateTime = Time.time + vibrateDelay;
            }
        }

        private void PlayWeaponReloadHaptics()
        {
            float duration = 0.03f;
            float frequency = 40f;
            float intensity = .5f;

            SteamVR_InputHandler.TriggerHapticPulse(
               Mathf.Lerp(duration, duration * 1.5f, intensity),
               Mathf.Lerp(frequency, frequency * 1.5f, intensity),
               intensity,
               Controllers.GetDeviceFromHandType(Controllers.mainControllerType));
        }

        private void PlayWeaponFireHaptics(Weapon weapon)
        {
            if (!VRSettings.useHapticForShooting)
            {
                return;
            }
            // Remap -1,1 to 0,1
            float intensity = Mathf.Pow(Mathf.Max(1 / Mathf.Abs(weapon.RecoilData.horizontalScale.Max), 1 / Mathf.Abs(weapon.RecoilData.verticalScale.Max)), 2);

            float duration = 0.03f;
            float frequency = 40f;

            intensity = intensity.RemapClamped(0, 8, 0.10f, VRSettings.shootingHapticsStrength);

            if (Controllers.aimingTwoHanded)
            {
                intensity *= .5f;
                intensity = Mathf.Max(intensity, 0.075f);
                SteamVR_InputHandler.TriggerHapticPulse(Mathf.Lerp(duration, duration * 1.5f, intensity),
                    Mathf.Lerp(frequency, frequency * 1.5f, intensity),
                    intensity,
                    Controllers.GetDeviceFromHandType(Controllers.offHandControllerType));
            }

            SteamVR_InputHandler.TriggerHapticPulse(
                Mathf.Lerp(duration, duration * 1.5f, intensity),
                Mathf.Lerp(frequency, frequency * 1.5f, intensity),
                intensity,
                Controllers.GetDeviceFromHandType(Controllers.mainControllerType));
        }

        private void PlayReceiveDamageHaptics(float dmg, Vector3 direction)
        {
            if (dmg > .5)
            {
                dmg = dmg.RemapClamped(0, 10f, 0, .75f);

                float duration = 0.08f;
                float frequency = 55f;

                SteamVR_InputHandler.TriggerHapticPulse(Mathf.Lerp(duration, duration * 2.5f, dmg),
                    Mathf.Lerp(frequency, frequency * 1.3f, dmg),
                    Mathf.Lerp(0.5f, 1f, dmg),
                    Controllers.GetDeviceFromHandType(Controllers.offHandControllerType));

                SteamVR_InputHandler.TriggerHapticPulse(Mathf.Lerp(duration, duration * 2.5f, dmg),
                    Mathf.Lerp(frequency, frequency * 1.3f, dmg),
                    Mathf.Lerp(0.1f, 1f, dmg),
                    Controllers.GetDeviceFromHandType(Controllers.mainControllerType));
            }
        }

        private void OnDestroy()
        {
            PlayerReceivedDamageEvents.OnPlayerTakeDamage -= PlayReceiveDamageHaptics;
            PlayerFireWeaponEvents.OnPlayerFireWeapon -= PlayWeaponFireHaptics;
            PlayerReloadEvents.OnPlayerReloaded -= PlayWeaponReloadHaptics;
            GlueGunEvents.OnPressureUpdate -= GlueGunPressureHaptics;
            HammerEvents.OnHammerCharging -= HammerCharging;
            HammerEvents.OnHammerSmack -= HammerSmack;
        }
    }
}