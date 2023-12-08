using HarmonyLib;
using Assets.SteamVR_Standalone.Standalone;
using Player;
using System.Collections.Generic;
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
    /// Set zeroing to work better up close/midrange, and fix thermals and other broken sights.
    /// Note: Might make sniper overshoot on long range shots, need to test this
    /// </summary>
    [HarmonyPatch(typeof(PlayerBackpackManager), nameof(PlayerBackpackManager.SetFPSRendering))]
    internal class InjectTweakSights
    {
        private static Material blitMaterial;

        private static Dictionary<string, Texture2D> crosshairTextureCache = new Dictionary<string, Texture2D>();

        private static Texture2D flipAndShiftCrosshairTexture( Texture2D source, float verticalShift )
        {
            if (blitMaterial == null)
            {
                blitMaterial = new Material(VRShaders.GetShader(VRShaders.VRShader.blitFlip));
            }

            // Texture is not writable, so we need to render a copy and work with that instead.
            RenderTexture renderTex = RenderTexture.GetTemporary(
                        source.width,
                        source.height,
                        0,
                        RenderTextureFormat.Default,
                        RenderTextureReadWrite.Linear);

            RenderTexture tempTex = RenderTexture.GetTemporary(
                        source.width,
                        source.height,
                        0,
                        RenderTextureFormat.Default,
                        RenderTextureReadWrite.Linear);


            Graphics.Blit(source, tempTex, new Vector2(1, 1f), new Vector2(0, verticalShift));  // Shift texture up.
            Graphics.Blit(tempTex, renderTex, blitMaterial); // flip texture using another blip, because negative scale above doesn't work.

            RenderTexture previous = RenderTexture.active;
            RenderTexture.active = renderTex;
            Texture2D readableText = new Texture2D(source.width, source.height);
            readableText.ReadPixels(new Rect(0, 0, renderTex.width, renderTex.height), 0, 0 );
            readableText.Apply();
            RenderTexture.active = previous;
            RenderTexture.ReleaseTemporary(renderTex);
            RenderTexture.ReleaseTemporary(tempTex);
            return readableText;
        }

        private static void fixCenteredCrosshairTexture( Material mat, string textureName, float verticalShift )
        {
            var rawTexture = mat.GetTexture(textureName);

            // This apparently gets called multiple times, and sometimes the texture is empty
            if (rawTexture == null)
            {
                return;
            }

            var texture = rawTexture.Cast<Texture2D>();

            string cacheKey = mat.name + textureName;

            // Cache textures so we don't generate a million copies
            // Not really necessary as game only calls hook once for each object and caches them
            if (crosshairTextureCache.ContainsKey(cacheKey))
            {
                crosshairTextureCache.TryGetValue(cacheKey, out texture);
            }
            else
            {
                texture = flipAndShiftCrosshairTexture(texture, verticalShift);
                crosshairTextureCache.Add(cacheKey, texture);
            }
 
            mat.SetTexture(textureName, texture);
        }

        private static void Prefix(ref bool enable, GameObject go)
        {
            // This hook is only called once for each GO
            foreach (var m in go.GetComponentsInChildren<MeshRenderer>(true))
            {
                if (m == null || m.sharedMaterials == null)
                {
                    continue;
                }

                GameObject sightGO = m.gameObject;

                // Bataldo Custom K33
                if (sightGO.name.Equals("Sight_24_picture"))
                {
                    // This sight is culled from the wrong direction in VR
                    // Crosshairs do suggest it is actually being flipped for some reason.
                    // Aligns correctly when rotated 180
                    sightGO.transform.localRotation = Quaternion.Euler(0, 180, 0);
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

                        // As of writing, all problematic sights use this shader
                        if (mat.shader.name.Equals("Unlit/HolographicSight_3Layers"))
                        {
                            // All working sights already have this keyword enabled
                            if (!mat.IsKeywordEnabled("ALTERNATIVE_PROJECTION_MODE"))
                            {
                                // And enabling it makes most of them at least usable
                                mat.EnableKeyword("ALTERNATIVE_PROJECTION_MODE");

                                // Accrat ND6 Heavy SMG
                                if (mat.name.Equals("Sight_2_1"))
                                {
                                    // The muzzle is also misaligned
                                    // This is corrected in InjectMuzzleAlignCorrection

                                    mat.SetFloat("_ProjSize1", 0.6f);
                                    mat.SetFloat("_ProjSize2", 0.2f);
                                    mat.SetFloat("_ProjSize3", 0.6f);

                                    mat.SetFloat("_ProjDist1", 100);
                                    mat.SetFloat("_ProjDist2", 20);
                                    mat.SetFloat("_ProjDist3", 5);

                                    mat.SetFloat("_ZeroOffset", 0.25f);

                                    // Tune down the dirt so you're not blinded
                                    mat.SetFloat("_SightDirt", 1);

                                    // Reticle is basically invisible unless completely center because of how it blends with the sight.
                                    // Make it glow like a dotsight should.
                                    float sightGlowMultiplier = 9;
                                    float SlightsightGlowMultiplier = 3;
                                    mat.SetColor("_ReticuleColorA", new Color(0, sightGlowMultiplier, 0.7935257f * sightGlowMultiplier, 1f));
                                    mat.SetColor("_ReticuleColorB", new Color(0, SlightsightGlowMultiplier, 0.7935257f * SlightsightGlowMultiplier, 1f));
                                    mat.SetColor("_ReticuleColorC", new Color(0, SlightsightGlowMultiplier, 0.7935257f * SlightsightGlowMultiplier, 1f));
                                }

                                // Techman Klaust 6 Burst Cannon
                                if (mat.name.Equals("Sight_9_1"))
                                {
                                    mat.SetFloat("_ProjSize2", 1.5f);
                                    mat.SetFloat("_ProjSize3", 0.5f);
                                }

                                //Drekker Pres Mod 556
                                if (mat.name.Equals("Sight_15_1"))
                                {
                                    mat.SetFloat("_ProjSize1", 0.75f);
                                    mat.SetFloat("_ProjSize3", 0.5f);

                                    // Tune down the dirt so you're not blinded, but still get the sight outline
                                    mat.SetFloat("_SightDirt", 1);

                                    // Reticle is basically invisible unless completely center because of how it blends with the sight.
                                    // Make it glow like a dotsight should.
                                    float sightGlowMultiplier = 9;
                                    mat.SetColor("_ReticuleColorA", new Color(0, sightGlowMultiplier, 0.13f * sightGlowMultiplier, 1f));
                                    mat.SetColor("_ReticuleColorB", new Color(0, sightGlowMultiplier, 0, 1f));
                                    mat.SetColor("_ReticuleColorC", new Color(0, sightGlowMultiplier, 0, 1f));
                                }

                                // Techman Arbalist
                                if (mat.name.Equals("Sight_21_1"))
                                {
                                    // Axis is reversed and only visible from the front
                                    mat.SetColor("_AxisX", new Color(-1, 0, 0, 0));
                                    mat.SetColor("_AxisY", new Color(0, 1, 0, 0));
                                    mat.SetColor("_AxisZ", new Color(0, 0, -1, 0));

                                    mat.SetFloat("_ZeroOffset", -1);

                                    mat.SetFloat("_ProjSize1", 31f);
                                    mat.SetFloat("_ProjDist1", 0.001f);

                                    mat.SetFloat("_ProjSize2", 0.75f);

                                    mat.SetFloat("_ProjDist3", 15);
                                    mat.SetFloat("_ProjSize3", 0.5f);

                                    // The crosshair texture is centered, but the other two reticles are not.
                                    // This results in nothing lining up. The crosshair texture is also upside down.
                                    // Flip it and shift its position to match the others
                                    fixCenteredCrosshairTexture(mat, "_ReticuleB", 0.04f);
                                }
                            }

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