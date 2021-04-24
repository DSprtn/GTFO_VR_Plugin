using GTFO_VR.Core;
using GTFO_VR.Core.UI;
using GTFO_VR.Events;
using HarmonyLib;
using UnityEngine;
using UnityEngine.PostProcessing;

namespace GTFO_VR.Injections.Rendering
{

    [HarmonyPatch(typeof(PostProcessingBehaviour), nameof(PostProcessingBehaviour.OnRenderImage))]
    internal class InjectPostProcessTweaks
    {
        private static void Prefix(PostProcessingBehaviour __instance)
        {
            if (__instance.profile != null)
            {
                __instance.m_Bloom.model.enabled = VRSettings.useBloomPostProcess;
                __instance.m_EyeAdaptation.model.enabled = VRSettings.useEyeAdaptionPostProcess;
                __instance.m_Vignette.model.enabled = VRSettings.useVignettePostProcess;
            }
        }
    }
}
