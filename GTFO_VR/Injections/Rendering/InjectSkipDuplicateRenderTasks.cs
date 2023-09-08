using GTFO_VR.Core.PlayerBehaviours;
using HarmonyLib;
using UnityEngine.Rendering.PostProcessing;
using WindVolume;

namespace GTFO_VR.Injections.Rendering
{
    /// <summary>
    /// Only calls UpdateVolume() responsible for fog turbine and repellers punching holes in fog
    /// </summary>
    [HarmonyPatch(typeof(WindVolumeCamera), nameof(WindVolumeCamera.OnPreCull))]
    internal class InjectWindVolumeUpdateSkip
    {
        private static bool Prefix()
        {
            return VRRendering.renderingFirstEye();
        }
    }
}
