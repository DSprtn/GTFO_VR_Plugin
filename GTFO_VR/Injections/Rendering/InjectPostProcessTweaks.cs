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
                __instance.m_Bloom.model.enabled = VRConfig.configPostBloom.Value;
                __instance.m_EyeAdaptation.model.enabled = VRConfig.configPostEyeAdaptation.Value;
                __instance.m_Vignette.model.enabled = VRConfig.configPostVignette.Value;
            }
        }
    }
}
