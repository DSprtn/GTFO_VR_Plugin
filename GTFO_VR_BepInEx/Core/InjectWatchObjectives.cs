﻿using System;
using System.Collections.Generic;
using System.Text;
using CellMenu;
using Globals;
using GTFO_VR;
using GTFO_VR.UI;
using HarmonyLib;
using Player;
using UnityEngine;


namespace GTFO_VR_BepInEx.Core
{
    /// <summary>
    /// Replicate new objectives on the VR watch
    /// </summary>

    [HarmonyPatch(typeof(PlayerGuiLayer),"UpdateObjectives")]
    class InjectWatchObjectives
    {
        static void Postfix(string mainObjective, string subObjective)
        {
            Watch.UpdateObjectives(mainObjective, subObjective);
        }
    }
}
