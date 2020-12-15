using System;
using System.Collections.Generic;
using System.Text;
using GTFO_VR;
using HarmonyLib;
using Player;
using UnityEngine;
using GTFO_VR.UI;


namespace GTFO_VR_BepInEx.Core
{
    /// <summary>
    /// Replicate HP, oxygen and infection on the VR watch
    /// </summary>

    [HarmonyPatch(typeof(PlayerGuiLayer),"UpdateHealth")]
    class InjectWatchHP
    {
        static void Postfix(float health)
        {
            Watch.UpdateHealth(health);
        }
    }

    [HarmonyPatch(typeof(PlayerGuiLayer), "UpdateAir")]
    class InjectWatchAir
    {
        static void Postfix(float val)
        {
            Watch.UpdateAir(val);
        }
    }

    [HarmonyPatch(typeof(PlayerGuiLayer), "UpdateInfection")]
    class InjectWatchInfection
    {
        static void Postfix(float infection, float infectionHealthRel)
        {
            Watch.UpdateInfection(infection);
        }
    }
}
