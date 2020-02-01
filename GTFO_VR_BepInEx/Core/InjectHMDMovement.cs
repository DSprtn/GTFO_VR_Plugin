using System;
using System.Collections.Generic;
using System.Text;
using GTFO_VR;
using HarmonyLib;
using Player;
using UnityEngine;

/// <summary>
/// Patch methods responsible for handling camera rotation and position and sync them to the HMD input
/// Note: This does sync correctly in multiplayer because we use (more or less) the same pathways as mouse and gamepad input
/// </summary>

namespace GTFO_VR_BepInEx.Core
{
    [HarmonyPatch(typeof(FPSCamera),"RotationUpdate")] // Trust me on this one
    class InjectHMDPosition
    {

        static void Postfix(FPSCamera __instance, PlayerAgent ___m_owner)
        {
            if (!VRInitiator.VR_ENABLED)
            {
                return;
            }
            if (VRInitiator.VR_TRACKING_TYPE.Equals(TrackingType.PositionAndRotation))
            {
                __instance.Position = ___m_owner.PlayerCharacterController.SmoothPosition + VRInitiator.hmd.transform.position;
            }
        }
    }

    
    // We inject pitch and yaw data separately from the roll 
    // This way lookDirection gets synced correctly to other players in multiplayer 

    [HarmonyPatch(typeof(FPSCamera), "UpdatePosOffset")] // Really, it makes sense in the code
    class InjectHMDRotationPitchYaw
    {

        static void Postfix(FPSCamera __instance)
        {
            if (!VRInitiator.VR_ENABLED)
            {
                return;
            }

            if ((VRInitiator.VR_TRACKING_TYPE.Equals(TrackingType.PositionAndRotation) || VRInitiator.VR_TRACKING_TYPE.Equals(TrackingType.Rotation)))
            {
                Vector3 euler = VRInitiator.GetVRCameraEulerRotation();
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
            if(!VRInitiator.VR_ENABLED)
            {
                return;
            }
            Vector3 Euler = __instance.m_camera.transform.parent.localEulerAngles;
            Euler.z = VRInitiator.GetVRCameraEulerRotation().z;
            __instance.m_camera.transform.parent.localEulerAngles = Euler;
        }
    }
}
