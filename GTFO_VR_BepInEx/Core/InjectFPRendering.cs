using Gear;
using GTFO_VR;
using GTFO_VR.Input;
using HarmonyLib;
using Player;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;


namespace GTFO_VR_BepInEx.Core
{
    /// <summary>
    /// Makes most items render normally instead of 'flattened' to the screen
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
    /// Makes the hacking tool render normally instead of in 2D
    /// </summary>
    [HarmonyPatch(typeof(HologramGraphics), "FeedParams")]
    class InjectRenderFirstPersonHackingToolForVR
    {
        static void Postfix(HackingTool __instance, List<HologramGraphicsPart> ___m_holoParts, Transform ___m_holoSpaceTransform)
        {
            if(___m_holoSpaceTransform == null || ___m_holoParts.Count < 1 || VR_Global.hackingToolRenderingOverriden)
            {
                return;
            }
            for (int index = 0; index < ___m_holoParts.Count; ++index)
            {
                Material material = ___m_holoParts[index].m_material;
                material.DisableKeyword("ENABLE_FPS_RENDERING");
                material.DisableKeyword("FPS_RENDERING_ALLOWED");
            }
            VR_Global.hackingToolRenderingOverriden = true;
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
