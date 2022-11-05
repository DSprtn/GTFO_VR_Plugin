using HarmonyLib;
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
    /// Set zeroing to work better up close/midrange, and fix sights for thermals
    /// Note: Might make sniper overshoot on long range shots, need to test this
    /// </summary>
    [HarmonyPatch(typeof(PlayerBackpackManager), nameof(PlayerBackpackManager.SetFPSRendering))]
    internal class InjectTweakSights
    {
        private static void Prefix(ref bool enable, GameObject go)
        {
            // This probably only needs to be run once for each unique GO, but I'm not going to be the one to break it.
            foreach (var m in go.GetComponentsInChildren<MeshRenderer>(true))
            {
                if (m == null || m.sharedMaterials == null)
                {
                    continue;
                }

                bool isThermalShaderObject = false;

                foreach (Material mat in m.sharedMaterials)
                {
                    if (mat != null)
                    {
                        if(mat.HasProperty("_ZeroOffset"))
                        {
                            mat.SetFloat("_ZeroOffset", .75f);
                        }

                        if(mat.shader.name.Equals("Unlit/HolographicSight_Thermal"))
                        {
                            // Will Activate and adjust non-thermal sight below
                            isThermalShaderObject = true;

                            // Make visible even off-angle
                            mat.SetFloat("_OffAngleFade", 0f);
                            
                            // Hide the flat reticules
                            mat.SetFloat("_ProjSize1", 0);                           
                            mat.SetFloat("_ProjSize2", 0);
                            mat.SetFloat("_ProjSize3", 0);
                        }
                    }
                }

                if (isThermalShaderObject)
                {
                    // Every thermal sight has normal sight GO hidden inside of it, deactivated.
                    // It has a default 3-layer holo sight material. Adapting the reticules from 
                    // the thermal material does not work well, so we leave them alone.

                    GameObject sightGlass = null;
                    Transform targetParent = m.transform.parent;
                    int childCount = targetParent.childCount;
                    for(int i = 0; i < childCount; i++)
                    {
                        Transform child = targetParent.GetChild(i);
                        if (child.name.EndsWith("_Glass"))
                        {
                            sightGlass = child.gameObject;
                            break;
                        }
                    }

                    // Make activate, and adjust so it's not buried behind the thermal sight.
                    // Only do this once!
                    if (sightGlass != null && !sightGlass.active)
                    {
                        // Offset from existing position or thermal sight is different for each, but end value is the same.
                        // Will likely need tweaking for future rundowns if there are more thermal guns.
                        sightGlass.transform.localPosition = new Vector3(0, 0, -0.0156f);
                        sightGlass.SetActive(true);

                        Material sightMat = sightGlass.GetComponent<MeshRenderer>().GetSharedMaterial();
                        if (sightMat != null)
                        {
                            sightMat.SetFloat("_SightDirt", 0); // remove extra glare
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