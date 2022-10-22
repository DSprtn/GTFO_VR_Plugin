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
            __instance.m_postProcessing.m_bloom.intensity.Override(.275f);
            __instance.m_postProcessing.m_bloom.threshold.Override(1.1f);

        }
    }


}
