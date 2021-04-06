using System;
using System.Collections.Generic;
using System.Text;
using CellMenu;
using GameData;
using Globals;
using GTFO_VR;
using GTFO_VR.UI;
using HarmonyLib;
using LevelGeneration;
using Player;
using UnityEngine;


namespace GTFO_VR_BepInEx.Core
{
    /// <summary>
    /// Replicate new objectives on the VR watch
    /// </summary>

    [HarmonyPatch(typeof(PlayerGuiLayer), "UpdateObjectives")]
    class InjectWatchObjectives
    {
        static void Postfix(LG_LayerType layer,
    string mainObjective,
    WardenObjectiveDataBlock data,
    eWardenSubObjectiveStatus sub,
    bool visible = true,
    bool isAdditionalHelp = false)
        {
            Watch.UpdateMainObjective(mainObjective);
            GTFO_VR_Plugin.log.LogDebug($"Got new objective! - {mainObjective}");
        }
    }

    [HarmonyPatch(typeof(PUI_GameObjectives), "SetSubObjective")]
    class InjectWatchSubObjectives
    {
        static void Postfix(PUI_GameObjectives __instance)
        {
            Watch.UpdateSubObjective(__instance.m_subObjective.text);
            GTFO_VR_Plugin.log.LogDebug($"Got new subobjective! - {__instance.m_subObjective.text}");
        }
    }
}
