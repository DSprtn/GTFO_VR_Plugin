using System;
using System.Collections.Generic;
using System.Text;
using CellMenu;
using Globals;
using GTFO_VR;
using GTFO_VR.Events;
using HarmonyLib;
using Player;
using UnityEngine;


namespace GTFO_VR_BepInEx.Core
{
    /// <summary>
    /// Add event call onItemEquipped
    /// </summary>

    [HarmonyPatch(typeof(ItemEquippable),"OnWield")]
    class InjectItemEquippableEvents
    {
        static void Postfix(ItemEquippable __instance)
        {
            ItemEquippableEvents.ItemEquipped(__instance);
        }
    }
}
