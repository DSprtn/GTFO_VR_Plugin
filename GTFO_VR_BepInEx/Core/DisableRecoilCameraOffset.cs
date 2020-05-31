using System;
using System.Collections.Generic;
using System.Text;
using CellMenu;
using GameData;
using Globals;
using GTFO_VR;
using GTFO_VR.UI;
using HarmonyLib;
using Player;
using UnityEngine;
using UnityEngine.Rendering;

namespace GTFO_VR_BepInEx.Core
{
    [HarmonyPatch(typeof(FPSCamera), "RotationUpdate")]
    class DisableRecoilOnCameraApply
    {
        static void Prefix(FPS_RecoilSystem ___m_recoilSystem)
        {
            ___m_recoilSystem.m_hasOverrideParentRotation = false;
        }
    }
    [HarmonyPatch(typeof(FPS_RecoilSystem), "FPS_Update")]
    class DisableRecoilOnCamera
    {
        static void Postfix(FPS_RecoilSystem __instance, int ___m_FPS_SightOffset_PropertyID)
        {
            __instance.transform.localEulerAngles = Vector3.zero;
            Shader.SetGlobalVector(___m_FPS_SightOffset_PropertyID, Vector4.zero);
        }
    }
}
