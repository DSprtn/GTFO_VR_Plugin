using GTFO_VR.UI;
using HarmonyLib;
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
        static void Postfix(PlayerGuiLayer __instance)
        {
            VRWorldSpaceUI.SetPlayerGUIRef(__instance, __instance.m_compass, __instance.m_wardenIntel);

            // ___Inventory.SetPosition(new Vector2(-500f, -250f));
            __instance.Inventory.transform.localScale *= .0f;
            __instance.Inventory.enabled = false;

            __instance.m_compass.SetPosition(new Vector2(0.0f, -250f));

            __instance.m_compass.transform.localScale *= .75f;


            __instance.m_compass.enabled = false;
            __instance.m_compass.transform.localScale *= .0f;



            __instance.m_gameEventLog.SetPosition(new Vector2(150f, 100f));
            __instance.m_gameEventLog.transform.localScale *= .0f;

            // ___m_wardenIntel.SetPosition(new Vector2(0, 0f));
            //___m_wardenIntel.transform.localScale *= .5f;
            //___m_wardenIntel.SetAnchor(GuiAnchor.MidCenter);

            //___m_wardenObjective.SetPosition(new Vector2(625f, -475f));
            __instance.m_wardenObjective.transform.localScale *= .0f;
            __instance.m_wardenObjective.enabled = false;

            //___m_playerStatus.SetPosition(new Vector2(0.0f, 180f));
            __instance.m_playerStatus.transform.localScale *= 0f;
            __instance.m_playerStatus.enabled = false;
        }
    }
}
