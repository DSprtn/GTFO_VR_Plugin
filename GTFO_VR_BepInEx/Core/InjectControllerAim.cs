using System;
using System.Collections.Generic;
using System.Text;
using Gear;
using GTFO_VR;
using HarmonyLib;
using Player;
using UnityEngine;


namespace GTFO_VR_BepInEx.Core
{
    /// <summary>
    /// Makes the first person items follow the right controller of the player
    /// </summary>

    [HarmonyPatch(typeof(FirstPersonItemHolder),"LateUpdate")]
    class InjectControllerAim
    {
        static void Prefix(FirstPersonItemHolder __instance, ItemEquippable ___WieldedItem)
        {
            // VR is enabled and controller is on (not at a zero vector)
            if(VRInitiator.VR_ENABLED && VRInitiator.VR_CONTROLLER_PRESENT)
            {
                if(___WieldedItem == null)
                {
                    return;
                }
                ___WieldedItem.transform.position = VRInitiator.GetControllerPosition();
                ___WieldedItem.transform.rotation = VRInitiator.GetControllerRotation();
            }
        }
    }

    /// Inject this twice because otherwise data like weapon muzzle position is not updated correctly (makes tracers spawn in wrong location, for instance)
    /// </summary>

    [HarmonyPatch(typeof(FirstPersonItemHolder), "Update")]
    class InjectControllerAimAlign
    {
        static void Postfix(FirstPersonItemHolder __instance, ItemEquippable ___WieldedItem)
        {
            // VR is enabled and controller is on (not at a zero vector)
            if (VRInitiator.VR_ENABLED && VRInitiator.VR_CONTROLLER_PRESENT)
            {
                if (___WieldedItem == null)
                {
                    return;
                }
                ___WieldedItem.transform.position = VRInitiator.GetControllerPosition();
                ___WieldedItem.transform.rotation = VRInitiator.GetControllerRotation();
            }
        }
    }

    /// <summary>
    /// Changes all interactions (placing, firing, throwing) to follow the controller forward instead of camera forward
    /// </summary>
    /// 

    [HarmonyPatch(typeof(FPSCamera), "UpdateCameraRay")]
    class InjectForwardInteractions
    {
        static void Postfix(FPSCamera __instance)
        {
            if (VRInitiator.VR_ENABLED && VRInitiator.VR_CONTROLLER_PRESENT)
            {
                __instance.CameraRayDir = VRInitiator.GetAimForward();
                RaycastHit hit;
                if (Physics.Raycast(VRInitiator.GetAimFromPos(), VRInitiator.GetAimForward(), out hit, 50f, LayerManager.MASK_CAMERA_RAY))
                {
                    __instance.CameraRayPos = hit.point;
                    __instance.CameraRayCollider = hit.collider;
                    __instance.CameraRayNormal = hit.normal;
                    __instance.CameraRayObject = hit.collider.gameObject;
                    __instance.CameraRayDist = hit.distance;
                }
                else
                {
                    __instance.CameraRayPos = VRInitiator.GetAimFromPos() + VRInitiator.GetAimForward() * 50f;
                    __instance.CameraRayCollider = null;
                    __instance.CameraRayNormal = -VRInitiator.GetAimForward();
                    __instance.CameraRayObject = null;
                    __instance.CameraRayDist = 0.0f;
                }
            }
        }
    }

    
}
