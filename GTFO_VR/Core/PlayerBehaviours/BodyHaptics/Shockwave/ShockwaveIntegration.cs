using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Agents;
using GTFO_VR.Core.PlayerBehaviours.BodyHaptics.Shockwave.Engine;
using GTFO_VR.Core.VR_Input;
using Player;
using UnityEngine;

namespace GTFO_VR.Core.PlayerBehaviours.BodyHaptics.Shockwave
{
    public class ShockwaveIntegration : BodyHapticAgent
    {
        private LocalPlayerAgent m_player;
        private OrientationSettings m_lastDamageOrientationSettings;
        private PlayerLocomotion.PLOC_State m_lastLocState;
        private float m_lastHealth = 1f;
        private bool m_isReloading;
        private bool m_isLowHealth;
        private bool m_isBioScanning;

        public void Setup(LocalPlayerAgent player)
        {
            m_player = player;
            m_lastLocState = player.Locomotion.m_currentStateEnum;
        }

        private void SendOrientedHapticPulse(ShockwaveManager.HapticRegion region, float strength, int regionHeight,
            float duration, OrientationSettings orientationSettings)
        {
            float angYaw = orientationSettings.OffsetAngleX;
            int longitudinalPosition = (int) Math.Round((orientationSettings.OffsetY + 0.5f) * regionHeight); // convert from [-0.5, 0.5] to [0, regionHeight]

            ShockwaveManager.Instance.sendHapticsPulsewithPositionInfo(region, strength,
                angYaw, longitudinalPosition, regionHeight, duration);
        }

        public void Update()
        {
        }

        public void HammerSmackHaptics(float dmg)
        {
            List<List<int>> rightIndices = new List<List<int>>
            {
                new List<int>{ 54, 55 },
                new List<int>{ 52, 53 },
                new List<int>{ 50, 51 },
                new List<int>{ 48, 49 },
                new List<int>{ 30, 38, 39, 29, 37, 36 },
                new List<int>{ 22, 31, 32, 21, 28, 35 },
                new List<int>{ 14, 23, 24, 13, 20, 27 },
                new List<int>{ 15, 16, 25, 12, 19, 26 },
                new List<int>{ 0, 8, 17, 3, 11, 18 },
                new List<int>{ 1, 9, 2, 10 },
                new List<int>{ 56, 57 },
                new List<int>{ 58, 59 },
            };

            HapticIndexPattern rightPattern = new HapticIndexPattern(rightIndices, 1.0f, 30);
            HapticIndexPattern pattern = (Controllers.MainControllerType == HandType.Left) ? ShockwaveEngine.GetPatternMirror(rightPattern) : rightPattern;
            ShockwaveEngine.PlayPattern(pattern);
        }

        public void HammerFullyChargedHaptics()
        {
            List<List<int>> rightIndices = new List<List<int>>
            {
                new List<int>{ 54 },
                new List<int>{ 52, 53 },
                new List<int>{ 50, 51 },
                new List<int>{ 48, 49 },
                new List<int>{ 38, 37 },
                new List<int>{ 30, 39, 29, 36 },
                new List<int>{ 31, 24, 27, 28 },
                new List<int>{ 23, 16, 19, 20 },
                new List<int>{ 8, 9, 10, 11 },
                new List<int>{ 1, 2 },
            };

            HapticIndexPattern rightPattern = new HapticIndexPattern(rightIndices, 0.8f, 40);
            HapticIndexPattern pattern = (Controllers.MainControllerType == HandType.Left) ? ShockwaveEngine.GetPatternMirror(rightPattern) : rightPattern;
            ShockwaveEngine.PlayPattern(pattern);
        }

        public void HammerChargingHaptics(float pressure)
        {
            if (IsInElevator())
            {
                return;
            }

            int[] rightArmIndices = { 54, 53 };
            int[] indices = (Controllers.MainControllerType == HandType.Left) ? ShockwaveEngine.GetPatternMirror(rightArmIndices) : rightArmIndices;
            ShockwaveEngine.PlayPattern(new HapticIndexPattern(indices, 0.4f, 25));
        }

