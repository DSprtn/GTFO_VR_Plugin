using HarmonyLib;
using UnityEngine;
using Gear;


namespace GTFO_VR.Injections.Gameplay
{
    /// <summary>
    /// Adjust the muzzle align to be where it should be on certain broken weapons
    /// </summary>
    [HarmonyPatch(typeof(BulletWeapon), nameof(BulletWeapon.SetupArchetype))]
    internal class InjectMuzzleAlignCorrection
    {
        private static void Postfix(BulletWeapon __instance)
        {
            // Accrat ND6 Heavy SMG
            if ("ACCRAT ND6".Equals(__instance.PublicName.ToUpper()) || "DREKKER CLR".Equals(__instance.PublicName.ToUpper()))
            {
                Transform muzzleAlign = __instance.MuzzleAlign;
                Transform animationRef = muzzleAlign.parent; // This is keyframed with an incorrect rotation, 
                                                             // so we can't just adjust it directly

                // Put muzzle align in a container positioned at origin of parent
                GameObject muzzleContainer = new GameObject("muzzleContainer");
                muzzleContainer.transform.SetParentAtZero(animationRef);
                muzzleAlign.transform.SetParent(muzzleContainer.transform);

                // Apply inverse rotation of parent to align muzzle
                muzzleContainer.transform.localRotation = Quaternion.Inverse(animationRef.transform.localRotation);

                // Move container to parent so it doesn't get moved by animations
                muzzleContainer.transform.SetParent(animationRef.transform.parent);
            }
            
        }
    }
}