using GTFO_VR.Core.VR_Input;
using HarmonyLib;

namespace GTFO_VR.Injections.Input
{
    /// <summary>
    /// Handles input for all VR peripherals supported by SteamVR
    /// Input is handled within SteamVR, in SteamVR_Input, to modify bindings and add new input actions have a look at my
    /// SteamVR_Standalone repo to start with. Once you create new action bindings you can link them to GTFO actions in VRInput.
    /// </summary>
    ///

    [HarmonyPatch(typeof(InputMapper), nameof(InputMapper.DoGetAxis))]
    internal class InjectInputAxis
    {
        private static void Postfix(InputAction action, ref float __result)
        {
            __result += SteamVR_InputHandler.GetAxis(action);
        }
    }

    [HarmonyPatch(typeof(InputMapper), nameof(InputMapper.DoGetButtonUp))]
    internal class InjectInputBooleanUp
    {
        private static void Postfix(InputAction action, ref bool __result)
        {
            __result = __result || SteamVR_InputHandler.GetActionUp(action);
        }
    }

    [HarmonyPatch(typeof(InputMapper), nameof(InputMapper.DoGetButtonDown))]
    internal class InjectInputBooleanDown
    {
        private static void Postfix(InputAction action, ref bool __result)
        {
            __result = __result || SteamVR_InputHandler.GetActionDown(action);
        }
    }

    [HarmonyPatch(typeof(InputMapper), nameof(InputMapper.DoGetButton))]
    internal class InjectInputBoolean
    {
        private static void Postfix(InputAction action, ref bool __result)
        {
            __result = __result || SteamVR_InputHandler.GetAction(action);
        }
    }
}