        private async void PlayWeaponReloadHapticsFunc()
        {
            const int delay = 150;

            int[] rightArmIndices = { 54, 52, 50, 51, 53, 55 };
            int[] armIndices = (Controllers.MainControllerType == HandType.Left) ? ShockwaveEngine.GetPatternMirror(rightArmIndices) : rightArmIndices;
            HapticIndexPattern armPattern = new HapticIndexPattern(armIndices, 0.5f, delay);

            int[] rightBodyIndices = { 38, 39, 31, 23, 22, 30 };
            int[] bodyIndices = (Controllers.MainControllerType == HandType.Left) ? ShockwaveEngine.GetPatternMirror(rightBodyIndices) : rightBodyIndices;
            HapticIndexPattern bodyPattern = new HapticIndexPattern(bodyIndices, 0.2f, delay);

            Debug.Assert(armIndices.Length == bodyIndices.Length && armPattern.delay == bodyPattern.delay,
                "Arm and body reload haptics should have same indices length and delay, or the Task.Delay must be adapted to match both durations");
            int sleepDuration = bodyIndices.Length * bodyPattern.delay;

            while (m_isReloading)
            {
                ShockwaveEngine.PlayPattern(armPattern);
                ShockwaveEngine.PlayPattern(bodyPattern);
                await Task.Delay(sleepDuration);
            }
        }

        public void PlayWeaponReloadHaptics()
        {
            m_isReloading = true;
            PlayWeaponReloadHapticsFunc();
        }

        public void StopWeaponReloadHaptics()
        {
            m_isReloading = false;
        }

        public void PlayWeaponFireHaptics(Weapon weapon)
        {
			float intensity = Haptics.GetFireHapticStrength(weapon);

			if (Controllers.MainControllerType == HandType.Left || Controllers.AimingTwoHanded)
            {
                var pattern = new HapticGroupPattern(new List<HapticGroupInfo>
                {
                    new HapticGroupInfo(ShockwaveManager.HapticGroup.LEFT_FOREARM, 1f * intensity),
                    new HapticGroupInfo(ShockwaveManager.HapticGroup.LEFT_ARM, 0.8f * intensity),
                    new HapticGroupInfo(ShockwaveManager.HapticGroup.LEFT_BICEP, 0.4f * intensity),
                    new HapticGroupInfo(ShockwaveManager.HapticGroup.LEFT_SHOULDER, 0.05f * intensity),
                }, 10);
                ShockwaveEngine.PlayPattern(pattern);
            }

			if (Controllers.MainControllerType == HandType.Right || Controllers.AimingTwoHanded)
            {
                var pattern = new HapticGroupPattern(new List<HapticGroupInfo>
                {
                    new HapticGroupInfo(ShockwaveManager.HapticGroup.RIGHT_FOREARM, 1f * intensity),
                    new HapticGroupInfo(ShockwaveManager.HapticGroup.RIGHT_ARM, 0.8f * intensity),
                    new HapticGroupInfo(ShockwaveManager.HapticGroup.RIGHT_BICEP, 0.4f * intensity),
                    new HapticGroupInfo(ShockwaveManager.HapticGroup.RIGHT_SHOULDER, 0.05f * intensity),
                }, 10);
                ShockwaveEngine.PlayPattern(pattern);
            }
        }

        public void PlayReceiveDamageHaptics(float dmg, Vector3 direction)
        {
            OrientationSettings orientationSettings = BodyHapticsUtils.GetOrientationSettingsFromDirection(m_player, direction);
            SendOrientedHapticPulse(ShockwaveManager.HapticRegion.TORSO, 1, 10, 50, orientationSettings);
			m_lastDamageOrientationSettings = orientationSettings;
        }

        private void SendTorsoOrientedHaptic(float offsetAngleX, float offsetY, float strength = 1f, int duration = 150)
        {
            const int regionHeight = 10;
            SendOrientedHapticPulse(ShockwaveManager.HapticRegion.TORSO, strength, regionHeight, duration,
                new OrientationSettings(offsetAngleX, offsetY));
        }

