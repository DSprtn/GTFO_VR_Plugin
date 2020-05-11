using System;
using System.Collections.Generic;
using System.Text;
using CellMenu;
using Globals;
using GTFO_VR;
using GTFO_VR.UI;
using HarmonyLib;
using Player;
using UnityEngine;
using UnityEngine.Rendering;

namespace GTFO_VR_BepInEx.Core
{
    /*
    [HarmonyPatch(typeof(FPS_RecoilSystem),"FPS_Update")]
    [HarmonyPatch(new Type[] {})]
    class DisableRecoilCameraOffset
    {
        static void Postfix(int ___m_FPS_SightOffset_PropertyID)
        {
            Shader.SetGlobalVector(___m_FPS_SightOffset_PropertyID, Vector4.zero);
        }
    }*/
}
