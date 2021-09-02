using GTFO_VR.Events;
using HarmonyLib;
using Player;

namespace GTFO_VR.Injections.Events
{
   [HarmonyPatch(typeof(PlayerBackpackManager), nameof(PlayerBackpackManager.ReceiveAmmoGive))]
    internal class InjectGiveAmmoEvent
    {
        private static void Postfix(pAmmoGive data)
        {
            if (!data.targetPlayer.TryGetPlayer(out var player) || player.IsLocal)
            {
                ResourceUpdatedEvents.AmmoGained(data.ammoStandardRel, data.ammoSpecialRel, data.ammoClassRel);
            }
        }
    }

    // TODO find method that is called when *receiving* disinfection (instead of when giving it)
    /*[HarmonyPatch(typeof(PlayerAgent), nameof(PlayerAgent.GiveDisinfection))]
    internal class InjectGiveDisinfectionEvent
    {
        private static void Postfix(float amountRel)
        {
            ResourceUpdatedEvents.DisinfectionGained(amountRel);
        }
    }*/
}