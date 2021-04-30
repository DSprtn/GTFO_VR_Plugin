using Gear;
using GTFO_VR.Events;
using HarmonyLib;
using UnityEngine;

namespace GTFO_VR.Injections.Events
{
    /// <summary>
    /// Add event calls for the player taking damage
    /// </summary>

    [HarmonyPatch(typeof(CrosshairGuiLayer), nameof(CrosshairGuiLayer.SetChargeUpVisibleAndProgress))]
    internal class InjectHammerChargeEvents
    {
        private static void Postfix(bool visible, float progress)
        {
            if(visible)
            {
                HammerEvents.HammerCharging(progress);
            }
        }
    }
}