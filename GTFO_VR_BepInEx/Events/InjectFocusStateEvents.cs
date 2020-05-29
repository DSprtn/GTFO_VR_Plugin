using System;
using System.Collections.Generic;
using System.Text;
using CellMenu;
using Globals;
using GTFO_VR;
using GTFO_VR.Events;
using HarmonyLib;
using Player;
using UnityEngine;


namespace GTFO_VR_BepInEx.Core
{
    /// <summary>
    /// Add event calls for focus state
    /// </summary>

    [HarmonyPatch(typeof(InputMapper),"OnFocusStateChanged")]
    class InjectFocusStateEvents
    {
        static void Prefix(eFocusState state)
        {
            FocusStateEvents.FocusChanged(state);   
        }
    }

   
}
