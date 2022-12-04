using GTFO_VR.Events;
using GTFO_VR.Util;
using SteamVR_Standalone_IL2CPP.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GTFO_VR.Core.PlayerBehaviours.ForceTube
{
    public static class ForceTube
    {
        public static void Setup()
        {
            ForceTubeVRInterface.InitAsync();
            Events.PlayerFireWeaponEvents.OnPlayerFireWeapon += WeaponFired;
            VRMeleeWeaponEvents.OnHammerSmack += HammerSmack;
        }

        private static void HammerSmack(float chargePercentage)
        {
            if (!VRConfig.configProtube.Value)
            {
                return;
            }

            byte intensity = 255;
            byte rumbleIntensity = 180;
            float rumbleDuration = 0.2f;

            var weaponData = WeaponArchetypeVRData.GetVRWeaponHapticData(ItemEquippableEvents.currentItem.PublicName);
            if (weaponData != null)
            {
                intensity = (byte)((float)weaponData.kickPower * Mathf.Max(0.1f, chargePercentage));
                rumbleIntensity = (byte)((float)weaponData.rumblePower * Mathf.Max(0.1f,chargePercentage));
                rumbleDuration = weaponData.rumbleDuration;
            }
            Log.Debug($"Effective melee force: kick:{intensity} intensity:{rumbleIntensity} duration:{rumbleDuration}");
            ForceTubeVRInterface.Shoot(intensity, rumbleIntensity, rumbleDuration, ForceTubeVRChannel.pistol1);
            ForceTubeVRInterface.Shoot(intensity, rumbleIntensity, rumbleDuration, ForceTubeVRChannel.rifle);
        }

        private static void WeaponFired(Weapon weapon)
        {
            if (!VRConfig.configProtube.Value)
            {
                return;
            }

            byte kickIntensity;
            byte rumbleIntensity;
            float rumbleDuration;

            var weaponData = WeaponArchetypeVRData.GetVRWeaponHapticData(ItemEquippableEvents.currentItem.PublicName);
            if (weaponData != null)
            {
                kickIntensity = weaponData.kickPower;
                rumbleIntensity = weaponData.rumblePower;
                rumbleDuration = weaponData.rumbleDuration;
            }
            else // If we have no data, fall back to a guesstimate
            {
                float strength = Mathf.Pow(Mathf.Max(1 / Mathf.Abs(weapon.RecoilData.horizontalScale.Max), 1 / Mathf.Abs(weapon.RecoilData.verticalScale.Max)), 2).RemapClamped(0, 8, 0.0f, 200.0f);
                rumbleDuration = 0.05f;
                kickIntensity = (byte)strength;
                rumbleIntensity = (byte)strength;
                Log.Debug($"Kick/Rumble strength = {strength}");
            }
            Log.Debug($"Kick intensity = {kickIntensity}");
            ForceTubeVRInterface.Shoot(kickIntensity, rumbleIntensity, rumbleDuration, ForceTubeVRChannel.pistol1);
            ForceTubeVRInterface.Shoot(kickIntensity, rumbleIntensity, rumbleDuration, ForceTubeVRChannel.rifle);
            ForceTubeVRInterface.Shoot(kickIntensity, rumbleIntensity, rumbleDuration, ForceTubeVRChannel.rifleBolt);
            ForceTubeVRInterface.Shoot(kickIntensity, rumbleIntensity, rumbleDuration, ForceTubeVRChannel.rifleButt);
        }
    }
}
