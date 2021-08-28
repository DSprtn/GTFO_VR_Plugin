using GTFO_VR.UI;
using GTFO_VR.Events;
using HarmonyLib;

namespace GTFO_VR.Injections.UI
{
    /// <summary>
    /// Replicate HP, oxygen and infection on the VR watch
    /// </summary>

    [HarmonyPatch(typeof(PlayerGuiLayer), nameof(PlayerGuiLayer.UpdateHealth))]
    internal class InjectWatchHP
    {
        private static void Postfix(float health)
        {
            Watch.Current?.UpdateHealth(health);

            ResourceUpdatedEvents.HealthUpdated(health);
        }
    }

    [HarmonyPatch(typeof(PlayerGuiLayer), nameof(PlayerGuiLayer.UpdateAir))]
    internal class InjectWatchAir
    {
        private static void Postfix(float val)
        {
            Watch.Current?.UpdateAir(val);
        }
    }

    [HarmonyPatch(typeof(PlayerGuiLayer), nameof(PlayerGuiLayer.UpdateInfection))]
    internal class InjectWatchInfection
    {
        private static void Postfix(float infection, float infectionHealthRel)
        {
            Watch.Current?.UpdateInfection(infection);
        }
    }
}