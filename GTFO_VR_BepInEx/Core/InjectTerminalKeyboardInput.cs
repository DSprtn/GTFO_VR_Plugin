using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using GTFO_VR;
using HarmonyLib;
using LevelGeneration;
using UnityEngine;

namespace GTFO_VR_BepInEx.Core
{
    /// <summary>
    /// Handle virtual keyboard input
    /// </summary>

    [HarmonyPatch(typeof(LG_TERM_PlayerInteracting),"ParseInput")]
    public static class InjectTerminalKeyboardInput_Patch
    {

        static MethodInfo VR_TXT_Input = SymbolExtensions.GetMethodInfo(() => VRGlobal.GetKeyboardInput());

        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            Debug.Log("New instruction!");
            Debug.Log(new CodeInstruction(OpCodes.Call, VR_TXT_Input));
            foreach (var instruction in instructions)
           {
                CodeInstruction curr = instruction;
                
                 if (instruction.ToString().Equals("call System.String get_inputString()"))
                 {
                    Debug.Log("Replacing input.inputString call....");
                    curr = new CodeInstruction(OpCodes.Call, VR_TXT_Input);
                 }
                yield return curr;
            }
        }
    }

    [HarmonyPatch(typeof(LG_ComputerTerminalCommandInterpreter), "UpdateTerminalScreen")]
    public static class InjectTerminalKeyboardCorrectAnyKeySkip_Patch
    {
        static MethodInfo VR_TXT_Input = SymbolExtensions.GetMethodInfo(() => VRGlobal.GetKeyboardInput());

        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            Debug.Log("New instruction!");
            Debug.Log(new CodeInstruction(OpCodes.Call, VR_TXT_Input));
            foreach (var instruction in instructions)
            {
                CodeInstruction curr = instruction;

                if (instruction.ToString().Equals("call System.String get_inputString()"))
                {
                    Debug.Log("Replacing input.inputString call....");
                    curr = new CodeInstruction(OpCodes.Call, VR_TXT_Input);
                }
                yield return curr;
            }
        }
    }
}
