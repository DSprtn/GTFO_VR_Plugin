using GTFO_VR.Core;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Text;

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
