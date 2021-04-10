using CellMenu;
using HarmonyLib;
using UnityEngine;


namespace GTFO_VR.Injections
{
    /// <summary>
    /// Unlocks cursor so steamVR desktop can be used freely
    /// </summary>

    [HarmonyPatch(typeof(InputMapper), nameof(InputMapper.OnFocusStateChanged))]
    class InjectFreeTheCursor
    {
        static void Postfix()
        {
            Cursor.lockState = CursorLockMode.None;
        }
    }

    [HarmonyPatch(typeof(InputMapper), nameof(InputMapper.CheckLockMouse))]
    class InjectFreeTheCursorMouseCheck
    {
        static void Postfix()
        {
            Cursor.lockState = CursorLockMode.None;
        }
    }

    [HarmonyPatch(typeof(CM_PageBase), nameof(CM_PageBase.Update))]
    class InjectFreeTheCursorMenu
    {
        static void Postfix()
        {
            Cursor.lockState = CursorLockMode.None;
        }
    }


}
