using Gear;
using GTFO_VR.Core.VR_Input;
using HarmonyLib;
using UnityEngine;

namespace GTFO_VR_BepInEx.Core
{
    [HarmonyPatch(typeof(Interact_Timed), "EvaluateTimedInteraction")]
    class InjectInteractionFromPos
    {
        static void Prefix(Interact_Timed __instance)
        {
            __instance.m_triggerStartAgentWorldPos = HMD.GetVRInteractionFromPosition();
        }
    }

    [HarmonyPatch(typeof(GuiManager), "IsOnScreen")]
    class InjectDisableOnScreenCheck
    {
        static void Postfix(ref bool __result)
        {
            __result = true;
        }
    }


    [HarmonyPatch(typeof(PlayerInteraction), nameof(PlayerInteraction.UpdateWorldInteractions))]
    class InjectWorldInteractionsTweak
    {
        static Vector3 cachedCamPos;
        static void Prefix(PlayerInteraction __instance)
        {
            cachedCamPos = __instance.m_owner.m_camPos;
            __instance.m_owner.m_camPos = HMD.GetVRInteractionFromPosition();
        }

        static void Postfix(PlayerInteraction __instance)
        {
            __instance.m_owner.m_camPos = cachedCamPos;
        }
    }

    [HarmonyPatch(typeof(ResourcePackFirstPerson), nameof(ResourcePackFirstPerson.UpdateInteraction))]
    class InjectResourcePackInteractionTweak
    {

        static void Prefix(PlayerInteraction __instance)
        {
            InjectFPSCameraForwardTweakForInteraction.useInteractionControllersForward = true;
            InjectFPSCameraPositionTweakForInteraction.useInteractionControllersPosition = true;
        }

        static void Postfix(PlayerInteraction __instance)
        {
            InjectFPSCameraForwardTweakForInteraction.useInteractionControllersForward = false;
            InjectFPSCameraPositionTweakForInteraction.useInteractionControllersPosition = false;
        }
    }

    [HarmonyPatch(typeof(LockMelterFirstPerson), nameof(LockMelterFirstPerson.UpdateApplyActionInput))]
    class InjectLockMelterInteractionTweak
    {

        static void Prefix(PlayerInteraction __instance)
        {
            InjectFPSCameraForwardTweakForInteraction.useInteractionControllersForward = true;
            InjectFPSCameraPositionTweakForInteraction.useInteractionControllersPosition = true;
        }

        static void Postfix(PlayerInteraction __instance)
        {
            InjectFPSCameraForwardTweakForInteraction.useInteractionControllersForward = false;
            InjectFPSCameraPositionTweakForInteraction.useInteractionControllersPosition = false;
        }
    }

    [HarmonyPatch(typeof(CarryItemEquippableFirstPerson), nameof(CarryItemEquippableFirstPerson.UpdateInsertOrDropItem))]
    class InjectCarryItemInteractionTweak
    {

        static void Prefix(PlayerInteraction __instance)
        {
            InjectFPSCameraForwardTweakForInteraction.useInteractionControllersForward = true;
            InjectFPSCameraPositionTweakForInteraction.useInteractionControllersPosition = true;
        }

        static void Postfix(PlayerInteraction __instance)
        {
            InjectFPSCameraForwardTweakForInteraction.useInteractionControllersForward = false;
            InjectFPSCameraPositionTweakForInteraction.useInteractionControllersPosition = false;
        }
    }


    [HarmonyPatch(typeof(FPSCamera), nameof(FPSCamera.Forward))]
    [HarmonyPatch(MethodType.Getter)]
    class InjectFPSCameraForwardTweakForInteraction
    {
        public static bool useInteractionControllersForward = false;

        static void Postfix(PlayerInteraction __instance, ref Vector3 __result)
        {
            if(useInteractionControllersForward)
            {
                __result = HMD.GetVRInteractionLookDir();
            } 
        }
    }


    [HarmonyPatch(typeof(FPSCamera), nameof(FPSCamera.Position))]
    [HarmonyPatch(MethodType.Getter)]
    class InjectFPSCameraPositionTweakForInteraction
    {
        public static bool useInteractionControllersPosition = false;

        static void Postfix(PlayerInteraction __instance, ref Vector3 __result)
        {
            if (useInteractionControllersPosition)
            {
                __result = HMD.GetVRInteractionFromPosition();
            }
        }
    }
}
