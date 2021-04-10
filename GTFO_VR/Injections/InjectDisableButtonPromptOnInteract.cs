using HarmonyLib;
using System;

namespace GTFO_VR.Injections
{
    /// <summary>
    /// Removes the button text that needs to be pressed on PC to do the given action from the interaction prompt.
    /// </summary>

    [HarmonyPatch(typeof(PUI_InteractionPrompt), nameof(PUI_InteractionPrompt.SetPrompt))]
    [HarmonyPatch(new Type[] { typeof(string), typeof(string) })]
    class InjectDisableButtonPromptOnInteract
    {
        static void Prefix(ref string button)
        {
            button = "";
        }
    }
}
