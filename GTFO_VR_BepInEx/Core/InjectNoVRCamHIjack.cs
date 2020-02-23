using System;
using System.Collections.Generic;
using System.Text;
using CellMenu;
using Globals;
using GTFO_VR;
using HarmonyLib;
using Player;
using UnityEngine;


namespace GTFO_VR_BepInEx.Core
{
    /// <summary>
    /// Prevents the game from overriding camera position
    /// </summary>
    [HarmonyPatch(typeof(FPSCamera), "LateUpdate")]
    class InjectVRFreedom
    {
        static void Prefix(FPSCamera __instance)
        {
            __instance.MouseLookEnabled = true;
            __instance.PlayerMoveEnabled = true;
        }
    }

    /// <summary>
    /// Make all transitions end instantly
    /// </summary>
    /// 

    [HarmonyPatch(typeof(FPSCamera), nameof(FPSCamera.StartTransition), new[] { typeof(Vector3), typeof(Quaternion), typeof(float), typeof(DelEasingFunction), typeof(DelEasingFunction), typeof(Action)})]
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
