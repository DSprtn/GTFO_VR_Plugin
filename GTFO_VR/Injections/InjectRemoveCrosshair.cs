using HarmonyLib;


namespace GTFO_VR_BepInEx.Core
{
    /// <summary>
    /// Disable crosshair GUI layer
    /// </summary>
    [HarmonyPatch(typeof(GuiManager), "OnFocusStateChanged")]
    class InjectRemoveCrosshairHit
    {
        static void Postfix()
        {
            GuiManager.CrosshairLayer.SetVisible(false);
        }
    }
}