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

    [HarmonyPatch(typeof(FirstPersonItemHolder),"Update")]
    class InjectControllerAim
    {
        static void Postfix(FirstPersonItemHolder __instance, ItemEquippable ___WieldedItem)
        {
            // VR is enabled and controller is on (not at a zero vector)
            if(VRInitiator.VR_ENABLED && VRInitiator.VR_CONTROLLER_PRESENT)
            {
                if(___WieldedItem == null)
                {
                    return;
                }
                ___WieldedItem.transform.position = VRInitiator.GetMainControllerPosition();
                ___WieldedItem.transform.rotation = VRInitiator.GetMainControllerRotation();
            }
        }
    }

    /// <summary>
    /// Changes all interactions (placing, firing, throwing) to follow the controller forward instead of camera forward
    /// </summary>
    /// 
    // TODO Expand to evaluate currently held item to shoot and interact from gun barrel instead of controller position

    [HarmonyPatch(typeof(FPSCamera), "UpdateCameraRay")]
    class InjectForwardInteractions
    {
        static void Postfix(FPSCamera __instance)
        {
            if (VRInitiator.VR_ENABLED && VRInitiator.VR_CONTROLLER_PRESENT)
            {
                RaycastHit hit;
                __instance.CameraRayDir = VRInitiator.GetMainControllerForward();
                if (Physics.Raycast(VRInitiator.GetMainControllerPosition(), VRInitiator.GetMainControllerForward(), out hit, 50f, LayerManager.MASK_CAMERA_RAY))
                {
                    __instance.CameraRayPos = hit.point;
                    __instance.CameraRayCollider = hit.collider;
                    __instance.CameraRayNormal = hit.normal;
                    __instance.CameraRayObject = hit.collider.gameObject;
                    __instance.CameraRayDist = hit.distance;
                } else
                {
                    __instance.CameraRayPos = VRInitiator.GetMainControllerPosition() + VRInitiator.GetMainControllerForward() * 50f;
                    __instance.CameraRayCollider = null;
                    __instance.CameraRayNormal = -VRInitiator.GetMainControllerForward();
                    __instance.CameraRayObject = null;
                    __instance.CameraRayDist = 0.0f;
                }
            }
        }
    }

    /// <summary>
    /// Makes the weapons render normally instead of in the 2D UI camera
    /// </summary>
    // TODO Not working correctly yet
    [HarmonyPatch(typeof(PlayerBackpackManager), "SetFPSRendering")]
    class InjectRenderFirstPersonItemsForVR
    {
        static void Prefix(ref bool enable)
        {
            enable = false;
        }
    }

    /// <summary>
    /// Disables FPS arms rendering, it's really wonky in VR so it's better to not see it at all
    /// </summary>

    [HarmonyPatch(typeof(FirstPersonItemHolder), "SetupFPSRig")]
    class InjectDisableFPSArms
    {
        static void Postfix(FirstPersonItemHolder __instance)
        {
            foreach(Renderer renderer in __instance.FPSArms.GetComponentsInChildren<Renderer>())
            {
                renderer.enabled = false;
            }
        }
    }
}
