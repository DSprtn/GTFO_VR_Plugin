using Gear;
using GTFO_VR.Input;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using UnityEngine;

namespace GTFO_VR_BepInEx.Core
{

    /// <summary>
    /// Makes Bioscanner work off of gun pos/rot instead of player pos/rot
    /// </summary>
    [HarmonyPatch(typeof(EnemyScannerGraphics), "UpdateCameraOrientation")]
    class InjectRenderBioScannerOffAimDir
    {
        static void Prefix(ref Vector3 position, ref Vector3 forward)
        {
            position = Controllers.GetAimFromPos();
            forward = Controllers.GetAimForward();
        }
    }


    [HarmonyPatch(typeof(EnemyScanner), "TryGetTaggableEnemies")]
    public static class InjectBioscanFromHand_Patch
    {
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            int endIndex = -1;

            CodeInstruction getOwner = new CodeInstruction(OpCodes.Call, typeof(EnemyScanner).GetMethod("get_Owner"));
            CodeInstruction getControllerTransform = new CodeInstruction(OpCodes.Call, typeof(Controllers).GetMethod("GetMainHandTransformRight"));
            Debug.Log("Listening for " + getOwner);


            var codes = new List<CodeInstruction>(instructions);
            for (int i = 0; i < codes.Count; i++)
            {
                if (codes[i].ToString().Equals(getOwner.ToString()))
                {
                    endIndex = i;
                    break;
                }
            }


            if(endIndex != -1)
            {
                Debug.Log("Replacing " + codes[endIndex] + "  with - " + getControllerTransform);
                codes[endIndex] = getControllerTransform;


                Debug.Log("Removing " + codes[endIndex + 2]);
                codes.RemoveAt(endIndex + 2);
                Debug.Log("Removing " + codes[endIndex + 1]);
                codes.RemoveAt(endIndex + 1);
                Debug.Log("Removing " + codes[endIndex - 1]);
                codes.RemoveAt(endIndex - 1);
            } else
            {
                Debug.LogError("Failed to replace bioscanner scan origin, no anchor found!");
            }
            return codes.AsEnumerable();
        }
    }
}
