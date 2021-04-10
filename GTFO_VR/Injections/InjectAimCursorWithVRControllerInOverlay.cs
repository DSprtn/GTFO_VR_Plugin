using CellMenu;
using GTFO_VR.Core.UI;
using HarmonyLib;
using UnityEngine;


namespace GTFO_VR.Injections
{
    /// <summary>
    /// Handles updating the cursor position based on the player's VR controllers within the overlay.
    /// </summary>

    [HarmonyPatch(typeof(CM_PageBase), nameof(CM_PageBase.UpdateCursorPosition))]
    class InjectAimCursorWithVRControllerInOverlay
    {
        static void Prefix()
        {
            Vector2 newCursorPos = Vector2.zero;
            if (VR_UI_Overlay.GetPlayerPointingAtPositionOnScreen(out newCursorPos))
            {
                Vector2 res = new Vector2(GuiManager.ScreenRes.width, GuiManager.ScreenRes.height);
                newCursorPos -= new Vector2(0.5f, 0.5f);
                newCursorPos.y *= -1f;
                newCursorPos *= res;
                CM_PageBase.m_cursorPos = newCursorPos;
            }
        }
    }

}
