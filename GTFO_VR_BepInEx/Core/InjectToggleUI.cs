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
    /// HUD UI Toggle --- Not needed anymore?
    /// </summary>
    /*
    [HarmonyPatch(typeof(UI_Core),"RenderUI")]
    class InjectToggleUI
    {
        static bool Prefix()
        {
            if(!PlayerVR.UIVisible && !FocusStateManager.CurrentState.Equals(eFocusState.Map) && !FocusStateManager.CurrentState.Equals(eFocusState.MainMenu))
            {
                return false;
            }
            return true;
        }
    }
    */
}
