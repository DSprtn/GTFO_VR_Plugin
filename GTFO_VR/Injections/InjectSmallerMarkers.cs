using Enemies;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Text;

namespace GTFO_VR_BepInEx.Core
{
    [HarmonyPatch(typeof(EnemyAgent), "SyncPlaceNavMarkerTag")]
    class InjectSmallerEnemyMarkers
    {

        static void Postfix(EnemyAgent __instance) {
            __instance.m_tagMarker.m_initScale *= .7f;
        }
    }

    [HarmonyPatch(typeof(NavMarkerLayer), "PlacePlayerMarker")]
    class InjectSmallerPlayerInfoMarkers
    {
        static void Postfix(ref NavMarker __result)
        {
            __result.m_initScale *= 0.6f;
        }
    }
}
