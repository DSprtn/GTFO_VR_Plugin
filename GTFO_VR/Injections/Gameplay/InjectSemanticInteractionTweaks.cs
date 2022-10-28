using Gear;
using GTFO_VR.Core;
using GTFO_VR.Core.VR_Input;
using HarmonyLib;
using Player;
using UnityEngine;

namespace GTFO_VR.Injections.Gameplay
{
    /// <summary>
    /// Tweak interactions for picking up items, giving packs, hacking, placing mines, sentries, etc. 
    /// by using either the camera or controller position for interaction depending on the context.
    /// </summary>

    [HarmonyPatch(typeof(Interact_Timed), nameof(Interact_Timed.EvaluateTimedInteraction))]
    internal class InjectInteractionFromPos
    {
        private static void Prefix(Interact_Timed __instance)
        {
            __instance.m_triggerStartAgentWorldPos = HMD.GetVRInteractionFromPosition();
        }
    }

    [HarmonyPatch(typeof(GuiManager), nameof(GuiManager.IsOnScreen))]
    internal class InjectDisableOnScreenCheck
    {
        private static void Postfix(ref bool __result)
        {
            __result = true;
        }
    }

    [HarmonyPatch(typeof(PlayerInteraction), nameof(PlayerInteraction.UpdateWorldInteractions))]
    internal class InjectWorldInteractionsTweak
    {
        private static Vector3 cachedCamPos;

        private static void Prefix(PlayerInteraction __instance)
        {
            if (!VRConfig.configUseControllers.Value)
            {
                return;
            }
            cachedCamPos = __instance.m_owner.m_camPos;
            __instance.m_owner.m_camPos = HMD.GetVRInteractionFromPosition();
        }

        private static void Postfix(PlayerInteraction __instance)
        {
            if (!VRConfig.configUseControllers.Value)
            {
                return;
            }
            __instance.m_owner.m_camPos = cachedCamPos;
        }
    }

    [HarmonyPatch(typeof(SentryGunFirstPerson), nameof(SentryGunFirstPerson.CheckCanPlace))]
    internal class InjectSentryGunPlacementFix
    {
        private static void Prefix()
        {
            InjectFPSCameraForwardTweakForInteraction.useVRControllerForward = true;
            InjectFPSCameraPositionTweakForInteraction.useControllerPosition = true;
        }

        private static void Postfix()
        {
            InjectFPSCameraForwardTweakForInteraction.useVRControllerForward = false;
            InjectFPSCameraPositionTweakForInteraction.useControllerPosition = false;
        }
    }

    [HarmonyPatch(typeof(MineDeployerFirstPerson), nameof(MineDeployerFirstPerson.CheckCanPlace))]
    internal class InjectMineDeployerPlacementFix
    {
        private static void Prefix()
        {
            InjectFPSCameraForwardTweakForInteraction.useVRControllerForward = true;
            InjectFPSCameraPositionTweakForInteraction.useControllerPosition = true;
        }

        private static void Postfix()
        {
            InjectFPSCameraForwardTweakForInteraction.useVRControllerForward = false;
            InjectFPSCameraPositionTweakForInteraction.useControllerPosition = false;
        }
    }

    [HarmonyPatch(typeof(PlayerLocomotion), nameof(PlayerLocomotion.FixedUpdate))]
    internal class InjectFlashlightSyncAimTweak
    {
        private static void Prefix(PlayerLocomotion __instance)
        {
            if(!__instance.LocallyOwned)
            {
                return;
            }
            InjectFPSCameraForwardTweakForInteraction.useVRInteractionForward = true;
            InjectFPSCameraPositionTweakForInteraction.useInteractionControllersPosition = true;
        }

        private static void Postfix(PlayerLocomotion __instance)
        {
            if (!__instance.LocallyOwned)
            {
                return;
            }
            InjectFPSCameraForwardTweakForInteraction.useVRInteractionForward = false;
            InjectFPSCameraPositionTweakForInteraction.useInteractionControllersPosition = false;
        }
    }

