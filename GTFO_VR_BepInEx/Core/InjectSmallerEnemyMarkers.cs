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

        static void Postfix(NavMarker ___m_tagMarker)
        {
            ___m_tagMarker.m_initScale *= .7f;
        }
    }
}
