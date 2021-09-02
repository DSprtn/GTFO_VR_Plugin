using HarmonyLib;
using LevelGeneration;
using GTFO_VR.Events;
using GTFO_VR.Core;
using Player;
using UnityEngine;

namespace GTFO_VR.Injections.Events
{
    [HarmonyPatch(typeof(LG_Door_Sync), nameof(LG_Door_Sync.AttemptInteract))]
    internal class InjectAttemptInteractDoorEvents
    {
        private static void Postfix(pDoorInteraction interaction)
        {
            if (interaction.user.TryGetPlayer(out var player) && player.IsLocal)
            {
                ItemInteractEvents.ItemInteracted();
            }
        }
    }

    [HarmonyPatch(typeof(LG_PickupItem_Sync), nameof(LG_PickupItem_Sync.AttemptInteract))]
    internal class InjectAttemptInteractPickupItemEvents
    {
        private static void Postfix(pPickupItemInteraction interaction)
        {
            if (interaction.pPlayer.TryGetPlayer(out var player) && player.IsLocal)
            {
                ItemInteractEvents.ItemInteracted();
            }
        }
    }
    
    [HarmonyPatch(typeof(LG_ResourceContainer_Sync), nameof(LG_ResourceContainer_Sync.AttemptInteract))]
    internal class InjectAttemptInteractResourceContainerEvents
    {
        private static void Postfix()
        {
            ItemInteractEvents.ItemInteracted();
        }
    }

    [HarmonyPatch(typeof(LG_ComputerTerminal), nameof(LG_ComputerTerminal.OnInteract))]
    internal class InjectAttemptInteractComputerTerminalEvents
    {
        private static void Postfix(PlayerAgent interactionSource)
        {
            ItemInteractEvents.ItemInteracted(interactionSource);
        }
    }
}