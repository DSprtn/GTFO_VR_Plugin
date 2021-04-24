using GTFO_VR.Core;
using GTFO_VR.Core.UI;
using GTFO_VR.Events;
using HarmonyLib;
using UnityEngine;
using UnityEngine.PostProcessing;

namespace GTFO_VR.Injections.GameHooks
{

    [HarmonyPatch(typeof(PostProcessingBehaviour), nameof(PostProcessingBehaviour.OnEnable))]
    internal class InjectPostProcessEvents
    {
        private static void Postfix(PostProcessingBehaviour __instance)
        {
            PostProcessEvents.PostProcessEventsEnabled(__instance);
        }
    }
}
