using HarmonyLib;
using LevelGeneration;
using GTFO_VR.Events;
using Player;

namespace GTFO_VR.Injections.Events
{
    [HarmonyPatch(typeof(LG_DoorButton), nameof(LG_DoorButton.Interact))]
    internal class InjectDoorButtonInteractEvents
    {
        private static void Postfix(PlayerAgent source)
        {
            Log.Info("Interacted with door button. SourceSlotIndex: " + source.PlayerSlotIndex + " at " + Time.time);
            ItemInteractEvents.ItemInteracted();
        }
    }

    [HarmonyPatch(typeof(LG_SecurityDoorButton), nameof(LG_SecurityDoorButton.Interact))]
    internal class InjectSecurityDoorButtonInteractEvents
    {
        private static void Postfix(PlayerAgent source)
        {
            Log.Info("Interacted with security door button. SourceSlotIndex: " + source.PlayerSlotIndex + " at " + Time.time);
            ItemInteractEvents.ItemInteracted();
        }
    }

    [HarmonyPatch(typeof(LG_PickupItem_Sync), nameof(LG_PickupItem_Sync.AttemptInteract))]
    internal class InjectAttemptInteractPickupItemEvents
    {
        private static void Postfix(pPickupItemInteraction interaction)
        {
            if (interaction.pPlayer.GetPlayer(out var player) && player.IsLocal)
            {
                ItemInteractEvents.ItemInteracted();
            }
        }
    }

    [HarmonyPatch(typeof(iPickupItemSync), nameof(iPickupItemSync.AttemptPickupInteraction))]
    internal class InjectPickupItemInteractionEvents
    {
        private static void Postfix(PlayerAgent source)
        {
            ItemInteractEvents.ItemInteracted();
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
            ItemInteractEvents.ItemInteracted();
        }
    }

    [HarmonyPatch(typeof(PlayerInventoryBase), nameof(PlayerInventoryBase.SetFlashlightEnabled))]
    internal class InjectFlashlightEnabledEvents
    {
        private static bool m_lastFlashlightState;

        private static void Postfix(bool enabled)
        {
            if (m_lastFlashlightState != enabled)
            {
                ItemInteractEvents.FlashlightToggled(enabled);
                m_lastFlashlightState = enabled;
            }
        }
    }
}