    [HarmonyPatch(typeof(ResourcePackFirstPerson), nameof(ResourcePackFirstPerson.UpdateInteraction))]
    internal class InjectResourcePackInteractionTweak
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

    [HarmonyPatch(typeof(LockMelterFirstPerson), nameof(LockMelterFirstPerson.UpdateApplyActionInput))]
    internal class InjectLockMelterInteractionTweak
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

    [HarmonyPatch(typeof(CarryItemEquippableFirstPerson), nameof(CarryItemEquippableFirstPerson.UpdateInsertOrDropItem))]
    internal class InjectCarryItemInteractionTweak
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

    [HarmonyPatch(typeof(PUI_CommunicationMenu), nameof(PUI_CommunicationMenu.UpdateCmdTripMine))]
    internal class InjectCommsMinerPlacementTweak
    {
        private static void Prefix()
        {
            InjectFPSCameraForwardTweakForInteraction.useVRControllerForward = true;
            InjectFPSCameraPositionTweakForInteraction.useControllerPosition = true;
        }

        private static void Postfix()
        {
            InjectFPSCameraForwardTweakForInteraction.useVRControllerForward = false;
            InjectFPSCameraPositionTweakForInteraction.useControllerPosition = false;
        }
    }

    [HarmonyPatch(typeof(PUI_CommunicationMenu), nameof(PUI_CommunicationMenu.UpdateCmdSentryGun))]
    internal class InjectCommsSentryPlacementTweak
    {
        private static void Prefix()
        {
            InjectFPSCameraForwardTweakForInteraction.useVRControllerForward = true;
            InjectFPSCameraPositionTweakForInteraction.useControllerPosition = true;
        }

        private static void Postfix()
        {
            InjectFPSCameraForwardTweakForInteraction.useVRControllerForward = false;
            InjectFPSCameraPositionTweakForInteraction.useControllerPosition = false;
        }
    }


    [HarmonyPatch(typeof(PUI_CommunicationMenu), nameof(PUI_CommunicationMenu.UpdateButtonsForNode))]
    internal class InjectCommsFinalPlacementTweak
    {
        private static void Prefix()
        {
            InjectFPSCameraForwardTweakForInteraction.useVRControllerForward = true;
            InjectFPSCameraPositionTweakForInteraction.useControllerPosition = true;
        }

        private static void Postfix()
        {
            InjectFPSCameraForwardTweakForInteraction.useVRControllerForward = false;
            InjectFPSCameraPositionTweakForInteraction.useControllerPosition = false;
        }
    }

    [HarmonyPatch(typeof(FPSCamera), nameof(FPSCamera.Forward))]
    [HarmonyPatch(MethodType.Getter)]
    internal class InjectFPSCameraForwardTweakForInteraction
    {
        public static bool useVRInteractionForward = false;
        public static bool useVRControllerForward = false;

        private static void Postfix(FPSCamera __instance, ref Vector3 __result)
        {
            if(!VRConfig.configUseControllers.Value)
            {
                return;
            }

            if (useVRInteractionForward)
            {
                __result = HMD.GetVRInteractionLookDir();
            }

            if (useVRControllerForward)
            {
                __result = Controllers.GetAimForward();
            }
        }
    }

    [HarmonyPatch(typeof(FPSCamera), nameof(FPSCamera.Position))]
    [HarmonyPatch(MethodType.Getter)]
    internal class InjectFPSCameraPositionTweakForInteraction
    {
        public static bool useInteractionControllersPosition = false;
        public static bool useControllerPosition = false;

        private static void Postfix(FPSCamera __instance, ref Vector3 __result)
        {
            if (!VRConfig.configUseControllers.Value)
            {
                return;
            }
            if (useInteractionControllersPosition)
            {
                __result = HMD.GetVRInteractionFromPosition();
            }
            if (useControllerPosition)
            {
                __result = Controllers.GetAimForward();
            }
        }
    }
}