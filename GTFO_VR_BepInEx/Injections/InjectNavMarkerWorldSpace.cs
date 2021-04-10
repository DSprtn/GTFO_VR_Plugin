using GTFO_VR.Core;
using GTFO_VR.UI;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;

namespace GTFO_VR_BepInEx.Core
{
    /// <summary>
    /// Calls into our VR library to handle converting the marker to world space
    /// </summary>
    [HarmonyPatch(typeof(NavMarker), "Setup")]
    class InjectNavMarkerWorldSpaceSetup
    {
        static void Postfix(NavMarker __instance, ref Vector3 ___m_pingScale, ref Vector3 ___m_pinStartScale)
        {
            GTFO_VR.UI.VRWorldSpaceUI.PrepareNavMarker(__instance);
            ___m_pingScale = __instance.m_initScale * 5.5f;
            ___m_pinStartScale = __instance.m_initScale;
            
        }
    }

    /// <summary>
    /// For some reason we can't do this the easy way like all the other UI, so we just brute force it.
    /// </summary>
    [HarmonyPatch(typeof(NavMarker), "SetDistance")]
    class InjectNavMarkerWorldDistanceHack
    {

        static void Postfix(NavMarker __instance, TextMeshPro ___m_distance)
        {
            if(___m_distance)
            {
                ___m_distance.GetComponent<MeshRenderer>().material.shader = VR_Assets.textAlwaysRender;
            }
        }
    }

    /// <summary>
    /// Calls into our VR library to handle positioning, scaling and rotating all nav markers
    /// </summary>
    [HarmonyPatch(typeof(NavMarkerLayer), "AfterCameraUpdate")]
    class InjectNavMarkerWorldSpacePositioning
    {
        static bool Prefix(NavMarkerLayer __instance, List<NavMarker> ___m_markersActive)
        {
            VRWorldSpaceUI.UpdateAllNavMarkers(___m_markersActive);
            return false;
        }
    }
  
}