        public async void MineExplosionHaptics(OrientationSettings orientation, float intensity)
        {
            const float offsetYStep = 0.2f;
            const float offsetAngleXStep = 45f;
            const int range = 3;

            const int delay = 150;
            SendTorsoOrientedHaptic(orientation.OffsetAngleX, orientation.OffsetY);
            await Task.Delay(delay);

            for (int i = 1; i <= range; i++)
            {
                float strength = intensity - ((range - (i-1)) * 0.2f);

                float down = orientation.OffsetY - (offsetYStep * i);
                float up = orientation.OffsetY + (offsetYStep * i);
                float left = BodyHapticsUtils.NormalizeOrientation(orientation.OffsetAngleX - offsetAngleXStep * i);
                float right = BodyHapticsUtils.NormalizeOrientation(orientation.OffsetAngleX + offsetAngleXStep * i);

                SendTorsoOrientedHaptic(orientation.OffsetAngleX, down, strength);
                SendTorsoOrientedHaptic(orientation.OffsetAngleX, up, strength);
                SendTorsoOrientedHaptic(left, orientation.OffsetY, strength);
                SendTorsoOrientedHaptic(right, orientation.OffsetY, strength);
                SendTorsoOrientedHaptic(left, up, strength);
                SendTorsoOrientedHaptic(right, up, strength);
                SendTorsoOrientedHaptic(left, down, strength);
                SendTorsoOrientedHaptic(right, down, strength);

                ShockwaveEngine.PlayPattern(new HapticIndexPattern(
                    new[,]{{ 52, 50, 42, 44, 43, 45, 51, 53, 64, 66, 56, 58, 57, 59, 65, 67 }}, strength * 0.5f, delay));

                await Task.Delay(delay);
            }
        }

        public async void TentacleAttackHaptics(float dmg, Agent sourceAgent, Vector3 position)
        {
			if (m_lastDamageOrientationSettings != null)
            {
                const float offsetYStep = 0.2f;
                const float offsetAngleXStep = 45f;
                const float strength = 1f;
                const int duration = 80;
				var orientation = m_lastDamageOrientationSettings;

                for (int i = 1; i <= 3; i++)
                {
                    SendTorsoOrientedHaptic(orientation.OffsetAngleX, orientation.OffsetY - (offsetYStep * i), strength, duration);
                    SendTorsoOrientedHaptic(orientation.OffsetAngleX, orientation.OffsetY + (offsetYStep * i), strength, duration);
                    SendTorsoOrientedHaptic(BodyHapticsUtils.NormalizeOrientation(orientation.OffsetAngleX - (offsetAngleXStep * i)),
                        orientation.OffsetY, strength, duration);
                    SendTorsoOrientedHaptic(BodyHapticsUtils.NormalizeOrientation(orientation.OffsetAngleX + (offsetAngleXStep * i)),
                        orientation.OffsetY, strength, duration);
                    await Task.Delay(duration);
                }
            }
        }

        public async void LandedFromElevator(eFocusState focusState)
        {
            await Task.Delay(500);
            ShockwaveEngine.PlayPattern(new HapticIndexPattern(BodyHapticsIndices.FeetToShoulders, 1f, 30));
        }

        public void PlayerInteractedHaptics(PlayerAgent source)
        {
            int[] rightArmIndices = { 54, 55 };
            int[] indices = (Controllers.MainControllerType == HandType.Left) ? ShockwaveEngine.GetPatternMirror(rightArmIndices) : rightArmIndices;
            ShockwaveEngine.PlayPattern(new HapticIndexPattern(indices, 0.6f, 25));
        }

