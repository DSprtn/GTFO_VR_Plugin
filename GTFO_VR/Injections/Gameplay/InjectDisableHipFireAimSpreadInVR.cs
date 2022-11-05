using Gear;
using GTFO_VR.Core;
using GTFO_VR.Core.VR_Input;
using HarmonyLib;

namespace GTFO_VR.Injections.Gameplay
{
    /// <summary>
    /// Handle weapon accuracy for double handed aiming/firing 'from the hip'.
    /// If a weapon is fired from the hip it will have greater spread unless double handed aiming is disabled.
    /// </summary>

    [HarmonyPatch(typeof(BulletWeapon), nameof(BulletWeapon.Fire))]
    internal class InjectAimSpreadInVR
    {
        private static void Prefix(BulletWeapon __instance)
        {
            if (Controllers.IsFiringFromADS())
            {
                __instance.FPItemHolder.ItemAimTrigger = true;
            }
        }
    }
}