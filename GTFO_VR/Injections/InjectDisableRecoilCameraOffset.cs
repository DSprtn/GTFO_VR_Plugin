using HarmonyLib;
using UnityEngine;

namespace GTFO_VR.Injections
{
    /// <summary>
    /// Disables weapon recoil impacting the camera.
    /// </summary>

    [HarmonyPatch(typeof(FPSCamera), nameof(FPSCamera.RotationUpdate))]
    class InjectDisableRecoilOnCameraApply
    {
        static void Prefix(FPSCamera __instance)
        {
            __instance.m_recoilSystem.m_hasOverrideParentRotation = false;
        }
    }

    [HarmonyPatch(typeof(FPS_RecoilSystem), nameof(FPS_RecoilSystem.FPS_Update))]
    class InjectDisableRecoilOnCamera
    {
        static void Postfix(FPS_RecoilSystem __instance)
        {
            __instance.transform.localEulerAngles = Vector3.zero;
            Shader.SetGlobalVector(__instance.m_FPS_SightOffset_PropertyID, Vector4.zero);
        }
    }
}
