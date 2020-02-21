using System;
using System.Collections.Generic;
using System.Text;
using CellMenu;
using Gear;
using Globals;
using GTFO_VR;
using GTFO_VR.Core;
using GTFO_VR.Events;
using GTFO_VR.Input;
using HarmonyLib;
using Player;
using UnityEngine;


namespace GTFO_VR_BepInEx.Core
{
    /// <summary>
    /// Add event call onItemEquipped
    /// </summary>

    [HarmonyPatch(typeof(BulletWeapon),"Fire")]
    class InjectAimSpreadInVR
    {
        static void Prefix(BulletWeapon __instance)
        {
            if (PlayerVR.VRSetup && VRSettings.UseVRControllers)
            {
                __instance.FPItemHolder.ItemAimTrigger = true;
            }
        }
    }
}
