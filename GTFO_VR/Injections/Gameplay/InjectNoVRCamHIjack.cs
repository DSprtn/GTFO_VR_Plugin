using HarmonyLib;
using System;
using UnityEngine;

namespace GTFO_VR.Injections.Gameplay
{
    /// <summary>
    /// Make all camera transitions end instantly, we never want the player's control of the camera to be taken away
    /// </summary>
    ///
    [HarmonyPatch(typeof(FPSCamera), nameof(FPSCamera.StartTransition), new[] { typeof(Vector3), typeof(Quaternion), typeof(float), typeof(DelEasingFunction), typeof(DelEasingFunction), typeof(Il2CppSystem.Action) })]
    internal class InjectTransitionKill
    {
        private static void Prefix(FPSCamera __instance, Vector3 endPos, Quaternion endRot, ref float duration, DelEasingFunction easingPos, DelEasingFunction easingRot, Action onTransitionDone)
        {
            duration = 0f;
        }
    }
}