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
using GTFO_VR.Util;
using HarmonyLib;
using Player;
using UnityEngine;
using static GTFO_VR.Util.WeaponArchetypeVRData;

namespace GTFO_VR_BepInEx.Core
{
    /// <summary>
    /// Handle weapon accuracy for double handed aiming/firing 'from the hip'
    /// </summary>

    [HarmonyPatch(typeof(BulletWeapon),"Fire")]
    class InjectAimSpreadInVR
    {
        static void Prefix(BulletWeapon __instance)
        {
            if (PlayerVR.VRSetup && VRSettings.UseVRControllers)
            {
                if (Utils.IsFiringFromADS())
                {
                    __instance.FPItemHolder.ItemAimTrigger = true;
                }
            }
        }
    }
}
