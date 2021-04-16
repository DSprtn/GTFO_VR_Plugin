using Enemies;
using HarmonyLib;

namespace GTFO_VR.Injections.UI
{
    [HarmonyPatch(typeof(EnemyAgent), nameof(EnemyAgent.SyncPlaceNavMarkerTag))]
    internal class InjectSmallerEnemyMarkers
    {
        private static void Postfix(EnemyAgent __instance)
        {
            __instance.m_tagMarker.m_initScale *= .7f;
        }
    }

    [HarmonyPatch(typeof(NavMarkerLayer), nameof(NavMarkerLayer.PlacePlayerMarker))]
    internal class InjectSmallerPlayerInfoMarkers
    {
        private static void Postfix(ref NavMarker __result)
        {
            __result.m_initScale *= 0.6f;
        }
    }
}