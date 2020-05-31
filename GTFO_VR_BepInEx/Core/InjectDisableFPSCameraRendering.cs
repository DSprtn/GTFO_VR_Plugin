using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using GTFO_VR;
using HarmonyLib;
using LevelGeneration;
using UnityEngine;

namespace GTFO_VR_BepInEx.Core
{
    /// <summary>
    /// Remove command calls from FPSCamera and move them to PlayerVR instead to use them after poses have been updated for better reprojection and a smoother experience
    /// </summary>

    [HarmonyPatch(typeof(FPSCamera), "LateUpdate")]
    public static class InjectDisableFPSCameraRendering_Patch
    {
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
			int startIndex = -1, endIndex = -1;

			var codes = new List<CodeInstruction>(instructions);
			for (int i = 0; i < codes.Count; i++)
			{
				if (codes[i].opcode == OpCodes.Ret)
				{
						endIndex = i; 
				}

				if(codes[i].ToString().Equals("callvirt Void AfterCameraUpdate()"))
				{
					startIndex = i + 1;
				}
			}
			if (startIndex > -1 && endIndex > -1)
			{
				
				codes.RemoveRange(startIndex, endIndex - startIndex);
			}

			if(startIndex == -1)
			{
				Debug.LogError("FPSCamera method changed, anchor not found!");
			}

			return codes.AsEnumerable();
		}
    }
}
