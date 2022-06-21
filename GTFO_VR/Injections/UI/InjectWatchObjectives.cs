using GameData;
using GTFO_VR.Core;
using GTFO_VR.UI;
using HarmonyLib;
using LevelGeneration;

namespace GTFO_VR.Injections.UI
{
    // As of R7 this patch will crash the game when it is called, at random. Commented out for now.

    /// <summary>
    /// Replicate new objectives on the VR watch
    /// </summary>
    /*
    [HarmonyPatch(typeof(PlayerGuiLayer), nameof(PlayerGuiLayer.UpdateObjectives))]
    internal class InjectWatchObjectives
    {
        private static void Postfix(LG_LayerType layer,string mainObjective)
        {
            Watch.Current?.UpdateMainObjective(mainObjective);
            Log.Debug($"Got new objective! - {mainObjective}");
        }
    }

    [HarmonyPatch(typeof(PUI_GameObjectives), nameof(PUI_GameObjectives.SetMainSubObjective))]
    internal class InjectWatchSubObjectives
    {
        private static void Postfix(string txt)
        {
            Watch.Current?.UpdateSubObjective(txt);
            Log.Debug($"Got new subobjective! - {txt}");
        }
    }
    */
}