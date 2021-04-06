using System;
using System.Collections.Generic;
using System.Text;
using CellMenu;
using Gear;
using Globals;
using GTFO_VR;
using HarmonyLib;
using Player;
using UnityEngine;


namespace GTFO_VR_BepInEx.Core
{
    /// <summary>
    /// Melee weapons normally check for camera direction for hit targets, we ignore this in VR because the camera is detached from where you're aiming
    /// </summary>

    [HarmonyPatch(typeof(MeleeWeaponFirstPerson),"Setup")]
    class InjectMeleeIgnoreCameraDir
    {
        static void Postfix(MeleeWeaponFirstPerson __instance)
        {
            __instance.m_cameraDamageRayLength = 0f;
        }
    }
}
