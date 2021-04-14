using GTFO_VR.Core.VR_Input;
using GTFO_VR.Events;
using UnityEngine;

namespace GTFO_VR.Core.PlayerBehaviours
{
    internal class VRDetectionModHack
    {
        // ToDO - Remove this and replace it with a pre/post FPS camera patch.
        public static float VRDetectionMod(Vector3 dir, float distance, float m_flashLightRange, float m_flashlight_spotAngle)
        {
            if (distance > m_flashLightRange)
            {
                return 0.0f;
            }
            Vector3 VRLookDir = HMD.GetWorldForward();
            if (ItemEquippableEvents.CurrentItemHasFlashlight() && VRSettings.useVRControllers)
            {
                VRLookDir = Controllers.GetAimForward();
            }
            float angleDiff = Vector3.Angle(dir, -VRLookDir);
            float spotlightAngleSize = m_flashlight_spotAngle * 0.5f;
            if (angleDiff > spotlightAngleSize)
                return 0.0f;
            float distanceMultiplier = 1.0f - distance / m_flashLightRange;
            return Mathf.Min((1.0f - angleDiff / spotlightAngleSize) * distanceMultiplier, 0.2f);
        }
    }
}