using HarmonyLib;
using LevelGeneration;
using GTFO_VR.Events;
using Player;

namespace GTFO_VR.Injections.Events
{
    [HarmonyPatch(typeof(LG_ComputerTerminal), nameof(LG_ComputerTerminal.OnInteract))]
    internal class InjectAttemptInteractComputerTerminalEvents
    {
        private static void Postfix(PlayerAgent interactionSource)
        {
            ItemInteractEvents.ItemInteracted(interactionSource);
        }
    }

    [HarmonyPatch(typeof(LG_ComputerTerminal), nameof(GenericSmallPickupItem_Core.OnInteractionPickUp))]
    internal class InjectSmallItemPickupEvents
    {
        private static void Postfix(PlayerAgent player)
        {
            ItemInteractEvents.ItemInteracted(player);
        }
    }

    [HarmonyPatch(typeof(LG_ComputerTerminal), nameof(ArtifactPickup_Core.OnInteractionPickUp))]
    internal class InjectArtifactPickupEvents
    {
        private static void Postfix(PlayerAgent player)
        {
            ItemInteractEvents.ItemInteracted(player);
        }
    }

    [HarmonyPatch(typeof(LG_ComputerTerminal), nameof(CommodityPickup_Core.OnInteractionPickUp))]
    internal class InjectCommodityPickupEvents
    {
        private static void Postfix(PlayerAgent player)
        {
            ItemInteractEvents.ItemInteracted(player);
        }
    }

    [HarmonyPatch(typeof(LG_ComputerTerminal), nameof(CarryItemPickup_Core.OnInteractionPickUp))]
    internal class InjectCarryItemPickupEvents
    {
        private static void Postfix(PlayerAgent player)
        {
            ItemInteractEvents.ItemInteracted(player);
        }
    }

    [HarmonyPatch(typeof(LG_ComputerTerminal), nameof(Gear.ResourcePackPickup.OnInteractionPickUp))]
    internal class InjectResourcePackPickupEvents
    {
        private static void Postfix(PlayerAgent player)
        {
            ItemInteractEvents.ItemInteracted(player);
        }
    }

    [HarmonyPatch(typeof(LG_ComputerTerminal), nameof(KeyItemPickup_Core.OnInteractionPickUp))]
    internal class InjectKeyItemPickupEvents
    {
        private static void Postfix(PlayerAgent player)
        {
            ItemInteractEvents.ItemInteracted(player);
        }
    }

    [HarmonyPatch(typeof(LG_ComputerTerminal), nameof(ConsumablePickup_Core.OnInteractionPickUp))]
    internal class InjectConsumablePickupEvents
    {
        private static void Postfix(PlayerAgent player)
        {
            ItemInteractEvents.ItemInteracted(player);
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