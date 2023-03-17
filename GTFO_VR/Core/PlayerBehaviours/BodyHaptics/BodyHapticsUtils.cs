using System;
using System.Collections.Generic;
using Player;
using UnityEngine;

namespace GTFO_VR.Core.PlayerBehaviours.BodyHaptics
{
    public class BodyHapticsUtils
    {
        public static readonly float LOW_HEALTH = 0.20f;
        public static readonly float MIN_HEALTH_GAIN_FOR_HAPTIC = 0.05f;

        public static readonly float ELEVATOR_RIDE_FEEDBACK_DURATION = 0.75f;

        public static float Clamp(float v, float min, float max)
        {
            return Math.Min(Math.Max(v, min), max);
        }

        public static float NormalizeOrientation(float orientation)
        {
            float result = orientation % 360;

            if (result < 0)
            {
                result += 360;
            }

            return result;
        }

        public static OrientationSettings GetOrientationSettingsFromDirection(LocalPlayerAgent player, Vector3 direction)
        {
            // Direction coordinates are [-1, 1]
            float angleRadians = (float) Math.Atan2(direction.z, direction.x);
            float angleDegrees = (float) (angleRadians * 180 / Math.PI);
            float cameraYRotation = player.FPSCamera.Rotation.eulerAngles.y;
            float offsetAngleX = NormalizeOrientation(angleDegrees + cameraYRotation + 90f);
            float offsetY = Clamp(0.5f - (direction.y * 2), -0.5f, 0.5f);
            return new OrientationSettings(offsetAngleX, offsetY);
        }

        public static float GetElevatorRideDurationScale()
        {
            const float MAX_ELEVATOR_VELOCITY = 400f;
            const float MIN_PATTERN_SCALE = 0.6f;
            const float MAX_PATTERN_SCALE = 1.3f;
            return MAX_PATTERN_SCALE - ((ElevatorRide.CurrentVelocity / MAX_ELEVATOR_VELOCITY) * (MAX_PATTERN_SCALE - MIN_PATTERN_SCALE));
        }
    }

    public class BodyHapticsIndices
    {
        public static readonly List<List<int>> FeetToShoulders = new List<List<int>>
        {
            new List<int>{ 70, 62, 63, 71 },
            new List<int>{ 68, 60, 61, 69 },
            new List<int>{ 66, 58, 59, 67 },
            new List<int>{ 64, 56, 57, 65 },
            new List<int>{ 0, 1, 2, 3, 4, 5, 6, 7 },
            new List<int>{ 8, 9, 10, 11, 12, 13, 14, 15, 54, 46, 47, 55 },
            new List<int>{ 16, 17, 18, 19, 20, 21, 22, 23, 52, 44, 45, 53 },
            new List<int>{ 24, 25, 26, 27, 28, 29, 30, 31, 50, 42, 43, 51 },
            new List<int>{ 32, 33, 34, 35, 36, 37, 38, 39, 48, 40, 41, 49 },
        };

        public static readonly List<List<int>> ShouldersToFeet = new List<List<int>>
        {
            new List<int>{ 32, 33, 34, 35, 36, 37, 38, 39, 48, 40, 41, 49 },
            new List<int>{ 24, 25, 26, 27, 28, 29, 30, 31, 50, 42, 43, 51 },
            new List<int>{ 16, 17, 18, 19, 20, 21, 22, 23, 52, 44, 45, 53 },
            new List<int>{ 8, 9, 10, 11, 12, 13, 14, 15, 54, 46, 47, 55 },
            new List<int>{ 0, 1, 2, 3, 4, 5, 6, 7 },
            new List<int>{ 64, 56, 57, 65 },
            new List<int>{ 66, 58, 59, 67 },
            new List<int>{ 68, 60, 61, 69 },
            new List<int>{ 70, 62, 63, 71 },
        };
    }

    public class OrientationSettings
    {
        public float OffsetAngleX { get; } // [0, 360]
        public float OffsetY { get; } // [-0.5, 0.5]

        public OrientationSettings(float offsetAngleX, float offsetY)
        {
            OffsetAngleX = offsetAngleX;
            OffsetY = offsetY;
        }
    }
}