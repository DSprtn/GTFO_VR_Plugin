using GTFO_VR.Core;
using HarmonyLib;
using System;

namespace GTFO_VR.Injections
{
    /// <summary>
    /// Disables UI rendering while not in the map or main menu
    /// </summary>
    [HarmonyPatch(typeof(UI_Core), nameof(UI_Core.RenderUI), new Type[0])]
    class InjectDisableUIRendering
    {
        static bool Prefix()
        {
            if (!VR_Settings.Render2DUI && !FocusStateManager.CurrentState.Equals(eFocusState.Map) && !FocusStateManager.CurrentState.Equals(eFocusState.MainMenu))
            {
                return false;
            }
            return true;
        }
    }
}
