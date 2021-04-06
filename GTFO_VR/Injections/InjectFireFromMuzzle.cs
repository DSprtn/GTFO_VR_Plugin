using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
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

    // ToDO - Replace fire from muzzle transpiler
    /*
    [HarmonyPatch(typeof(BulletWeapon), "Fire")]
    public static class InjectFireFromMuzzle_Patch
    {
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            int endIndex = -1;
            

            CodeInstruction getMuzzle = new CodeInstruction(OpCodes.Call, typeof(ItemEquippable).GetMethod("get_MuzzleAlign"));
            CodeInstruction getMuzzlePosition = new CodeInstruction(OpCodes.Call, typeof(Transform).GetMethod("get_position"));

            var codes = new List<CodeInstruction>(instructions);
            for (int i = 0; i < codes.Count; i++)
            {
                if (codes[i].ToString().Equals("callvirt UnityEngine.Vector3 FPSCamera::get_Position()"))
                {
                    endIndex = i;
                    break;
                }
            }


            Debug.Log("Replacing " + codes[endIndex - 1] + "  with - " + getMuzzle);
            codes[endIndex - 1] = getMuzzle;
            Debug.Log("Replacing " + codes[endIndex] + "  with - " + getMuzzlePosition);
            codes[endIndex] = getMuzzlePosition;
            Debug.Log("Removing " + codes[endIndex - 2]);
            codes.RemoveAt(endIndex - 2);

            if (endIndex == -1)
            {
                Debug.LogError("Failed to replace weapon fire origin, no anchor found!");
            }

            return codes.AsEnumerable();
        }*/

}

    /*
     * 
    ldarg.0
    IL_0092: call instance class Player.PlayerAgent Item::get_Owner()
    IL_0097: ldfld class FPSCamera Player.PlayerAgent::FPSCamera
    IL_009c: callvirt instance valuetype[UnityEngine.CoreModule] UnityEngine.Vector3 FPSCamera::get_Position()
    stloc.0

    
    ldarg.0
    Shotgun instruction - call UnityEngine.Transform ItemEquippable::get_MuzzleAlign()
    Shotgun instruction - callvirt UnityEngine.Vector3 UnityEngine.Transform::get_position()
    stloc.0

    */
