using System;
using System.Collections.Generic;
using System.Text;
using GTFO_VR;
using GTFO_VR.Core;
using GTFO_VR.Events;
using GTFO_VR.Input;
using HarmonyLib;
using Player;
using UnityEngine;

/// <summary>
/// Patch methods responsible for handling camera rotation and position and sync them to the HMD input
/// Note: This does sync correctly in multiplayer because we use (more or less) the same pathways as mouse and gamepad input
/// </summary>

namespace GTFO_VR_BepInEx.Core
{

    // Patch position as raw HMD pos + player body position offset, everything is kept in world coords where possible
    [HarmonyPatch(typeof(FPSCamera),"RotationUpdate")]
    class InjectHMDPosition
    {

        static void Postfix(FPSCamera __instance, PlayerAgent ___m_owner)
        {
            if (!PlayerVR.VRSetup)
            {
                return;
            }
            if (VRSettings.VR_TRACKING_TYPE.Equals(TrackingType.PositionAndRotation) && !FocusStateManager.CurrentState.Equals(eFocusState.InElevator))
            {
                __instance.Position = ___m_owner.PlayerCharacterController.SmoothPosition + HMD.GetPosition();
            }
        }
    }

    
    // We inject pitch and yaw rotation data into the same method where mouse and gamepad input is being handled
    // This way lookDirection gets synced correctly to other players in multiplayer 

    [HarmonyPatch(typeof(FPSCamera), "UpdatePosOffset")]
    class InjectHMDRotationPitchYaw
    {

        static void Postfix(FPSCamera __instance, PlayerAgent ___m_owner)
        {
            if (!PlayerVR.VRSetup)
            {
                return;
            }

            // Repeat position inject or the transforms will get out of sync (Unity transform handling mumbo jumbo ensues, frame later or frame behind tracking)
            if (VRSettings.VR_TRACKING_TYPE.Equals(TrackingType.PositionAndRotation) && !FocusStateManager.CurrentState.Equals(eFocusState.InElevator))
            {
                __instance.Position = ___m_owner.PlayerCharacterController.SmoothPosition + HMD.GetPosition();
            }

            if ((VRSettings.VR_TRACKING_TYPE.Equals(TrackingType.PositionAndRotation) || VRSettings.VR_TRACKING_TYPE.Equals(TrackingType.Rotation)))
            {
                Vector3 euler = HMD.GetVRCameraEulerRotation();
                AccessTools.FieldRefAccess<LookCameraController, float>((LookCameraController)__instance, "m_pitch") = euler.x;
                AccessTools.FieldRefAccess<LookCameraController, float>((LookCameraController)__instance, "m_yaw") = euler.y;
            }

        }
    }

    // Roll is patched in a separate method because it does not exist as a field within FPSCamera
    // Since nothing else is done with the roll of the m_camera anywhere we can just set it and forget it
    [HarmonyPatch(typeof(FPSCamera), "RotationUpdate")]
    class InjectHMDRotationRoll
    {

        static void Postfix(FPSCamera __instance)
        {
            if(!PlayerVR.VRSetup)
            {
                return;
            }
            Vector3 euler = __instance.m_camera.transform.parent.localEulerAngles;
            euler = HMD.GetVRCameraEulerRotation();
            __instance.m_camera.transform.parent.localRotation = Quaternion.Euler(euler);
        }
    }
}
