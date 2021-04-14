using Enemies;
using HarmonyLib;

namespace GTFO_VR.Injections
{
    [HarmonyPatch(typeof(EnemyAgent), nameof(EnemyAgent.SyncPlaceNavMarkerTag))]
    class InjectSmallerEnemyMarkers
    {

        static void Postfix(EnemyAgent __instance)
        {
            __instance.m_tagMarker.m_initScale *= .7f;
        }
    }

    [HarmonyPatch(typeof(NavMarkerLayer), nameof(NavMarkerLayer.PlacePlayerMarker))]
    class InjectSmallerPlayerInfoMarkers
    {
        static void Postfix(ref NavMarker __result)
        {
            __result.m_initScale *= 0.6f;
        }
    }
}
