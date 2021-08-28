using GTFO_VR.Events;
using HarmonyLib;
using Player;

namespace GTFO_VR.Injections.Events
{
    [HarmonyPatch(typeof(PlayerAgent), nameof(PlayerAgent.GiveHealth))]
    internal class InjectGiveHealthEvent
    {
        private static void Postfix(float amountRel)
        {
            ResourceUpdatedEvents.HealthGained(amountRel);
        }
    }

    [HarmonyPatch(typeof(PlayerAgent), nameof(PlayerAgent.GiveAmmoRel))]
    internal class InjectGiveAmmoEvent
    {
        private static void Postfix(float ammoStandardRel, float ammoSpecialRel, float ammoClassRel)
        {
            ResourceUpdatedEvents.AmmoGained(ammoStandardRel, ammoSpecialRel, ammoClassRel);
        }
    }

    [HarmonyPatch(typeof(PlayerAgent), nameof(PlayerAgent.GiveDisinfection))]
    internal class InjectGiveDisinfectionEvent
    {
        private static void Postfix(float amountRel)
        {
            ResourceUpdatedEvents.DisinfectionGained(amountRel);
        }
    }
}