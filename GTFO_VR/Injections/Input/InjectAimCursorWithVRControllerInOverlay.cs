using CellMenu;
using GTFO_VR.Core.UI;
using HarmonyLib;
using UnityEngine;

namespace GTFO_VR.Injections.Input
{
    /// <summary>
    /// Handles updating the cursor position based on the player's VR controllers within the overlay.
    /// </summary>

    [HarmonyPatch(typeof(CM_PageBase), nameof(CM_PageBase.UpdateCursorPosition))]
    internal class InjectAimCursorWithVRControllerInOverlay
    {
        private static void Prefix(CM_PageBase __instance)
        {
            if (VR_UI_Overlay.Current && VR_UI_Overlay.Current.GetPlayerPointingAtPositionOnScreen(out Vector2 newCursorPos))
            {
                Vector2 res = __instance.m_screenResVec2;
                newCursorPos -= new Vector2(0.5f, 0.5f);
                newCursorPos.y *= -1f;
                newCursorPos *= res;
                CM_PageBase.m_cursorPos = newCursorPos;
            }
        }
    }
}