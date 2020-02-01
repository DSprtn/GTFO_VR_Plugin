using System;
using System.Collections.Generic;
using System.Text;
using GTFO_VR;
using HarmonyLib;
using Player;
using UnityEngine;

namespace GTFO_VR_BepInEx.Core
{
    /// <summary>
    /// Handles input for all VR peripherals supported by SteamVR
    /// Input is handled within SteamVR, in SteamVR_Input, to modify bindings and add new input actions have a look at my 
    /// SteamVR_Standalone repo to start with. Once you create new action bindings you can link them to GTFO actions in VRInput.
    /// </summary>
    /// 

    [HarmonyPatch(typeof(InputMapper), "DoGetAxis")]
    class InjectInputAxis
    {

        static void Postfix(InputAction action, ref float __result)
        {
            if (VRInitiator.VR_ENABLED)
            {
                __result += VRInput.GetAxis(action);
            }
        }
    }

    [HarmonyPatch(typeof(InputMapper), "DoGetButtonUp")]
    class InjectInputBooleanUp
    {
        static void Postfix(InputAction action, ref bool __result)
        {
            if (!__result)
            {
                __result = VRInput.GetActionUp(action);
            }
        }
    }

    [HarmonyPatch(typeof(InputMapper), "DoGetButtonDown")]
    class InjectInputBooleanDown
    {
        static void Postfix(InputAction action, ref bool __result)
        {
            if (!__result)
            {
                __result = VRInput.GetActionDown(action);
            }
        }
    }

    [HarmonyPatch(typeof(InputMapper), "DoGetButton")]
    class InjectInputBoolean
    {
        static void Postfix(InputAction action, ref bool __result)
        {
            if (!__result)
            {
                __result = VRInput.GetAction(action);
            }
        }
    }
}

