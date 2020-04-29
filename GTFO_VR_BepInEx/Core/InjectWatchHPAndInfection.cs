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
    /// Hacky thing to get the GUI visible to the player inside the HMD --- 
    /// Basically moves all UI elements more towards the center to account for the bigger screen size necessary for lens distortion
    /// </summary>

    [HarmonyPatch(typeof(PlayerGuiLayer),"UpdateHealth")]
    class InjectWatchHP
    {
        static void Postfix(float health)
        {
            Watch.UpdateHealth(health);
           
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
