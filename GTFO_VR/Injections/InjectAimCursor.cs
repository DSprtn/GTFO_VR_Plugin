using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using CellMenu;
using Globals;
using GTFO_VR.Core;
using HarmonyLib;
using Player;
using UnityEngine;


namespace GTFO_VR_BepInEx.Core
{
    /// <summary>
    /// Unlocks cursor so steamVR desktop can be used freely
    /// </summary>

    [HarmonyPatch(typeof(CM_PageBase),"UpdateCursorPosition")]
    class InjectAimAtOverlay
    {
        static void Prefix()
        {
            Vector2 newCursorPos = Vector2.zero;
            if(VR_Global.GetPlayerPointingAtPositionOnScreen(out newCursorPos))
            {
                Vector2 res = new Vector2(GuiManager.ScreenRes.width, GuiManager.ScreenRes.height);
                newCursorPos -= new Vector2(0.5f, 0.5f);
                newCursorPos.y *= -1f;
                newCursorPos *= res;
                CM_PageBase.m_cursorPos = newCursorPos;
            }
        }
    }

}
