using GTFO_VR.UI;
using HarmonyLib;

namespace GTFO_VR.Injections.GameHooks
{
    /// <summary>
    /// Get references to UI components we want to put into world space
    /// </summary>
    [HarmonyPatch(typeof(PlayerGuiLayer), nameof(PlayerGuiLayer.Setup))]
    internal class InjectPlayerGUIRef
    {
        private static void Postfix(PlayerGuiLayer __instance)
        {
            VRWorldSpaceUI.SetPlayerGUIRef(__instance, __instance.m_compass, __instance.m_wardenIntel, __instance.m_objectiveTimer);
            __instance.m_compass.transform.localScale *= .75f;
        }
    }
}