        private async void PlayBioscanPatternFunc()
        {
            List<List<int>> RightToLeft = new List<List<int>>
            {
                new List<int>{ 54, 52, 50, 48, 55, 53, 51, 49 },
                new List<int>{ 70, 68, 66, 64, 6, 14, 22, 30, 38, 71, 69, 67, 65, 5, 13, 21, 29, 37 },
                new List<int>{ 7, 15, 23, 31, 39, 4, 12, 20, 28, 36 },
                new List<int>{ 0, 8, 16, 24, 32, 3, 11, 19, 27, 35 },
                new List<int>{ 62, 60, 58, 56, 1, 9, 17, 25, 33, 63, 61, 59, 57, 2, 10, 18, 26, 34 },
                new List<int>{ 46, 44, 42, 40, 47, 45, 43, 41 },
            };

            int i = 0;
            while (m_isBioScanning)
            {
                HapticIndexPattern pattern = null;
                switch (i % 4)
                {
                    case 0:
                        pattern = new HapticIndexPattern(BodyHapticsIndices.FeetToShoulders, 0.4f, 300);
                        break;
                    case 1:
                        pattern = new HapticIndexPattern(BodyHapticsIndices.ShouldersToFeet, 0.4f, 300);
                        break;
                    case 2:
                        pattern = new HapticIndexPattern(RightToLeft, 0.4f, 300);
                        break;
                    case 3:
                        pattern = ShockwaveEngine.GetPatternMirror(new HapticIndexPattern(RightToLeft, 0.4f, 300));
                        break;
                }
                await ShockwaveEngine.PlayPatternFunc(pattern, () => !m_isBioScanning);
                await Task.Delay(400);
                i++;
            }
        }

        public void PlayBioscanHaptics()
        {
            m_isBioScanning = true;
            PlayBioscanPatternFunc();
        }

        public void StopBioscanHaptics()
        {
            m_isBioScanning = false;
        }

        public void FlashlightToggledHaptics()
        {
            int[] rightArmIndices = { 54 };
            int[] indices = (Controllers.MainControllerType == HandType.Left) ? ShockwaveEngine.GetPatternMirror(rightArmIndices) : rightArmIndices;
            ShockwaveEngine.PlayPattern(new HapticIndexPattern(indices, 0.6f, 25));
        }

        private HapticIndexPattern GetSidePattern(HapticIndexPattern rightSidePattern, bool isLeft)
        {
            if (isLeft)
            {
                return ShockwaveEngine.GetPatternMirror(rightSidePattern);
            }
            else
            {
                return rightSidePattern;
            }
        }

        public void PlayerChangedItemHaptics(ItemEquippable item)
        {
            if (IsInElevator())
            {
                return;
            }

            StopWeaponReloadHaptics();

            bool isLeftHand = Controllers.MainControllerType == HandType.Left;
            int delay = 50;
            ShockwaveEngine.PlayPattern(GetSidePattern(new HapticIndexPattern(new[,]{{48, 49}}, 0.4f, delay), isLeftHand));
            ShockwaveEngine.PlayPattern(GetSidePattern(new HapticIndexPattern(new[,]{{50, 51}}, 0.5f, delay), isLeftHand));
            ShockwaveEngine.PlayPattern(GetSidePattern(new HapticIndexPattern(new[,]{{52, 53}}, 0.6f, delay), isLeftHand));
            ShockwaveEngine.PlayPattern(GetSidePattern(new HapticIndexPattern(new[,]{{54, 55}}, 0.8f, delay), isLeftHand));
        }

        private void PlayGainAmmoPattern()
        {
            const float intensity = 0.7f;
            const int delay = 300;

            int[,] rightArmIndices =
            {
                { 54, 55 },
                { 52, 53 },
                { 50, 51 },
                { 48, 49 },
            };
            ShockwaveEngine.PlayPattern(new HapticIndexPattern(rightArmIndices, intensity, delay));

            int[,] leftArmIndices =
            {
                { 46, 47 },
                { 44, 45 },
                { 42, 43 },
                { 40, 41 },
            };
            ShockwaveEngine.PlayPattern(new HapticIndexPattern(leftArmIndices, intensity, delay));

            int[,] bodyIndices =
            {
                { 0, 1, 2, 3, 4, 5, 6, 7 },
                { 8, 9, 10, 11, 12, 13, 14, 15 },
                { 16, 17, 18, 19, 20, 21, 22, 23 },
                { 24, 25, 26, 27, 28, 29, 30, 31 },
                { 32, 33, 34, 35, 36, 37, 38, 39 },
            };
            ShockwaveEngine.PlayPattern(new HapticIndexPattern(bodyIndices, intensity, delay));
        }

