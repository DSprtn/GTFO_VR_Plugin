using Gear;
using GTFO_VR.Events;
using HarmonyLib;
using UnityEngine;

namespace GTFO_VR.Injections.Events
{
    /// <summary>
    /// Add misc. event calls for held items
    /// </summary>

    [HarmonyPatch(typeof(CrosshairGuiLayer), nameof(CrosshairGuiLayer.SetChargeUpVisibleAndProgress))]
    internal class InjectHeldItemEvents
    {
        private static void Postfix(bool visible, float progress)
        {
            if(visible)
            {
                HeldItemEvents.ItemCharging(progress);
            }
        }
    }

}