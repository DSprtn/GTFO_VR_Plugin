using HarmonyLib;
using UnityEngine;

namespace GTFO_VR.Injections.Rendering
{
    /// <summary>
    /// Remove command calls from FPSCamera and move them to PlayerVR instead to use them after poses have been updated for better reprojection and a smoother experience
    /// </summary>
	///

    [HarmonyPatch(typeof(FPSCamera), nameof(FPSCamera.OnPreCull))]
    internal class InjectDisableFPSCameraRenderUpdate
    {
        private static bool Prefix(FPSCamera __instance)
        {
            return false;
        }
    }    
}