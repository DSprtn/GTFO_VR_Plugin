using HarmonyLib;
using Player;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace GTFO_VR_BepInEx.Core
{
    /// <summary>
    /// Makes most items render normally instead of in the 2D UI camera
    /// </summary>
    [HarmonyPatch(typeof(PlayerBackpackManager), "SetFPSRendering")]
    class InjectRenderFirstPersonItemsForVR
    {
        static void Prefix(ref bool enable)
        {
            enable = false;
        }
    }

    /// <summary>
    /// Makes the hacking tool render normally instead of in the 2D UI camera
    /// </summary>
    [HarmonyPatch(typeof(HackingTool), "Setup")]
    class InjectRenderFirstPersonHackingToolForVR
    {
        static void Postfix(HackingTool __instance)
        {
            foreach (Transform t in __instance.gameObject.GetComponentsInChildren<Transform>())
            {
                PlayerBackpackManager.SetFPSRendering(t.gameObject, false);
            }
        }
    }

    /// <summary>
    /// Disables FPS arms rendering, it's really wonky in VR so it's better to not see it at all
    /// </summary>

    [HarmonyPatch(typeof(FirstPersonItemHolder), "SetupFPSRig")]
    class InjectDisableFPSArms
    {
        static void Postfix(FirstPersonItemHolder __instance)
        {
            foreach (Renderer renderer in __instance.FPSArms.GetComponentsInChildren<Renderer>())
            {
                renderer.enabled = false;
            }
        }
    }
}
