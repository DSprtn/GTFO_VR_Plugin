using GTFO_VR.Core;
using GTFO_VR.Core.VR_Input;
using HarmonyLib;
using UnityEngine;

/// <summary>
/// Patch methods responsible for handling camera rotation and position and sync them to the HMD input
/// Note: This does sync correctly in multiplayer because we use (more or less) the same pathways as mouse and gamepad input.
/// The exception being lookDir while holding a weapon with a flashlight, which needs to be synced differently.
/// </summary>

namespace GTFO_VR.Injections.Gameplay
{
    // Patch position as raw HMD pos + player body position offset, everything is kept in world coords where possible
    [HarmonyPatch(typeof(FPSCamera), nameof(FPSCamera.RotationUpdate))]
    internal class InjectHMDPosition
    {
        private static void Prefix(FPSCamera __instance)
        {
            Vector3 euler = HMD.GetVRCameraEulerRelativeToFPSCameraParent();
            __instance.m_pitch = euler.x;
            __instance.m_yaw = euler.y;
        }
    }

    // We inject pitch and yaw rotation data into the same method where mouse and gamepad input is being handled
    // This way lookDirection gets synced correctly to other players in multiplayer

    [HarmonyPatch(typeof(FPSCamera), nameof(FPSCamera.UpdatePosOffset))]
    internal class InjectHMDRotationPitchYaw
    {
        private static void Postfix(FPSCamera __instance)
        {
            // Repeat position inject or the transforms will get out of sync (Unity transform handling mumbo jumbo ensues, frame later or frame behind tracking)
            if (!FocusStateManager.CurrentState.Equals(eFocusState.InElevator))
            {
                __instance.Position = HMD.GetWorldPosition();
            }

            Vector3 euler = HMD.GetVRCameraEulerRelativeToFPSCameraParent();
            __instance.m_pitch = euler.x;
            __instance.m_yaw = euler.y;
        }
    }

    [HarmonyPatch(typeof(FPSCamera), nameof(FPSCamera.RotationUpdate))]
    internal class InjectHMDRotationRoll
    {
        private static void Postfix(FPSCamera __instance)
        {
            if (!FocusStateManager.CurrentState.Equals(eFocusState.InElevator))
            {
                __instance.Position = HMD.GetWorldPosition();
            }

            Vector3 euler = __instance.m_camera.transform.parent.localEulerAngles;
            euler.z = HMD.GetVRCameraEulerRelativeToFPSCameraParent().z;
            __instance.m_camera.transform.parent.localRotation = Quaternion.Euler(euler);
            __instance.UpdateFlatTransform();
        }
    }
}