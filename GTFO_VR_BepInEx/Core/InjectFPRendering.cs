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
    /// Makes Bioscanner work off of gun pos/rot instead of player pos/rot
    /// </summary>
    [HarmonyPatch(typeof(EnemyScannerGraphics), "UpdateCameraOrientation")]
    class InjectRenderBioScannerOffAimDir
    {
        static void Prefix(ref Vector3 position, ref Vector3 forward)
        {
            position = Controllers.GetAimFromPos();
            forward = Controllers.GetAimForward();
        }
    }

    
    /// <summary>
    /// Modifies ref screenres to match VR resolution, allowing markers to be placed correctly in the HMD
    /// </summary>
    [HarmonyPatch(typeof(GuiManager), "ScreenToGUIScaled")]
    class InjectVRNavMarkers
    {
        static bool Prefix(GuiManager __instance, Vector3 screenPos, Vector2 refResolution, float canvasScaleFactor, ref Vector3 __result)
        {
            Vector3 vector3 = screenPos;
            Vector2 vector2 = new Vector2(refResolution.x / (float)VRGlobal.VR_Resolution.width, refResolution.y / (float)VRGlobal.VR_Resolution.height);
            float num1 = vector3.x - (float)VRGlobal.VR_Resolution.width / 2f;
            double num2 = (double)vector3.y - (float)VRGlobal.VR_Resolution.height / 2f;
            float x = num1 / canvasScaleFactor;
            double num3 = (double)canvasScaleFactor;
            float y = (float)(num2 / num3);
            vector3 = new Vector3(x, y, vector3.z);
            __result = vector3;
            return false;
        }
    }

    /// <summary>
    /// Makes all nav markers a tad smaller for VR
    /// </summary>
    [HarmonyPatch(typeof(NavMarker), "Setup")]
    class InjectVRNavMarkerScale
    {
        static void Postfix(NavMarker __instance)
        {
            __instance.m_initScale *= 0.65f;
        }
    }

    /// <summary>
    /// Makes the hacking tool render normally instead of in the 2D UI camera
    /// </summary>
    [HarmonyPatch(typeof(HologramGraphics), "FeedParams")]
    class InjectRenderFirstPersonHackingToolForVR
    {
        static void Postfix(HackingTool __instance, List<HologramGraphicsPart> ___m_holoParts, Transform ___m_holoSpaceTransform)
        {
            if(___m_holoSpaceTransform == null || ___m_holoParts.Count < 1 || VRGlobal.hackingToolRenderingOverriden)
            {
                return;
            }
            for (int index = 0; index < ___m_holoParts.Count; ++index)
            {
                Material material = ___m_holoParts[index].m_material;
                material.DisableKeyword("ENABLE_FPS_RENDERING");
                material.DisableKeyword("FPS_RENDERING_ALLOWED");
            }
            VRGlobal.hackingToolRenderingOverriden = true;
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
