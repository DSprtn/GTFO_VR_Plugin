using Gear;
using GTFO_VR.Core;
using GTFO_VR.Core.PlayerBehaviours;
using GTFO_VR.Util;
using HarmonyLib;


namespace GTFO_VR.Injections
{
    /// <summary>
    /// Handle weapon accuracy for double handed aiming/firing 'from the hip'. 
    /// If a weapon is fired from the hip it will have greater spread unless double handed aiming is disabled.
    /// </summary>

    [HarmonyPatch(typeof(BulletWeapon), nameof(BulletWeapon.Fire))]
    class InjectAimSpreadInVR
    {
        static void Prefix(BulletWeapon __instance)
        {
            if (PlayerVR.VRPlayerIsSetup && VR_Settings.useVRControllers)
            {
                if (Utils.IsFiringFromADS())
                {
                    __instance.FPItemHolder.ItemAimTrigger = true;
                }
            }
        }
    }
}
