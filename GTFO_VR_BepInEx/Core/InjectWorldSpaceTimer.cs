using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace GTFO_VR_BepInEx.Core
{
    /*
    [HarmonyPatch(typeof(PUI_InteractionPrompt), "SetTimerFill")]
    class InjectWorldSpaceTimer
    {
        static Dictionary<PUI_InteractionPrompt,Transform> dict = new Dictionary<PUI_InteractionPrompt,Transform>();

        static void Prefix(float fill, PUI_InteractionPrompt __instance)
        {
            Transform bg = null;
            if(!dict.TryGetValue(__instance, out bg))
            {
                dict.Add(__instance, __instance.transform.FindChildRecursive("Timer BG"));
            }
            bg = dict[__instance];

            Vector3 minScale = bg.localScale;
            minScale.x = 0;
            Vector3 maxScale = minScale;
            maxScale.x = 1;
            bg.localScale = Vector3.Lerp(minScale, maxScale, fill);
        }
    }

    [HarmonyPatch(typeof(PUI_InteractionPrompt), "SetStyle")]
    class InjectWorldSpaceTimerStyle
    {
        static Dictionary<PUI_InteractionPrompt, SpriteRenderer> dict = new Dictionary<PUI_InteractionPrompt, SpriteRenderer>();
        static void Postfix(string ___m_colorHex, PUI_InteractionPrompt __instance)
        {
            SpriteRenderer bg = null;
            if (!dict.TryGetValue(__instance, out bg))
            {
                dict.Add(__instance, __instance.transform.FindChildRecursive("Timer BG").GetComponent<SpriteRenderer>());
                
            }
            bg = dict[__instance];
            bg.color = ColorExt.Hex(___m_colorHex);
        }
    }
    */
}
