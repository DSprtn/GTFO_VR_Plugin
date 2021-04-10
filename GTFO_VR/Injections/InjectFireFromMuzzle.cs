using Gear;
using HarmonyLib;
using UnityEngine;


namespace GTFO_VR_BepInEx.Core
{

    [HarmonyPatch(typeof(BulletWeapon), nameof(BulletWeapon.Fire))]
    class InjectFireFromWeaponMuzzle
    {
        static Vector3 cachedPosition = Vector3.zero;

        static void Prefix(BulletWeapon __instance)
        {
            cachedPosition = __instance.Owner.FPSCamera.Position;
            __instance.Owner.FPSCamera.Position = __instance.MuzzleAlign.position;
        }
        static void Postfix(BulletWeapon __instance)
        {
            __instance.Owner.FPSCamera.Position = cachedPosition;
        }
    }
}