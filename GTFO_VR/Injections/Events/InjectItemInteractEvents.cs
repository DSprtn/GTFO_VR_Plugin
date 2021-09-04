using HarmonyLib;
using LevelGeneration;
using GTFO_VR.Events;
using GTFO_VR.Core;
using Player;
using UnityEngine;

namespace GTFO_VR.Injections.Events
{
    [HarmonyPatch(typeof(LG_DoorButton), nameof(LG_DoorButton.Interact))]
    internal class InjectDoorButtonInteract
    {
        private static void Postfix(PlayerAgent source)
        {
            ItemInteractEvents.ItemInteracted(source);
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
    
    [HarmonyPatch(typeof(LG_ResourceContainer_Storage), nameof(LG_ResourceContainer_Storage.EnablePickupInteractions))]
    internal class InjectContainerStoragePickupInteract
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