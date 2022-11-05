using CellMenu;
using GTFO_VR.Core;
using HarmonyLib;
using UnityEngine;

namespace GTFO_VR.Injections.Input
{
    /// <summary>
    /// Unlocks cursor so steamVR desktop can be used freely
    /// </summary>

    [HarmonyPatch(typeof(InputMapper), nameof(InputMapper.OnFocusStateChanged))]
    internal class InjectFreeTheCursor
    {
        private static void Postfix()
        {
            if (!VRConfig.configUseControllers.Value)
            {
                return;
            }
            Cursor.lockState = CursorLockMode.None;
        }
    }

    [HarmonyPatch(typeof(InputMapper), nameof(InputMapper.CheckLockMouse))]
    internal class InjectFreeTheCursorMouseCheck
    {
        private static void Postfix()
        {
            if (!VRConfig.configUseControllers.Value)
            {
                return;
            }
            Cursor.lockState = CursorLockMode.None;
        }
    }

    [HarmonyPatch(typeof(CM_PageBase), nameof(CM_PageBase.Update))]
    internal class InjectFreeTheCursorMenu
    {
        private static void Postfix()
        {
            if (!VRConfig.configUseControllers.Value)
            {
                return;
            }
            Cursor.lockState = CursorLockMode.None;
        }
    }
}