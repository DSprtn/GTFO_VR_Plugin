using GTFO_VR.Core;
using GTFO_VR.Core.PlayerBehaviours;
using GTFO_VR.Core.VR_Input;
using HarmonyLib;
using UnityEngine;

/// <summary>
/// Patch methods responsible for handling camera rotation and position and sync them to the HMD input
/// Note: This does sync correctly in multiplayer because we use (more or less) the same pathways as mouse and gamepad input
/// </summary>

namespace GTFO_VR_BepInEx.Core
{

    // Patch position as raw HMD pos + player body position offset, everything is kept in world coords where possible
    [HarmonyPatch(typeof(FPSCamera),nameof(FPSCamera.RotationUpdate))]
    class InjectHMDPosition
    {

        static void Prefix(FPSCamera __instance)
        {
            if (!PlayerVR.VRPlayerIsSetup)
            {
                return;
            }
            Vector3 euler = HMD.GetVRCameraEulerRotation();
            __instance.m_pitch = euler.x;
            __instance.m_yaw = euler.y;

        }
    }

    
    // We inject pitch and yaw rotation data into the same method where mouse and gamepad input is being handled
    // This way lookDirection gets synced correctly to other players in multiplayer 

    [HarmonyPatch(typeof(FPSCamera), nameof(FPSCamera.UpdatePosOffset))]
    class InjectHMDRotationPitchYaw
    {

        static void Postfix(FPSCamera __instance)
        {
            if (!PlayerVR.VRPlayerIsSetup)
            {
                return;
            }
            
            // Repeat position inject or the transforms will get out of sync (Unity transform handling mumbo jumbo ensues, frame later or frame behind tracking)
            if (VR_Settings.VR_TRACKING_TYPE.Equals(TrackingType.PositionAndRotation) && !FocusStateManager.CurrentState.Equals(eFocusState.InElevator))
            {
                __instance.Position = HMD.GetWorldPosition();
            }

            if (VR_Settings.VR_TRACKING_TYPE.Equals(TrackingType.PositionAndRotation) || VR_Settings.VR_TRACKING_TYPE.Equals(TrackingType.Rotation))
            {
                Vector3 euler = HMD.GetVRCameraEulerRotation();
                __instance.m_pitch = euler.x;
                __instance.m_yaw = euler.y;
            }
        }
    }

    // Roll is patched in a separate method because it does not exist as a field within FPSCamera
    // Since nothing else is done with the roll of the m_camera anywhere we can just set it and forget it
    [HarmonyPatch(typeof(FPSCamera), nameof(FPSCamera.RotationUpdate))]
    class InjectHMDRotationRoll
    {

        static void Postfix(FPSCamera __instance)
        {
            if(!PlayerVR.VRPlayerIsSetup)
            {
                return;
            }
            if (VR_Settings.VR_TRACKING_TYPE.Equals(TrackingType.PositionAndRotation) && !FocusStateManager.CurrentState.Equals(eFocusState.InElevator))
            {
                __instance.Position = HMD.GetWorldPosition();
            }

            Vector3 euler = __instance.m_camera.transform.parent.localEulerAngles;
            euler.z = HMD.GetVRCameraEulerRotation().z;
            __instance.m_camera.transform.parent.localRotation = Quaternion.Euler(euler);
            __instance.UpdateFlatTransform();
        }
    }
}
