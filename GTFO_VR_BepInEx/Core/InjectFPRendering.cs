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
            double num2 = (double)vector3.y - (float)VRGlobal.VR_Resolution.height / 2.0;
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

    /*
/// <summary>
/// Turns off 2D rendering for all nav markers (enemy tagging, item tags, player names/info etc.)
/// </summary>
[HarmonyPatch(typeof(GuiManager), "UpdateResolution")]
class OverrideResolutionForGUI
{
    static bool Prefix(GuiManager __instance, float ___m_edgeSafeArea)
    {
        Resolution hmdResolution = VRGlobal.VR_Resolution;

        GuiManager.ScreenRes = hmdResolution;
        GuiManager.ScreenCenter.x = hmdResolution.width * .5f;
        GuiManager.ScreenCenter.y = hmdResolution.height * .5f;

        GuiManager.safeAreaXmin = (float)GuiManager.ScreenRes.width * (1f - ___m_edgeSafeArea);
        GuiManager.safeAreaXmax = (float)GuiManager.ScreenRes.width * ___m_edgeSafeArea;
        GuiManager.safeAreaYmin = (float)GuiManager.ScreenRes.height * (1f - ___m_edgeSafeArea);
        GuiManager.safeAreaYmax = (float)GuiManager.ScreenRes.height * ___m_edgeSafeArea;
        return false;
    }
}
*/


    /*
    [HarmonyPatch(typeof(NavMarkerLayer), "AfterCameraUpdate")]
    class FixNavMarkerPositioningForVR
    {
        static bool Prefix(NavMarkerLayer __instance, Camera ___m_cam, List<NavMarker> ___m_markersActive, float ___m_markerFocusDis, float ___m_markerDisMax, float ___m_markerScaleMin)
        {
            bool ___m_visible = AccessTools.FieldRefAccess<GuiLayer, bool>((GuiLayer)__instance, "m_visible");
            if (___m_visible)
                return false;
            ___m_cam = CameraManager.GetCurrentCamera();
            if (___m_cam == null)
                return false;
            for (int index = 0; index < ___m_markersActive.Count; ++index)
            {
                NavMarker navMarker = ___m_markersActive[index];
                if (navMarker != null && navMarker.m_trackingObj != null)
                {
                    Vector3 s_tempLocalPos = ___m_cam.WorldToScreenPoint(navMarker.m_trackingObj.transform.position);
                    if (GuiManager.GetClampedScreenPos(ref s_tempLocalPos))
                    {
                        float s_tempFloat = (GuiManager.ScreenCenter - (Vector2)s_tempLocalPos).magnitude;
                        navMarker.DisScreenSpaceToCenterRel = s_tempFloat / (float)GuiManager.ScreenRes.width;
                        if (navMarker.DisScreenSpaceToCenterRel <= ___m_markerFocusDis)
                        {
                            if (navMarker.m_currentState != NavMarkerState.InFocus)
                                navMarker.SetState(NavMarkerState.InFocus);
                        }
                        else if (navMarker.m_currentState != NavMarkerState.Visible)
                            navMarker.SetState(NavMarkerState.Visible);
                        float s_tempDist = (navMarker.m_trackingObj.transform.position - ___m_cam.transform.position).magnitude;
                        float s_tempScale = Mathf.Max(1f - Mathf.Min(s_tempDist / ___m_markerDisMax, 1f), ___m_markerScaleMin);
                        navMarker.transform.localScale = navMarker.m_initScale * s_tempScale;
                        navMarker.SetDistance(s_tempDist);
                    }
                    else if (navMarker.m_currentState != NavMarkerState.Inactive)
                        navMarker.SetState(NavMarkerState.Inactive);
                    s_tempLocalPos = GuiManager.ScreenToGUIScaled(NavMarkerLayer.s_tempLocalPos, this.GuiLayerBase.ReferenceResolution, this.GuiLayerBase.m_cellUICanvas.CanvasScale, navMarker.IsVisible, navMarker.gameObject);
                    navMarker.transform.localRotation = Quaternion.Euler(new Vector3(0.0f, 0.0f, 0.0f));
                    navMarker.transform.localPosition = navMarker.m_trackingObj.transform.position;
                }
            }
            return false;
        }
    }
    */



    /*
    /// <summary>
    /// Turns off 2D rendering for all nav markers (enemy tagging, item tags, player names/info etc.)
    /// </summary>
    [HarmonyPatch(typeof(NavMarkerComponent), "SetEnabled")]
    class InjectVRNavMarkers
    {
        static void Postfix(NavMarkerComponent __instance)
        {
            foreach(SpriteRenderer s in __instance.GetComponentsInChildren<SpriteRenderer>())
            {
                foreach(Material m in s.sharedMaterials)
                {
                    m.DisableKeyword("FPS_RENDERING_ALLOWED");
                }
            }
        }
    } */


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
            Matrix4x4 localToWorldMatrix = ___m_holoSpaceTransform.localToWorldMatrix;
            Matrix4x4 worldToLocalMatrix = ___m_holoSpaceTransform.worldToLocalMatrix;
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