        private async void PlayGainToolsPattern()
        {
            const float intensity = 0.7f;

            for (int i = 0; i < 2; i++)
            {
                int[,] rightArmIndices;
                int[,] leftArmIndices;
                int[,] bodyIndices;

                if (i == 0)
                {
                    rightArmIndices = new[,] { { 52, 53 } };
                    leftArmIndices = new[,] { { 44, 45 } };
                    bodyIndices = new[,] { { 8, 9, 10, 11, 12, 13, 14, 15 } };
                }
                else
                {
                    rightArmIndices = new[,] { { 50, 51 } };
                    leftArmIndices = new[,] { { 42, 43 } };
                    bodyIndices = new[,] { { 24, 25, 26, 27, 28, 29, 30, 31 } };
                }

                for (int j = 0; j < 2; j++)
                {
                    int delay = 100;
                    ShockwaveEngine.PlayPattern(new HapticIndexPattern(rightArmIndices, intensity, delay));
                    ShockwaveEngine.PlayPattern(new HapticIndexPattern(leftArmIndices, intensity, delay));
                    ShockwaveEngine.PlayPattern(new HapticIndexPattern(bodyIndices, intensity, delay));
                    await Task.Delay(delay + 100);
                }

                await Task.Delay(300);
            }
        }

        public void AmmoGainedHaptics(float ammoStandardRel, float ammoSpecialRel, float ammoClassRel)
        {
            if (ammoStandardRel > 0 || ammoSpecialRel > 0)
            {
                PlayGainAmmoPattern();
            }
            else if (ammoClassRel > 0)
            {
                PlayGainToolsPattern();
            }
        }

        public async void InfectionHealed(float infection)
        {
            int[] indices = {
                6, 14, 22, 30, 38, 39, 31, 23, 15, 7, 0, 8, 16, 24, 32, 33, 25, 17, 9, 1,
                2, 10, 18, 26, 34, 35, 27, 19, 11, 3, 4, 12, 20, 28, 36, 37, 29, 21, 13, 5
            };
            HapticIndexPattern pattern = new HapticIndexPattern(indices, 0.8f, 40);
            await ShockwaveEngine.PlayPatternFunc(pattern);

            int[,] limbsIndices = {
                { 48, 40, 41, 49, 64, 56, 57, 65 },
                { 50, 42, 43, 51, 66, 58, 59, 67 },
                { 52, 44, 45, 53, 68, 60, 61, 69 },
                { 54, 46, 47, 55, 70, 62, 63, 71 },
            };
            pattern = new HapticIndexPattern(limbsIndices, 0.5f, 100);
            await ShockwaveEngine.PlayPatternFunc(pattern);
        }

        private async void PlayLifeGainPattern()
        {
            const int bodyPointsCount = 40;
            int[] indices = new int[bodyPointsCount];
            for (int i = 0; i < bodyPointsCount; i++)
            {
                indices[i] = i;
            }

            HapticIndexPattern bodyPattern = new HapticIndexPattern(indices, 0.8f, 40);
            await ShockwaveEngine.PlayPatternFunc(bodyPattern);

            indices = new[]{ 31, 24, 16, 8, 15, 7, 4, 12, 11, 19, 27, 28, 36, 35 };
            bodyPattern = new HapticIndexPattern(indices, 0.8f, 60);
            await ShockwaveEngine.PlayPatternFunc(bodyPattern);
        }

        private async void PlayHeartbeatPattern()
        {
            const float VERY_LOW_HEALTH = 0.10f;

            while (m_isLowHealth)
            {
                float intensity = 0.2f;
                int delay = 1000;
                if (m_lastHealth <= VERY_LOW_HEALTH)
                {
                    delay = 600;
                    intensity = 0.4f;
                }

                ShockwaveEngine.PlayPattern(new HapticIndexPattern(new[,]
                {
                    {16, 17, 24},
                }, intensity, 25));
                await Task.Delay(185);
                ShockwaveEngine.PlayPattern(new HapticIndexPattern(new[,]
                {
                    {16, 17, 24},
                }, intensity, 25));

                await Task.Delay(delay);
            }
        }

