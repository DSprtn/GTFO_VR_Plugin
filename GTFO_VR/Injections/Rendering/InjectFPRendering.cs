﻿using HarmonyLib;
using Player;
using UnityEngine;

namespace GTFO_VR.Injections.Rendering
{
    /// <summary>
    /// Makes most items render normally instead of 'flattened' to the screen
    /// </summary>
    [HarmonyPatch(typeof(PlayerBackpackManager), nameof(PlayerBackpackManager.SetFPSRendering))]
    internal class InjectRenderFirstPersonItemsForVR
    {
        private static void Prefix(ref bool enable, GameObject go)
        {
            enable = false;
            foreach (var m in go.GetComponentsInChildren<MeshRenderer>(true))
            {
                if (m == null || m.sharedMaterials == null)
                {
                    continue;
                }
                foreach (Material mat in m.sharedMaterials)
                {
                    if (mat != null)
                    {
                        mat.DisableKeyword("ENABLE_FPS_RENDERING");
                        mat.DisableKeyword("FPS_RENDERING_ALLOWED");
                    }
                }
            }
        }
    }

    /// <summary>
    /// Set zeroing to work better up close/midrange
    /// Note: Might make sniper overshoot on long range shots, need to test this
    /// </summary>
    [HarmonyPatch(typeof(PlayerBackpackManager), nameof(PlayerBackpackManager.SetFPSRendering))]
    internal class InjectTweakSightZeroing
    {
        private static void Prefix(ref bool enable, GameObject go)
        {
            foreach (var m in go.GetComponentsInChildren<MeshRenderer>(true))
            {
                if (m == null || m.sharedMaterials == null)
                {
                    continue;
                }
                foreach (Material mat in m.sharedMaterials)
                {
                    if (mat != null)
                    {
                        if(mat.HasProperty("_ZeroOffset"))
                        {
                            mat.SetFloat("_ZeroOffset", .75f);
                        }
                    }
                }
            }
        }
    }

    /// <summary>
    /// Makes the hacking tool render normally instead of in 2D
    /// </summary>
    [HarmonyPatch(typeof(HologramGraphics), nameof(HologramGraphics.AddHoloPart))]
    internal class InjectRenderFirstPersonHackingToolForVR
    {
        private static void Prefix(HologramGraphicsPart part, HologramGraphics __instance)
        {
            Material material = part.m_renderer.sharedMaterial;
            material.DisableKeyword("ENABLE_FPS_RENDERING");
            material.DisableKeyword("FPS_RENDERING_ALLOWED");
        }
    }

    /// <summary>
    /// Disables FPS arms rendering, it's really wonky in VR so it's better to not see it at all
    /// </summary>

    [HarmonyPatch(typeof(PlayerFPSBody), nameof(PlayerFPSBody.Setup))]
    internal class InjectDisableFPSArms
    {
        private static void Postfix(PlayerFPSBody __instance)
        {
            __instance.SetGFXVisible(__instance.m_gfxArms, false);
            foreach (GameObject gfxArm in __instance.m_gfxArms)
                PlayerBackpackManager.SetFPSRendering(gfxArm, false);
        }
    }


    /// <summary>
    /// Disables FPS arms rendering, it's really wonky in VR so it's better to not see it at all
    /// </summary>

    [HarmonyPatch(typeof(PlayerFPSBody), nameof(PlayerFPSBody.UpdateModel))]
    internal class InjectDisableFPSArmsUpdate
    {
        private static void Postfix(PlayerFPSBody __instance)
        {
            __instance.SetGFXVisible(__instance.m_gfxArms, false);
            foreach (GameObject gfxArm in __instance.m_gfxArms)
                PlayerBackpackManager.SetFPSRendering(gfxArm, false);
        }
    }

    /// <summary>
    /// Disables FPS arms rendering, it's really wonky in VR so it's better to not see it at all
    /// </summary>

    [HarmonyPatch(typeof(PlayerFPSBody), nameof(PlayerFPSBody.SetVisible))]
    internal class InjectDisableFPSArmsSetVisible
    {
        private static void Postfix(PlayerFPSBody __instance, bool state)
        {
            if (state)
            {
                __instance.SetGFXVisible(__instance.m_gfxArms, false);
                foreach (GameObject gfxArm in __instance.m_gfxArms)
                    PlayerBackpackManager.SetFPSRendering(gfxArm, false);
            }
        }
    }

}