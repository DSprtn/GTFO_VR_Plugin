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
    /// Entry point for loading and initiating all things VR
    /// </summary>

    [HarmonyPatch(typeof(CM_PageBase),"Update")]
    class InjectDebug
    {
        static void Prefix(CM_PageBase __instance)
        {
            if(Input.GetKeyDown(KeyCode.F11))
            {
                DebugHelper.LogScene();
            }
           
        }
    }
}
