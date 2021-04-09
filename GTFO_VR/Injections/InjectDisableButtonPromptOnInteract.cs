using HarmonyLib;
using System;

namespace GTFO_VR_BepInEx.Core
{
    [HarmonyPatch(typeof(PUI_InteractionPrompt), "SetPrompt")]
    [HarmonyPatch(new Type[] { typeof(string), typeof(string) })]
    class InjectDisableButtonPromptOnInteract
    {
        static void Prefix(ref string button)
        {
            button = "";
        }
    }
}
