using GTFO_VR.Core;
using GTFO_VR.Core.PlayerBehaviours;
using GTFO_VR.Core.VR_Input;
using HarmonyLib;
using Player;
using UnityEngine;

namespace GTFO_VR_BepInEx.Core
{

    /// <summary>
    /// Makes the first person items follow the position and aim direction of the main controller(s) of the player
    /// </summary>
    [HarmonyPatch(typeof(FirstPersonItemHolder), "Update")]
    class InjectControllerAimAlign
    {
        static void Postfix()
        {
            if (PlayerVR.VRPlayerIsSetup)
            {
                PlayerVR.UpdateHeldItemPosition();
            }
        }
    }



    [HarmonyPatch(typeof(PlayerAgent), nameof(PlayerAgent.UpdateInfectionLocal))]
    class InjectAimFlashlightFixBegin
    {
        static void Postfix()
        {
            InjectFPSCameraForwardTweakForInteraction.useVRInteractionForward = true;
            InjectFPSCameraPositionTweakForInteraction.useInteractionControllersPosition = true;
        }

    }

    [HarmonyPatch(typeof(PlayerAgent), nameof(PlayerAgent.UpdateGoodNodeAndArea))]
    class InjectScreenLiquidFix
    {
        static void Postfix(PlayerAgent __instance)
        {
            ScreenLiquidManager.cameraDir = PlayerVR.VRCamera.transform.forward;
            ScreenLiquidManager.cameraPosition = PlayerVR.VRCamera.transform.position;
        }
    }


    [HarmonyPatch(typeof(PlayerAgent), nameof(PlayerAgent.UpdateGlobalInput))]
    class InjectGlobalInteractionTweakFix
    {
        static void Prefix()
        {
            InjectFPSCameraForwardTweakForInteraction.useVRInteractionForward = true;
            InjectFPSCameraPositionTweakForInteraction.useInteractionControllersPosition = true;
        }

        static void Postfix()
        {
            InjectFPSCameraForwardTweakForInteraction.useVRInteractionForward = false;
            InjectFPSCameraPositionTweakForInteraction.useInteractionControllersPosition = false;
        }

    }


    [HarmonyPatch(typeof(SentryGunFirstPerson), nameof(SentryGunFirstPerson.CheckCanPlace))]
    class InjectSentryGunPlacementFix
    {
        static void Prefix()
        {
            InjectFPSCameraForwardTweakForInteraction.useVRControllerForward = true;
            InjectFPSCameraPositionTweakForInteraction.useControllerPosition = true;
        }

        static void Postfix()
        {
            InjectFPSCameraForwardTweakForInteraction.useVRControllerForward = false;
            InjectFPSCameraPositionTweakForInteraction.useControllerPosition = false;
        }

    }


    [HarmonyPatch(typeof(MineDeployerFirstPerson), nameof(MineDeployerFirstPerson.CheckCanPlace))]
    class InjectMineDeployerPlacementFix
    {
        static void Prefix()
        {
            InjectFPSCameraForwardTweakForInteraction.useVRControllerForward = true;
            InjectFPSCameraPositionTweakForInteraction.useControllerPosition = true;
        }

        static void Postfix()
        {
            InjectFPSCameraForwardTweakForInteraction.useVRControllerForward = false;
            InjectFPSCameraPositionTweakForInteraction.useControllerPosition = false;
        }
    }

    /// <summary>
    /// Changes most actions (placing, firing, throwing) to follow the controller forward instead of camera forward 
    /// NOTE: Does not affect flashlight aggro
    /// </summary>
    /// 

    [HarmonyPatch(typeof(FPSCamera), "UpdateCameraRay")]
    class InjectForwardInteractions
    {
        static bool Prefix(FPSCamera __instance)
        {
            bool vis = false;
            if (PlayerVR.VRPlayerIsSetup && VR_Settings.useVRControllers)
            {
               
                //Used for throwing weapons
                __instance.CameraRayDir = HMD.GetVRInteractionLookDir();

                RaycastHit hit;
                if (Physics.Raycast(Controllers.GetAimFromPos(), Controllers.GetAimForward(), out hit, 50f, LayerManager.MASK_CAMERA_RAY))
                {
                    __instance.CameraRayPos = hit.point;
                    __instance.CameraRayCollider = hit.collider;
                    __instance.CameraRayNormal = hit.normal;
                    __instance.CameraRayObject = hit.collider.gameObject;
                    __instance.CameraRayDist = hit.distance;
                    if (FPSCamera.FriendlyTargetVisAllowed && hit.collider.gameObject.layer == LayerManager.LAYER_PLAYER_SYNCED)
                    {
                        vis = true;
                    }
                }
                else
                {
                    __instance.CameraRayPos = Controllers.GetAimFromPos() + Controllers.GetAimForward() * 50f;
                    __instance.CameraRayCollider = null;
                    __instance.CameraRayNormal = -Controllers.GetAimForward();
                    __instance.CameraRayObject = null;
                    __instance.CameraRayDist = 0.0f;
                }
        }
        GuiManager.CrosshairLayer.SetFriendlyTargetVisible(vis);
        return false;
        }
    }

    
}
