using GameData;
using GTFO_VR.Core;
using GTFO_VR.UI;
using HarmonyLib;
using LevelGeneration;
using Localization;

namespace GTFO_VR.Injections.UI
{
    /// <summary>
    /// Replicate new objectives on the VR watch. 
    /// </summary>
    
    [HarmonyPatch(typeof(PlayerGuiLayer), nameof(PlayerGuiLayer.UpdateObjectives))]
    internal class InjectWatchObjectives
    {
        public static PUI_GameObjectives PlayerGuiLayer_PUI_GameObjectives_Instance = null;

        private static void Prefix(PlayerGuiLayer __instance)
        {
            // Keep track of the instance so we can skip the PUI_GameObjectives.SetProgressionObjective() patch
            // when called on any other instance. Unfortunately patching that method is the only way.
            PlayerGuiLayer_PUI_GameObjectives_Instance = __instance.WardenObjectives;
        }

        private static void Postfix(PlayerGuiLayer __instance)
        {
            Watch.Current?.UpdateObjective(__instance.m_wardenObjective);
            Log.Debug($"Got new objectives!");
        }
    }
    
    [HarmonyPatch(typeof(PUI_GameObjectives), nameof(PUI_GameObjectives.SetProgressionObjective))]
    internal class InjectWatchObjectivesProgression
    {
        private static void Postfix(PUI_GameObjectives __instance)
        {
            if ( __instance == InjectWatchObjectives.PlayerGuiLayer_PUI_GameObjectives_Instance)
            {
                Watch.Current?.UpdateObjective(__instance);
                Log.Debug($"Got new progression!");
            }
        }
    }

    [HarmonyPatch(typeof(PUI_GameObjectives), nameof(PUI_GameObjectives.RemoveProgressionObjective))]
    internal class InjectWatchObjectivesProgressionRemove
    {
        private static void Postfix(PUI_GameObjectives __instance)
        {
            if (__instance == InjectWatchObjectives.PlayerGuiLayer_PUI_GameObjectives_Instance)
            {
                Watch.Current?.UpdateObjective(__instance);
                Log.Debug($"Got progression removal!");
            }
        }
    }
}