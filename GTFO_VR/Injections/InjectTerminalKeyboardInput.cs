using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using GTFO_VR;
using GTFO_VR.Core;
using HarmonyLib;
using LevelGeneration;
using UnityEngine;

namespace GTFO_VR_BepInEx.Core
{
    /// <summary>
    /// Handle virtual keyboard input
    /// </summary>
    /// 



    // ToDO - Fix VR keyboard transpilers
    /*

    [HarmonyPatch(typeof(LG_TERM_PlayerInteracting),"ParseInput")]
    public static class InjectTerminalKeyboardInput_Patch
    {

        static readonly MethodInfo VR_TXT_Input = SymbolExtensions.GetMethodInfo(() => VR_Keyboard.GetKeyboardInput());

        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            bool foundReplace = false;
            Debug.Log("New instruction!");
            Debug.Log(new CodeInstruction(OpCodes.Call, VR_TXT_Input));
            foreach (var instruction in instructions)
           {
                CodeInstruction curr = instruction;
                // Handle two different version of BepInEx
                if (instruction.ToString().Equals("call System.String UnityEngine.Input::get_inputString()") || instruction.ToString().Equals("call System.String get_inputString()"))
                {
                    Debug.Log("Replacing input.inputString call....");
                    foundReplace = true;
                    curr = new CodeInstruction(OpCodes.Call, VR_TXT_Input);
                 }
                yield return curr;
            }
            if (!foundReplace)
            {
                Debug.LogError("Failed to replace terminal input, no anchor found!");
            }
        }
    }

    [HarmonyPatch(typeof(LG_ComputerTerminalCommandInterpreter), "UpdateTerminalScreen")]
    public static class InjectTerminalKeyboardCorrectAnyKeySkip_Patch
    {
        static readonly MethodInfo VR_TXT_Input = SymbolExtensions.GetMethodInfo(() => VR_Keyboard.GetKeyboardInput());

        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            bool foundReplace = false;
            Debug.Log("New instruction!");
            Debug.Log(new CodeInstruction(OpCodes.Call, VR_TXT_Input));
            foreach (var instruction in instructions)
            {
                CodeInstruction curr = instruction;
                // Handle two different version of BepInEx
                if (instruction.ToString().Equals("call System.String UnityEngine.Input::get_inputString()") || instruction.ToString().Equals("call System.String get_inputString()"))
                {
                    Debug.Log("Replacing input.inputString call....");
                    foundReplace = true;
                    curr = new CodeInstruction(OpCodes.Call, VR_TXT_Input);
                }
                yield return curr;
            }
            if(!foundReplace)
            {
                Debug.LogError("Failed to replace terminal input, no anchor found!");
            }
        }
    }
    */
}
