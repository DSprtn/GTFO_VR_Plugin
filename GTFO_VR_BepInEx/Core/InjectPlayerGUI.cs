using System;
using System.Collections.Generic;
using System.Text;
using GTFO_VR;
using GTFO_VR.Core;
using GTFO_VR.UI;
using HarmonyLib;
using Player;
using UnityEngine;


namespace GTFO_VR_BepInEx.Core
{
    /// <summary>
    /// Hacky thing to get the GUI visible to the player inside the HMD --- 
    /// Basically moves all UI elements more towards the center to compensate for lens distortion
    /// </summary>

    [HarmonyPatch(typeof(PlayerGuiLayer),"Setup")]
    class InjectPlayerGUI
    {
        static void Postfix(PlayerGuiLayer __instance, PUI_Inventory ___Inventory, PUI_LocalPlayerStatus ___m_playerStatus, PUI_WardenIntel ___m_wardenIntel, PUI_GameEventLog ___m_gameEventLog, PUI_Compass ___m_compass, PUI_GameObjectives ___m_wardenObjective)
        {
            VRWorldSpaceUI.SetPlayerGUIRef(__instance, ___m_compass, ___m_wardenIntel);

           // ___Inventory.SetPosition(new Vector2(-500f, -250f));
            ___Inventory.transform.localScale *= .0f;
            ___Inventory.enabled = false;

            ___m_compass.SetPosition(new Vector2(0.0f, -250f));

            ___m_compass.transform.localScale *= .75f;

            if(VR_Settings.disableCompass)
            {
                ___m_compass.enabled = false;
                ___m_compass.transform.localScale *= .0f;
            }

           
            ___m_gameEventLog.SetPosition(new Vector2(150f, 100f));
            ___m_gameEventLog.transform.localScale *= .0f;

           // ___m_wardenIntel.SetPosition(new Vector2(0, 0f));
            //___m_wardenIntel.transform.localScale *= .5f;
            //___m_wardenIntel.SetAnchor(GuiAnchor.MidCenter);

            //___m_wardenObjective.SetPosition(new Vector2(625f, -475f));
            ___m_wardenObjective.transform.localScale *= .0f;
            ___m_wardenObjective.enabled = false;

            //___m_playerStatus.SetPosition(new Vector2(0.0f, 180f));
            ___m_playerStatus.transform.localScale *= 0f;
            ___m_playerStatus.enabled = false;
        }
    }
}
