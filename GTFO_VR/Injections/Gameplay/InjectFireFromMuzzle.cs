using Gear;
using HarmonyLib;
using UnityEngine;

namespace GTFO_VR.Injections.Gameplay
{
    /// <summary>
    /// Makes weapons fire from the muzzle of the weapon instead of the camera.
    /// TODO: Remove. Base-game logic already does this now, probably reundant.
    /// </summary>
    [HarmonyPatch(typeof(BulletWeapon), nameof(BulletWeapon.Fire))]
    internal class InjectFireFromWeaponMuzzle
    {
        private static Vector3 cachedPosition = Vector3.zero;
        private static Vector3 cachedLookPosition = Vector3.zero;

        private static void Prefix(BulletWeapon __instance)
        {
            cachedPosition = __instance.Owner.FPSCamera.Position;
            cachedLookPosition = __instance.Owner.FPSCamera.CameraRayPos;
            __instance.Owner.FPSCamera.Position = __instance.MuzzleAlign.position;
            __instance.Owner.FPSCamera.CameraRayPos = __instance.MuzzleAlign.position + __instance.MuzzleAlign.forward * 50f;
        }

        private static void Postfix(BulletWeapon __instance)
        {
            __instance.Owner.FPSCamera.Position = cachedPosition;
            __instance.Owner.FPSCamera.CameraRayPos = cachedLookPosition;
        }
    }

    /// <summary>
    /// Makes shotgun-typw weapons fire from the muzzle of the weapon instead of the camera.
    /// FPSCamera itself has an empty transform, and is moved by its parent transforms.
    /// The transform of FPSCameraRotation sandwiched between the holder and the camera is used for Shotgun pewpew. 
    /// </summary>
    [HarmonyPatch(typeof(Shotgun), nameof(Shotgun.Fire))]
    internal class InjectFireFromShotgunWeaponMuzzle
    {
        private static Vector3 cachedCamRotPosition = Vector3.zero;
        private static Quaternion cachedCamRotRotation = Quaternion.identity;

        private static void Prefix(Shotgun __instance)
        {
            Transform camRotation = __instance.Owner.FPSCamera.transform.parent;

            cachedCamRotPosition = camRotation.transform.position;
            cachedCamRotRotation = camRotation.transform.rotation;
            camRotation.transform.position = __instance.MuzzleAlign.position;
            camRotation.transform.rotation = __instance.MuzzleAlign.rotation;
        }

        private static void Postfix(Shotgun __instance)
        {
            Transform camRotation = __instance.Owner.FPSCamera.transform.parent;

            camRotation.position = cachedCamRotPosition;
            camRotation.rotation = cachedCamRotRotation;
        }
    }
}
