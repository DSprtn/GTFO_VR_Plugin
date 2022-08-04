using GTFO_VR.Core;
using GTFO_VR.Core.UI;
using GTFO_VR.Core.UI.Terminal;
using HarmonyLib;
using UnityEngine;

namespace GTFO_VR.Injections.Input
{
    /// <summary>
    /// Handles rare occasions of GTFO checking input for a specific keycode.
    /// Used when the terminal plays an audio log and requests Y / N input to play/cancel, and Z to stop.
    /// </summary>
    ///

    [HarmonyPatch(typeof(UnityEngine.Input), nameof(UnityEngine.Input.GetKeyDown), typeof(UnityEngine.KeyCode))]
    internal class InjectKeyDown
    {
        private static void Postfix(KeyCode key, ref bool __result)
        {
            __result = __result || TerminalKeyboardInterface.GetKeycodeDown(key);
        }
    }

    /// <summary>
    /// When checking for modifiers such as ctrl ( ctrl-c to cancel ping repeat ), GetKey() is queried instead
    /// </summary>
    ///
    [HarmonyPatch(typeof(UnityEngine.Input), nameof(UnityEngine.Input.GetKey), typeof(UnityEngine.KeyCode))]
    internal class InjectKey
    {
        private static void Postfix(KeyCode key, ref bool __result)
        {
            __result = __result || TerminalKeyboardInterface.GetKeycodeDown(key);
        }
    }

}