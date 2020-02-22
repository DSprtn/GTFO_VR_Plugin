using System;
using System.Collections.Generic;
using System.Text;
using CellMenu;
using Globals;
using GTFO_VR;
using HarmonyLib;
using Player;
using UnityEngine;


namespace GTFO_VR_BepInEx.Core
{
    /// <summary>
    /// Get local player reference
    /// </summary>

    [HarmonyPatch(typeof(PlayerAgent),"Setup")]
    class InjectGetLocalPlayerAgentRef
    {
        static void Postfix(PlayerAgent __instance)
        {
            if(__instance.IsLocallyOwned)
            {
                PlayerVR.playerAgent = __instance;
            }
        }
    }
}
