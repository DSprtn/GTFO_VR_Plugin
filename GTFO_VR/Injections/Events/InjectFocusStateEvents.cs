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


namespace GTFO_VR.Injections.Events
{
    /// <summary>
    /// Inject event calls for focus state updates
    /// Focus states describe the current 'focus' context of the game --- InTerminal, Elevator, MainMenu etc.
    /// </summary>

    [HarmonyPatch(typeof(InputMapper), nameof(InputMapper.OnFocusStateChanged))]
    class InjectFocusStateEvents
    {
        static void Prefix(eFocusState state)
        {
            FocusStateEvents.FocusChanged(state);
        }
    }


}
