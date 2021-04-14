using HarmonyLib;
using GTFO_VR.UI;


namespace GTFO_VR.Injections
{
    /// <summary>
    /// Replicate HP, oxygen and infection on the VR watch
    /// </summary>


    [HarmonyPatch(typeof(PlayerGuiLayer), nameof(PlayerGuiLayer.UpdateHealth))]
    class InjectWatchHP
    {
        static void Postfix(float health)
        {
            Watch.Current?.UpdateHealth(health);
        }
    }


    [HarmonyPatch(typeof(PlayerGuiLayer), nameof(PlayerGuiLayer.UpdateAir))]
    class InjectWatchAir
    {
        static void Postfix(float val)
        {
            Watch.Current?.UpdateAir(val);
        }
    }

    [HarmonyPatch(typeof(PlayerGuiLayer), nameof(PlayerGuiLayer.UpdateInfection))]
    class InjectWatchInfection
    {
        static void Postfix(float infection, float infectionHealthRel)
        {
            Watch.Current?.UpdateInfection(infection);
        }
    }
}
