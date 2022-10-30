using System;
using Player;
using UnityEngine;

namespace GTFO_VR.Core.PlayerBehaviours.BodyHaptics
{
    public class BodyHapticsUtils
    {
        public static readonly float LOW_HEALTH = 0.20f;
        public static readonly float MIN_HEALTH_GAIN_FOR_HAPTIC = 0.05f;

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