using GTFO_VR.Events;
using HarmonyLib;
using UnityEngine;

namespace GTFO_VR.Injections.Events
{
    /// <summary>
    /// Add event calls for Cfoam launcher for haptics
    /// </summary>

    [HarmonyPatch(typeof(GlueGun), nameof(GlueGun.UpdateLocal))]
    internal class InjectGlueGunPressureEvents
    {
        private static void Postfix(GlueGun __instance)
        {
            if(__instance.Owner.IsLocallyOwned)
            {
                if(__instance.m_pressure > 0.01f)
                {
                    GlueGunEvents.PressureBuilding(__instance.m_pressure);
                }
            }
        }
    }

}