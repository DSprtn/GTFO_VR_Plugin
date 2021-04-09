using GTFO_VR.Core;
using GTFO_VR.UI;
using HarmonyLib;
using UnityEngine;

namespace GTFO_VR_BepInEx.Core
{
    /// <summary>
    /// Calls into our VR library to handle converting the marker to world space
    /// </summary>
    [HarmonyPatch(typeof(NavMarker), "Setup")]
    class InjectNavMarkerWorldSpaceSetup
    {
        static void Postfix(NavMarker __instance)
        {
            GTFO_VR.UI.VRWorldSpaceUI.PrepareNavMarker(__instance);
            __instance.m_pingScale = __instance.m_initScale * 5.5f;
            __instance.m_pinStartScale = __instance.m_initScale;
            
        }
    }

    /// <summary>
    /// For some reason we can't do this the easy way like all the other UI, so we just brute force it.
    /// </summary>
    [HarmonyPatch(typeof(NavMarker), "SetDistance")]
    class InjectNavMarkerWorldDistanceHack
    {

        static void Postfix(NavMarker __instance)
        {
            if(__instance.m_distance)
            {
                __instance.m_distance.GetComponent<MeshRenderer>().material.shader = VR_Assets.textAlwaysRender;
            }
        }
    }

    /// <summary>
    /// Calls into our VR library to handle positioning, scaling and rotating all nav markers
    /// </summary>
    [HarmonyPatch(typeof(NavMarkerLayer), "AfterCameraUpdate")]
    class InjectNavMarkerWorldSpacePositioning
    {
        static bool Prefix(NavMarkerLayer __instance)
        {
            VRWorldSpaceUI.UpdateAllNavMarkers(__instance.m_markersActive);
            return false;
        }
    }
  
}
