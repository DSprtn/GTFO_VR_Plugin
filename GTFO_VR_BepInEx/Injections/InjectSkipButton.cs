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
    /// Skip button uses a part of input mapper which doesn't handle gamepad or vr input, so we inject our own bypass
    /// </summary>
    
    [HarmonyPatch(typeof(PUI_SkipText),"UpdateSkipTimer")]
    class InjectMenuSkipInput
    {
        static void Prefix(PUI_SkipText __instance, Action onSkip)
        {
           if(VRInput.GetActionDown(InputAction.Fire))
            {
                onSkip?.Invoke();
            }
        }
    }
}

