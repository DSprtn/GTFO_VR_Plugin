using GTFO_VR.Core;
using HarmonyLib;
using System;

namespace GTFO_VR.Injections.Rendering
{
    /// <summary>
    /// Disables UI rendering while not in the map or main menu
    /// </summary>
    [HarmonyPatch(typeof(UI_Core), nameof(UI_Core.RenderUI), new Type[0])]
    internal class InjectDisableUIRendering
    {
        private static bool Prefix()
        {
            if (!FocusStateManager.CurrentState.Equals(eFocusState.Map) && !FocusStateManager.CurrentState.Equals(eFocusState.MainMenu) && !FocusStateManager.Current.Equals(eFocusState.GlobalPopupMessage))
            {
                return false;
            }
            return true;
        }
    }
}