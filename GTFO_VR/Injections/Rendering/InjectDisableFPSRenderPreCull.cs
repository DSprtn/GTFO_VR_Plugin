using HarmonyLib;
using UnityEngine;

namespace GTFO_VR.Injections.Rendering
{
    /// <summary>
    /// Remove command calls from FPSCamera and move them to PlayerVR instead to use them after poses have been updated for better reprojection and a smoother experience
    /// </summary>
	///

    [HarmonyPatch(typeof(FPS_Render), nameof(FPS_Render.OnPreCull))]
    internal class InjectDisableFPSRenderPreCull
    {
        private static bool Prefix(FPS_Render __instance)
        {
            return false;
        }
    }
}