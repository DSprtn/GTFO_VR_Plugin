using System;
using System.Collections.Generic;
using System.Text;
using GTFO_VR;
using HarmonyLib;
using Player;
using UnityEngine;


namespace GTFO_VR_BepInEx.Core
{
    /// <summary>
    /// Entry point for loading and initiating all things VR
    /// </summary>

    [HarmonyPatch(typeof(PlayerGuiLayer),"Setup")]
    class InjectPlayerGUI
    {
        static void Postfix(PlayerGuiLayer __instance, PUI_Inventory ___Inventory, PUI_LocalPlayerStatus ___m_playerStatus, PUI_WardenIntel ___m_wardenIntel, PUI_GameEventLog ___m_gameEventLog, PUI_Compass ___m_compass, PUI_GameObjectives ___m_wardenObjective)
        {
            ___Inventory.SetPosition(new Vector2(-450f, -200f));
            ___Inventory.transform.localScale *= .85f;
            ___m_compass.SetPosition(new Vector2(0.0f, -150f));
            ___m_compass.transform.localScale *= .8f;
            ___m_gameEventLog.SetPosition(new Vector2(150f, 50f));

            ___m_wardenIntel.SetPosition(new Vector2(400f, -100f));
            ___m_wardenIntel.SetSize(new Vector2(325, 350f));
            ___m_wardenIntel.transform.localScale *= .85f;
            ___m_wardenObjective.SetPosition(new Vector2(400f, -300f));
            ___m_wardenObjective.transform.localScale *= .7f;
            ___m_playerStatus.SetPosition(new Vector2(0.0f, 80f));
        }
    }
}
