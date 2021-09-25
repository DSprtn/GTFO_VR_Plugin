using GTFO_VR.Core;
using GTFO_VR.Core.PlayerBehaviours;
using GTFO_VR.Core.VR_Input;
using GTFO_VR.Events;
using HarmonyLib;
using Player;
using UnityEngine;

namespace GTFO_VR.Injections.Gameplay
{
    /// <summary>
    /// Change detection to use weapon flashlight direction (when you're the host or playing solo)
    /// </summary>
    [HarmonyPatch(typeof(PlayerAgent), nameof(PlayerAgent.GetDetectionMod))]
    internal class InjectWeaponAimFlashlightAggro
    {
        private static void Postfix(PlayerAgent __instance, Vector3 dir, float distance, ref float __result)
        {
            if(!VRConfig.configUseControllers.Value || !__instance.IsLocallyOwned)
            {
                return;
            }
            __result = VRDetectionMod(dir, distance, __instance.Inventory.m_flashlight.range, __instance.Inventory.m_flashlight.spotAngle);
        }

        // ToDO - Replace this with patch, hopefully
        public static float VRDetectionMod(Vector3 dir, float distance, float m_flashLightRange, float m_flashlight_spotAngle)
        {
            if (distance > m_flashLightRange)
            {
                return 0.0f;
            }
            Vector3 VRLookDir = HMD.GetWorldForward();
            if (ItemEquippableEvents.CurrentItemHasFlashlight() && VRConfig.configUseControllers.Value)
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