using GTFO_VR.Core;
using GTFO_VR.Core.PlayerBehaviours;
using GTFO_VR.Core.VR_Input;
using HarmonyLib;
using Player;
using UnityEngine;

namespace GTFO_VR.Injections.Gameplay
{
    /// <summary>
    /// Makes the first person items follow the position and aim direction of the main controller(s) of the player
    /// </summary>
    [HarmonyPatch(typeof(FirstPersonItemHolder), nameof(FirstPersonItemHolder.Update))]
    internal class InjectControllerAimAlign
    {
        private static void Postfix()
        {
            VRPlayer.UpdateHeldItemTransform();
        }
    }

    /// <summary>
    /// This is probably obsolete, TODO - test
    /// </summary>
    [HarmonyPatch(typeof(PlayerAgent), nameof(PlayerAgent.UpdateInfectionLocal))]
    internal class InjectAimFlashlightFixBegin
    {
        private static void Postfix()
        {
            InjectFPSCameraForwardTweakForInteraction.useVRInteractionForward = true;
            InjectFPSCameraPositionTweakForInteraction.useInteractionControllersPosition = true;
        }
    }

    [HarmonyPatch(typeof(PlayerAgent), nameof(PlayerAgent.UpdateGlobalInput))]
    internal class InjectGlobalInteractionTweakFix
    {
        private static void Prefix()
        {
            InjectFPSCameraForwardTweakForInteraction.useVRInteractionForward = true;
            InjectFPSCameraPositionTweakForInteraction.useInteractionControllersPosition = true;
        }

        private static void Postfix()
        {
            InjectFPSCameraForwardTweakForInteraction.useVRInteractionForward = false;
            InjectFPSCameraPositionTweakForInteraction.useInteractionControllersPosition = false;
        }
    }

    /// <summary>
    /// Changes interactions and throwing to use the VR camera ray.
    /// </summary>

    [HarmonyPatch(typeof(FPSCamera), nameof(FPSCamera.UpdateCameraRay))]
    internal class InjectForwardInteractions
    {
        private static bool Prefix(FPSCamera __instance)
        {
            bool vis = false;
            if (VRConfig.configUseControllers.Value)
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