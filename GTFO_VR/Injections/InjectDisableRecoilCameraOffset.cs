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
    class InjectDisableRecoilOnCameraApply
    {
        static void Prefix(FPSCamera __instance)
        {
            __instance.m_recoilSystem.m_hasOverrideParentRotation = false;
        }
    }
    [HarmonyPatch(typeof(FPS_RecoilSystem), "FPS_Update")]
    class InjectDisableRecoilOnCamera
    {
        static void Postfix(FPS_RecoilSystem __instance)
        {
            __instance.transform.localEulerAngles = Vector3.zero;
            Shader.SetGlobalVector(__instance.m_FPS_SightOffset_PropertyID, Vector4.zero);
        }
    }
}
