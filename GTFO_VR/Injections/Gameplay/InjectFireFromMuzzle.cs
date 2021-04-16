using Gear;
using HarmonyLib;
using UnityEngine;

namespace GTFO_VR.Injections.Gameplay
{
    /// <summary>
    /// Makes weapons fire from the muzzle of the weapon instead of the camera.
    /// </summary>
    [HarmonyPatch(typeof(BulletWeapon), nameof(BulletWeapon.Fire))]
    internal class InjectFireFromWeaponMuzzle
    {
        private static Vector3 cachedPosition = Vector3.zero;

        private static void Prefix(BulletWeapon __instance)
        {
            cachedPosition = __instance.Owner.FPSCamera.Position;
            __instance.Owner.FPSCamera.Position = __instance.MuzzleAlign.position;
        }

        private static void Postfix(BulletWeapon __instance)
        {
            __instance.Owner.FPSCamera.Position = cachedPosition;
        }
    }
}