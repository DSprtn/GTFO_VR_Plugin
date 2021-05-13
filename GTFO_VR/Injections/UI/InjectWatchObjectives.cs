using GameData;
using GTFO_VR.Core;
using GTFO_VR.UI;
using HarmonyLib;
using LevelGeneration;

namespace GTFO_VR.Injections.UI
{
    /// <summary>
    /// Replicate new objectives on the VR watch
    /// </summary>

    [HarmonyPatch(typeof(PlayerGuiLayer), nameof(PlayerGuiLayer.UpdateObjectives))]
    internal class InjectWatchObjectives
    {
        private static void Postfix(LG_LayerType layer,
    string mainObjective,
    WardenObjectiveDataBlock data,
    eWardenSubObjectiveStatus sub,
    bool visible = true,
    bool isAdditionalHelp = false)
        {
            Watch.Current?.UpdateMainObjective(mainObjective);
            Log.Debug($"Got new objective! - {mainObjective}");
        }
    }

    [HarmonyPatch(typeof(PUI_GameObjectives), nameof(PUI_GameObjectives.SetSubObjective))]
    internal class InjectWatchSubObjectives
    {
        /// <summary>
        /// Do not show done objectives
        /// </summary>
        /// <param name="showPrevious"></param>
        private static void Prefix(ref bool showPrevious)
        {
            showPrevious = false;
        }

        private static void Postfix(PUI_GameObjectives __instance)
        {
            Watch.Current?.UpdateSubObjective(__instance.m_subObjective.text);
            Log.Debug($"Got new subobjective! - {__instance.m_subObjective.text}");
        }
    }
}