using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using GTFO_VR;
using HarmonyLib;
using LevelGeneration;
using UnityEngine;
using Valve.VR;

namespace GTFO_VR_BepInEx.Core
{
    /// <summary>
    /// Remove command calls from FPSCamera and move them to PlayerVR instead to use them after poses have been updated for better reprojection and a smoother experience
    /// </summary>
	/// 

    [HarmonyPatch(typeof(FPSCamera), nameof(FPSCamera.LateUpdate))]
    class InjectDisableFPSCameraRendering
    {
        static bool Prefix(FPSCamera __instance)
        {
            
            if (__instance.m_owner == null)
            {
                return false;
            }
            if (__instance.m_cameraTransition.IsActive())
            {
                if (!__instance.m_holder.m_connectedToPlayer)
                {
                    __instance.Position = __instance.m_transCurrPos;
                    __instance.Rotation = __instance.m_transCurrRot;
                    __instance.m_owner.transform.rotation = Quaternion.Euler(0.0f, __instance.Rotation.eulerAngles.y, 0.0f);
                    __instance.ResetYawPitchRotation();
                }
            }
            else
            {
                if (!__instance.m_holder.m_connectedToPlayer && __instance.PlayerMoveEnabled)
                    __instance.Position = __instance.m_owner.PlayerCharacterController.SmoothPosition + __instance.m_posOffset;
                if (__instance.MouseLookEnabled)
                {
                    __instance.RotationUpdate();
                    if (!__instance.m_holder.m_connectedToPlayer)
                        __instance.m_owner.transform.rotation = Quaternion.Euler(0.0f, __instance.Rotation.eulerAngles.y, 0.0f);
                }
            }

            __instance.UpdateLocalPos();
            __instance.UpdateLookatTeammates();
            WeaponShellManager.EjectFPSShells();
            __instance.m_owner.PositionHasBeenUpdated();
            GuiManager.Current.AfterCameraUpdate();
            __instance.m_owner.Interaction.AfterCameraUpdate();

            return false;
        }
    }
}
