using BepInEx;
using GTFO_VR.Core.VR_Input;
using HarmonyLib;
using Il2CppSystem;
using Il2CppSystem.Diagnostics;
using System.Diagnostics;

namespace GTFO_VR.Injections.Input
{
    /// <summary>
    /// Skip button uses a part of input mapper which doesn't handle gamepad or vr input, so we inject our own bypass
    /// </summary>

    [HarmonyPatch(typeof(PUI_SkipText), nameof(PUI_SkipText.UpdateSkipTimer))]
    internal class InjectMenuSkipInput
    {
        private static bool Prefix(PUI_SkipText __instance, Action onSkip)
        {
            if (SteamVR_InputHandler.GetActionDown(InputAction.Fire) || !UnityEngine.Input.inputString.IsNullOrWhiteSpace())
            {
                onSkip?.Invoke();
            }

            return false;
        }
    }
}