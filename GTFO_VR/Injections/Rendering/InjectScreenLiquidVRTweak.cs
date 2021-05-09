using HarmonyLib;
using Player;

namespace GTFO_VR.Injections.Rendering
{
    
    /// <summary>
    /// Patches the screen liquid system to use the VR camera's properties
    /// </summary>
    [HarmonyPatch(typeof(PlayerAgent), nameof(PlayerAgent.UpdateGoodNodeAndArea))]
    internal class InjectScreenLiquidVRTweak
    {
        private static void Postfix(PlayerAgent __instance)
        {
            ScreenLiquidManager.cameraDir = __instance.FPSCamera.transform.forward;
            ScreenLiquidManager.cameraPosition = __instance.FPSCamera.transform.position;
        }
    }
    
}