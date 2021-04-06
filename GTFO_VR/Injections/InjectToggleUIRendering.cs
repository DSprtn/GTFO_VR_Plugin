using GTFO_VR.Core;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Text;

namespace GTFO_VR_BepInEx.Core
{
    [HarmonyPatch(typeof(UI_Core), nameof(UI_Core.RenderUI), new Type[0])]
    class InjectToggleUIRendering
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
