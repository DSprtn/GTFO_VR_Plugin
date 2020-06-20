using Gear;
using GTFO_VR.Input;
using HarmonyLib;
using Player;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using UnityEngine;

namespace GTFO_VR_BepInEx.Core
{



    [HarmonyPatch(typeof(PlayerCharacterController), "SetColliderCrouched")]
    public static class InjectCrouchHeight_Patch
    {
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            int endIndex = -1;

            CodeInstruction oldInstruction = new CodeInstruction(OpCodes.Ldc_R4, .55f);
            CodeInstruction newInstruction = new CodeInstruction(OpCodes.Ldc_R4, .5f);
            Debug.Log("Listening for " + oldInstruction);


            var codes = new List<CodeInstruction>(instructions);
            for (int i = 0; i < codes.Count; i++)
            {
                if (codes[i].ToString().Equals(oldInstruction.ToString()))
                {
                    endIndex = i;
                    break;
                }
            }


            if(endIndex != -1)
            {
                Debug.Log("Replacing " + codes[endIndex] + "  with - " + newInstruction);
                codes[endIndex] = newInstruction;
            } else
            {
                Debug.LogError("Failed to replace crouch height, no anchor found!");
            }
            return codes.AsEnumerable();
        }
    }
}
