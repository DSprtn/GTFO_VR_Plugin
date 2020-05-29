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
    /// Weapons are always firing as if they were fired from the hip, so we make the game think we're always aiming
    /// </summary>

    [HarmonyPatch(typeof(BulletWeapon),"Fire")]
    class InjectAimSpreadInVR
    {
        static void Prefix(BulletWeapon __instance)
        {
            if (PlayerVR.VRSetup)// && VRSettings.UseVRControllers)
            {
                __instance.FPItemHolder.ItemAimTrigger = true;

                // Toggle for hipfire-like spread when not using double handed aiming, some people might prefer playing it single handed though so I'm leaving this disabled
                /*
                VRWeaponData data = WeaponArchetypeVRData.GetVRWeaponData(__instance.ArchetypeName);
                if(!data.allowsDoubleHanded || (data.allowsDoubleHanded && Controllers.aimingTwoHanded)) {
                    __instance.FPItemHolder.ItemAimTrigger = true;
                }
                */
            }
        }
    }
}
