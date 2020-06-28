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
    [HarmonyPatch(typeof(Interact_Timed), "EvaluateTimedInteraction")]
    class InjectInteractionFromPos
    {
        static void Prefix(ref Vector3 ___m_triggerStartAgentWorldPos)
        {
            ___m_triggerStartAgentWorldPos = HMD.GetVRInteractionFromPosition();
        }
    }

    [HarmonyPatch(typeof(GuiManager), "IsOnScreen")]
    class InjectDisableOnScreenCheck
    {
        static bool Prefix(ref bool __result)
        {
            __result = true;
            return false;
        }
    }

    [HarmonyPatch(typeof(MeleeWeaponFirstPerson), "CheckForAttackTargets")]
    public static class InjectClosestPlayerNode_Patch
    {
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            bool foundReplace = false;
            CodeInstruction target = new CodeInstruction(OpCodes.Ldc_R4, 0.0f);
            foreach (var instruction in instructions)
            {
                CodeInstruction curr = instruction;
                if (curr.ToString().Equals(target.ToString()))
                {
                    Debug.Log("Old instruction == " + curr);
                    curr = new CodeInstruction(OpCodes.Ldc_R4, -1.0f);
                    Debug.Log("New instruction == " + curr);
                    foundReplace = true;
                }
                yield return curr;
            }
            if (!foundReplace)
            {
                Debug.LogError("Failed to replace dot check replacement, no anchor found!");
            }
        }
    }


    [HarmonyPatch(typeof(CarryItemEquippableFirstPerson), "UpdateInsertOrDropItem")]
    public static class InjectDropInteractablesInteraction_Patch
    {
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            Debug.Log("Patching interactable dropping interaction...");
            int endIndex = -1;

            CodeInstruction getCameraForward = new CodeInstruction(OpCodes.Callvirt, typeof(FPSCamera).GetMethod("get_Forward"));

            CodeInstruction getInteractFromPos = new CodeInstruction(OpCodes.Call, typeof(HMD).GetMethod(nameof(HMD.GetVRInteractionFromPosition)));

            CodeInstruction getInteractDir = new CodeInstruction(OpCodes.Call, typeof(HMD).GetMethod(nameof(HMD.GetVRInteractionLookDir)));

            Debug.Log("Listening for " + getCameraForward);


            var codes = new List<CodeInstruction>(instructions);
            for (int i = 0; i < codes.Count; i++)
            {
                if (codes[i].ToString().Equals(getCameraForward.ToString()))
                {
                    endIndex = i;
                    break;
                }
            }


            if (endIndex != -1)
            {
                Debug.Log("Replacing " + codes[endIndex] + "  with - " + getInteractDir);
                codes[endIndex] = getInteractDir;
                Debug.Log("Replacing " + codes[endIndex - 4] + "  with - " + getInteractFromPos);
                codes[endIndex - 4] = getInteractFromPos;


                Debug.Log("Removing " + codes[endIndex - 1]);
                codes.RemoveAt(endIndex - 1);
                Debug.Log("Removing " + codes[endIndex - 2]);
                codes.RemoveAt(endIndex - 2);
                Debug.Log("Removing " + codes[endIndex - 3]);
                codes.RemoveAt(endIndex - 3);
                Debug.Log("Removing " + codes[endIndex - 5]);
                codes.RemoveAt(endIndex - 5);
                Debug.Log("Removing " + codes[endIndex - 6]);
                codes.RemoveAt(endIndex - 6);
                Debug.Log("Removing " + codes[endIndex - 7]);
                codes.RemoveAt(endIndex - 7);
            }
            else
            {
                Debug.LogError("Failed to replace interaction drop mechanics, no anchor found!");
            }
            return codes.AsEnumerable();
        }
    }

    [HarmonyPatch(typeof(LockMelterFirstPerson), "UpdateApplyActionInput")]
    public static class InjectLockMelterInteraction_Patch
    {
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            Debug.Log("Patching lockMelter interaction...");
            int endIndex = -1;

            CodeInstruction getCameraForward = new CodeInstruction(OpCodes.Callvirt, typeof(FPSCamera).GetMethod("get_Forward"));

            CodeInstruction getInteractFromPos = new CodeInstruction(OpCodes.Call, typeof(HMD).GetMethod(nameof(HMD.GetVRInteractionFromPosition)));

            CodeInstruction getInteractDir = new CodeInstruction(OpCodes.Call, typeof(HMD).GetMethod(nameof(HMD.GetVRInteractionLookDir)));

            Debug.Log("Listening for " + getCameraForward);


            var codes = new List<CodeInstruction>(instructions);
            for (int i = 0; i < codes.Count; i++)
            {
                if (codes[i].ToString().Equals(getCameraForward.ToString()))
                {
                    endIndex = i;
                    break;
                }
            }


            if (endIndex != -1)
            {
                Debug.Log("Replacing " + codes[endIndex] + "  with - " + getInteractDir);
                codes[endIndex] = getInteractDir;
                Debug.Log("Replacing " + codes[endIndex - 4] + "  with - " + getInteractFromPos);
                codes[endIndex - 4] = getInteractFromPos;


                Debug.Log("Removing " + codes[endIndex - 1]);
                codes.RemoveAt(endIndex -1);
                Debug.Log("Removing " + codes[endIndex - 2]);
                codes.RemoveAt(endIndex - 2);
                Debug.Log("Removing " + codes[endIndex - 3]);
                codes.RemoveAt(endIndex - 3);
                Debug.Log("Removing " + codes[endIndex - 5]);
                codes.RemoveAt(endIndex - 5);
                Debug.Log("Removing " + codes[endIndex - 6]);
                codes.RemoveAt(endIndex - 6);
                Debug.Log("Removing " + codes[endIndex - 7]);
                codes.RemoveAt(endIndex - 7);
            }
            else
            {
                Debug.LogError("Failed to replace lockMelter interaction, no anchor found!");
            }
            return codes.AsEnumerable();
        }
    }

    [HarmonyPatch(typeof(ResourcePackFirstPerson), "UpdateApplyActionInput")]
    public static class InjectResourcePackInteraction_Patch
    {
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            Debug.Log("Patching resourcePack interaction...");
            int endIndex = -1;

            CodeInstruction getCameraForward = new CodeInstruction(OpCodes.Callvirt, typeof(FPSCamera).GetMethod("get_Forward"));

            CodeInstruction getInteractFromPos = new CodeInstruction(OpCodes.Call, typeof(HMD).GetMethod(nameof(HMD.GetVRInteractionFromPosition)));

            CodeInstruction getInteractDir = new CodeInstruction(OpCodes.Call, typeof(HMD).GetMethod(nameof(HMD.GetVRInteractionLookDir)));

            Debug.Log("Listening for " + getCameraForward);


            var codes = new List<CodeInstruction>(instructions);
            for (int i = 0; i < codes.Count; i++)
            {
                if (codes[i].ToString().Equals(getCameraForward.ToString()))
                {
                    endIndex = i;
                    break;
                }
            }


            if (endIndex != -1)
            {
                Debug.Log("Replacing " + codes[endIndex] + "  with - " + getInteractDir);
                codes[endIndex] = getInteractDir;
                Debug.Log("Replacing " + codes[endIndex - 4] + "  with - " + getInteractFromPos);
                codes[endIndex - 4] = getInteractFromPos;


                Debug.Log("Removing " + codes[endIndex - 1]);
                codes.RemoveAt(endIndex - 1);
                Debug.Log("Removing " + codes[endIndex - 2]);
                codes.RemoveAt(endIndex - 2);
                Debug.Log("Removing " + codes[endIndex - 3]);
                codes.RemoveAt(endIndex - 3);
                Debug.Log("Removing " + codes[endIndex - 5]);
                codes.RemoveAt(endIndex - 5);
                Debug.Log("Removing " + codes[endIndex - 6]);
                codes.RemoveAt(endIndex - 6);
                Debug.Log("Removing " + codes[endIndex - 7]);
                codes.RemoveAt(endIndex - 7);
            }
            else
            {
                Debug.LogError("Failed to replace resource pack interaction, no anchor found!");
            }
            return codes.AsEnumerable();
        }
    }

    [HarmonyPatch(typeof(PlayerInteraction), "UpdateWorldInteractions")]
    public static class InjectPlayerInteraction_Patch
    {
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            Debug.Log("Patching playerInteraction pos");
            int endIndex = -1;

            CodeInstruction targetInstruction = new CodeInstruction(OpCodes.Ldfld, typeof(PlayerInteraction).GetField(nameof(PlayerInteraction.m_playerCam)));

            CodeInstruction getInteractFromPos = new CodeInstruction(OpCodes.Call, typeof(HMD).GetMethod(nameof(HMD.GetVRInteractionFromPosition)));

            //CodeInstruction getInteractDir = new CodeInstruction(OpCodes.Call, typeof(HMD).GetMethod(nameof(HMD.GetVRInteractionFromPosition)));

            Debug.Log("Listening for " + targetInstruction);


            var codes = new List<CodeInstruction>(instructions);
            for (int i = 0; i < codes.Count; i++)
            {
                if (codes[i].ToString().Equals(targetInstruction.ToString()) && codes[i + 1].opcode == OpCodes.Callvirt)
                {
                    endIndex = i;
                    break;
                }
            }


            if (endIndex != -1)
            {
                CodeInstruction replacePos = new CodeInstruction(OpCodes.Stloc_0);

                Debug.Log("Inserting ... " + replacePos);
                codes.Insert(endIndex + 4, replacePos);
                Debug.Log("Inserting ... " + getInteractFromPos);
                codes.Insert(endIndex + 4, getInteractFromPos);

            }
            else
            {
                Debug.LogError("Failed to replace playerInteraction pos, no anchor found!");
            }
            return codes.AsEnumerable();
        }
    }
}
