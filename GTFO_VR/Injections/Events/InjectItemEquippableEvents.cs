using GTFO_VR.Events;
using HarmonyLib;


namespace GTFO_VR.Injections.Events
{
    /// <summary>
    /// Add event calls for item equips
    /// </summary>

    [HarmonyPatch(typeof(ItemEquippable), nameof(ItemEquippable.OnWield))]
    class InjectItemEquippableEvents
    {
        static void Postfix(ItemEquippable __instance)
        {
            ItemEquippableEvents.ItemEquipped(__instance);
        }
    }
}
