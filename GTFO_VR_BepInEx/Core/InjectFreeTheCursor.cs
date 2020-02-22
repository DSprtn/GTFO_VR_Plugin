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
    /// Unlocks cursor so steamVR desktop can be used freely
    /// </summary>

    [HarmonyPatch(typeof(InputMapper),"OnFocusStateChanged")]
    class InjectFreeTheCursor
    {
        static void Postfix()
        {
            Cursor.lockState = CursorLockMode.None;
        }
    }

    [HarmonyPatch(typeof(InputMapper), "CheckLockMouse")]
    class InjectFreeTheCursorMouseCheck
    {
        static void Postfix()
        {
            Cursor.lockState = CursorLockMode.None;
        }
    }

    [HarmonyPatch(typeof(CM_PageBase), "Update")]
    class InjectFreeTheCursorMenu
    {
        static void Postfix()
        {
            Cursor.lockState = CursorLockMode.None;
        }
    }


}
