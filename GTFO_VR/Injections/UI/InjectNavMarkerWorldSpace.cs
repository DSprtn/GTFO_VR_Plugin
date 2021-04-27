using GTFO_VR.Core;
using GTFO_VR.Core.VR_Input;
using GTFO_VR.UI;
using HarmonyLib;
using UnityEngine;

namespace GTFO_VR.Injections.UI
{
    /// <summary>
    /// Calls into our VR library to handle converting the marker to world space
    /// </summary>
    [HarmonyPatch(typeof(NavMarker), nameof(NavMarker.Setup))]
    internal class InjectNavMarkerWorldSpaceSetup
    {
        private static void Postfix(NavMarker __instance)
        {
            VRWorldSpaceUI.PrepareNavMarker(__instance);
            __instance.m_pingScale = __instance.m_initScale * 5.5f;
            __instance.m_pinStartScale = __instance.m_initScale;
        }
    }

    /// <summary>
    /// For some reason we can't do this the easy way like all the other UI, so we just brute force it.
    /// </summary>
    [HarmonyPatch(typeof(NavMarkerComponent), nameof(NavMarkerComponent.Start))]
    internal class InjectNavMarkerWorldSpriteHack
    {
        private static void Postfix(NavMarkerComponent __instance)
        {
            foreach (SpriteRenderer r in __instance.transform.GetComponentsInChildren<SpriteRenderer>())
            {
                r.material.shader = VRAssets.SpriteAlwaysRender;
            }
        }
    }


    /// <summary>
    /// Calls into our VR library to handle positioning, scaling and rotating all nav markers
    /// </summary>
    [HarmonyPatch(typeof(NavMarkerLayer), nameof(NavMarkerLayer.AfterCameraUpdate))]
    internal class InjectNavMarkerWorldSpacePositioning
    {
        private static bool Prefix(NavMarkerLayer __instance)
        {
            UpdateAllNavMarkers(__instance.m_markersActive);
            return false;
        }

        internal static void UpdateAllNavMarkers(Il2CppSystem.Collections.Generic.List<NavMarker> m_markersActive)
        {
            float tempScale = 1f;
            bool inElevator = FocusStateManager.CurrentState.Equals(eFocusState.InElevator);

            foreach (NavMarker n in m_markersActive)
            {
                if (inElevator && n)
                {
                    n.transform.localScale = Vector3.zero;
                    return;
                }

                if (n != null && n.m_trackingObj != null)
                {
                    n.transform.position = n.m_trackingObj.transform.position;

                    float dotToCamera = Vector3.Dot((n.m_trackingObj.transform.position - HMD.GetWorldPosition()).normalized, HMD.GetWorldForward());

                    if (dotToCamera < 0)
                    {
                        n.SetState(NavMarkerState.Inactive);
                    }
                    else
                    {
                        float distanceToCamera = Vector3.Distance(n.m_trackingObj.transform.position, HMD.GetWorldPosition());
                        Vector3 hmdToTrackObj = (n.m_trackingObj.transform.position - HMD.GetWorldPosition()).normalized;
                        Quaternion rotToCamera = Quaternion.LookRotation(hmdToTrackObj.normalized);
                        n.transform.rotation = rotToCamera;

                        if (distanceToCamera > 60)
                        {
                            n.transform.position = HMD.GetWorldPosition() + hmdToTrackObj * 60f;
                        }

                        if (dotToCamera > 0.94f)
                        {
                            if (n.m_currentState != NavMarkerState.InFocus)
                            {
                                n.SetState(NavMarkerState.InFocus);
                            }
                        }
                        else if (n.m_currentState != NavMarkerState.Visible)
                        {
                            n.SetState(NavMarkerState.Visible);
                        }
                        if(n.m_distance != null && n.m_distance.fontSharedMaterial != null)
                        {
                            n.m_distance.fontSharedMaterial.shader = VRAssets.GetTextNoCull();
                        }
                        n.SetDistance(distanceToCamera);

                        // Scale up to camera culling distance
                        // If nav marker is beyond that it will place itself back to 60m away
                        tempScale = 1 + Mathf.Clamp(distanceToCamera / 25f, 0, 2.4f);

                        n.transform.localScale = n.m_initScale * tempScale;
                    }
                }
            }
        }
    }
}