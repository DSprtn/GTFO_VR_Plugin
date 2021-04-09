using System;
using HarmonyLib;
using UnityEngine;


namespace GTFO_VR_BepInEx.Core
{

    /// <summary>
    /// Make all camera transitions end instantly, we never want the player's control of the camera to be taken away
    /// </summary>
    /// 
    [HarmonyPatch(typeof(FPSCamera), nameof(FPSCamera.StartTransition), new[] { typeof(Vector3), typeof(Quaternion), typeof(float), typeof(DelEasingFunction), typeof(DelEasingFunction),typeof(Il2CppSystem.Action)})]
    //[HarmonyPatch(typeof(FPSCamera))]
    //[HarmonyPatch("StartTransition", new[] {typeof(Vector3), typeof(Quaternion), typeof(float), typeof(DelEasingFunction), typeof(DelEasingFunction), typeof(Action))})]
    class InjectTransitionKill
    {
        static void Prefix(FPSCamera __instance, Vector3 endPos, Quaternion endRot, ref float duration, DelEasingFunction easingPos, DelEasingFunction easingRot, Action onTransitionDone)
        {
            duration = 0f;
        }
    }


}
