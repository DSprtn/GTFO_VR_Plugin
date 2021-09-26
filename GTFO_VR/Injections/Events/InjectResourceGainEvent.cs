using GTFO_VR.Events;
using HarmonyLib;
using Player;

namespace GTFO_VR.Injections.Events
{
    /// <summary>
    /// Add event calls for player resource gains
    /// </summary>

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
}