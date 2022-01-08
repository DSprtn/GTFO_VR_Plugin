using GTFO_VR.Core;
using GTFO_VR.Core.PlayerBehaviours;
using GTFO_VR.Core.UI;
using GTFO_VR.Events;
using HarmonyLib;
using UnityEngine;
using UnityEngine.PostProcessing;

namespace GTFO_VR.Injections.Rendering
{

    [HarmonyPatch(typeof(FPSCamera), nameof(FPSCamera.RefreshPostEffectsEnabled))]
    internal class InjectPostProcessTweaksInit
    {
        private static void Postfix(FPSCamera __instance)
        {
            //__instance.m_postProcessing.m_autoExposure.active = VRConfig.configPostEyeAdaptation.Value;
            __instance.m_postProcessing.m_vignette.active = VRConfig.configPostVignette.Value || VRConfig.configUseVignetteWhenMoving.Value;
            __instance.m_postProcessing.m_motionBlur.active = false;


            var bloomSettings = __instance.m_postProcessing.m_bloomModel.settings.bloom;
            bloomSettings.intensity *= .25f;
            bloomSettings.radius *= .5f;
            bloomSettings.threshold *= 1.2f;
            __instance.m_postProcessing.m_bloomModel.settings.bloom = bloomSettings;
        }
    }


}
