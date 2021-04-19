using GTFO_VR.Events;
using HarmonyLib;
using UnityEngine;

namespace GTFO_VR.Injections.Events
{
    /// <summary>
    /// Add event calls for the player shooting weapons
    /// </summary>

    [HarmonyPatch(typeof(Weapon), nameof(Weapon.ApplyRecoil))]
    internal class InjectPlayerWeaponFireEvents
    {
        private static void Postfix(Weapon __instance)
        {
            if(__instance.Owner.IsLocallyOwned)
            {
                PlayerFireWeaponEvents.WeaponFired(__instance);
            }
        }
    }
}