        private void PlayDeathPattern()
        {
            ShockwaveEngine.PlayPattern(new HapticIndexPattern(BodyHapticsIndices.ShouldersToFeet, 1.0f, 40));
        }

        public void OnHealthUpdated(float health)
        {
            if (health <= BodyHapticsUtils.LOW_HEALTH && !m_isLowHealth)
            {
                m_isLowHealth = true;
                PlayHeartbeatPattern();
            }
            else if (health > BodyHapticsUtils.LOW_HEALTH && m_isLowHealth)
            {
                m_isLowHealth = false;
            }

            if (health - m_lastHealth > BodyHapticsUtils.MIN_HEALTH_GAIN_FOR_HAPTIC) // Gained some health
            {
                PlayLifeGainPattern();
            }

            if (health <= 0 && m_lastHealth > 0) // Player is dead
            {
                StopWeaponReloadHaptics();
                m_isBioScanning = false;

                PlayDeathPattern();

                Task.Run(async () => // Stop heartbeat after a couple seconds
                {
                    await Task.Delay(3000);
                    m_isLowHealth = false;
                });
            }

            m_lastHealth = health;
        }

        public async void WeaponAmmoEmpty(bool leftArm)
        {
            if (IsInElevator())
            {
                return;
            }

            await Task.Delay(50);
            int[] rightArmIndices = { 54, 55 };
            int[] indices = leftArm ? ShockwaveEngine.GetPatternMirror(rightArmIndices) : rightArmIndices;
            ShockwaveEngine.PlayPattern(new HapticIndexPattern(indices, 1.0f, 25));
        }

        public void OnPlayerLocomotionStateChanged(PlayerLocomotion.PLOC_State state)
        {
            if ((m_lastLocState == PlayerLocomotion.PLOC_State.Fall || m_lastLocState == PlayerLocomotion.PLOC_State.Jump)
                && (state == PlayerLocomotion.PLOC_State.Stand || state == PlayerLocomotion.PLOC_State.Crouch))
            {
                var indices = new List<List<int>>
                {
                    new List<int>{ 8, 9, 10, 11, 12, 13, 14, 15 },
                    new List<int>{ 0, 1, 2, 3, 4, 5, 6, 7 },
                    new List<int>{ 64, 56, 57, 65 },
                    new List<int>{ 66, 58, 59, 67 },
                    new List<int>{ 68, 60, 61, 69 },
                    new List<int>{ 70, 62, 63, 71 },
                };
                ShockwaveEngine.PlayPattern(new HapticIndexPattern(indices, 0.4f, 25));
            }

            m_lastLocState = state;
        }

        public void CrouchToggleHaptics(bool isCrouched)
        {
            if (IsInElevator())
            {
                return;
            }

            if (isCrouched)
            {
                var indices = new List<List<int>>
                {
                    new List<int>{ 8, 9, 10, 11, 12, 13, 14, 15 },
                    new List<int>{ 0, 1, 2, 3, 4, 5, 6, 7 },
                    new List<int>{ 64, 56, 57, 65 },
                };
                ShockwaveEngine.PlayPattern(new HapticIndexPattern(indices, 0.4f, 150));
            }
            else
            {
                var indices = new List<List<int>>
                {
                    new List<int>{ 64, 56, 57, 65 },
                    new List<int>{ 0, 1, 2, 3, 4, 5, 6, 7 },
                    new List<int>{ 8, 9, 10, 11, 12, 13, 14, 15 },
                };
                ShockwaveEngine.PlayPattern(new HapticIndexPattern(indices, 0.4f, 150));
            }
        }

        private bool IsInElevator()
        {
            return m_lastLocState == PlayerLocomotion.PLOC_State.InElevator;
        }
    }
}