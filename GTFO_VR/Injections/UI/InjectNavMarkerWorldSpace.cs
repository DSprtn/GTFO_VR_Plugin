using GTFO_VR.Core;
using GTFO_VR.Core.VR_Input;
using GTFO_VR.UI;
using HarmonyLib;
using System.Collections.Generic;
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
    internal class InjectNavMarkerMaterialTweak
    {
        private static void Postfix(NavMarkerComponent __instance)
        {
            foreach (SpriteRenderer r in __instance.transform.GetComponentsInChildren<SpriteRenderer>())
            {
                if ( r.sharedMaterial != null )
                {
                    r.sharedMaterial.shader = VRAssets.SpriteAlwaysRender;
                    r.sharedMaterial.renderQueue = 4001;
                }
            }

            if ( __instance.m_text != null && __instance.m_text.fontSharedMaterial != null)
            {
                __instance.m_text.fontSharedMaterial.shader = VRAssets.GetTextNoCull();
                __instance.m_text.fontSharedMaterial.renderQueue = 4001;
            }
        }
    }

    /// <summary>
    /// Get a reference to every marker that is created so we can iterate through them without the performancei issues of accessing NavMarkerLayer.m_markersActive.
    /// Note that this also includes Locator markers, which we (probably?) don't want to touch. Those are removed in a patch below.
    /// </summary>
    [HarmonyPatch(typeof(NavMarkerLayer), nameof(NavMarkerLayer.PrepareMarker))]
    internal class InjectNavMarkerPrepare
    {
        private static void Postfix(NavMarker __result)
        {
            InjectNavMarkerWorldSpacePositioning.Markers.Add(__result);
        }
    }

    /// <summary>
    /// Also remove marker from our internal list. As of writing these are always null, so this effectively just clears any null elements.
    /// NavMarkerLayer.m_markersActive will continue growing forever, filling up with null elements.
    /// </summary>
    [HarmonyPatch(typeof(NavMarkerLayer), nameof(NavMarkerLayer.RemoveMarker))]
    internal class InjectNavMarkerRemove
    {
        private static void Prefix(NavMarker marker)
        {
            InjectNavMarkerWorldSpacePositioning.Markers.Remove(marker);

            // As of writing the game is passing null for most (all?) of these, so do a quick sweep for nulls.
            InjectNavMarkerWorldSpacePositioning.Markers.RemoveWhere(mkr => mkr == null);
        }
    }

    /// <summary>
    /// Same as RemoveMarker(), but looks up the marker by its m_trackingObj. Probably not used.
    /// </summary>
    [HarmonyPatch(typeof(NavMarkerLayer), nameof(NavMarkerLayer.RemoveMarkerForGO))]
    internal class InjectNavMarkerRemoveForGO
    {
        private static void Prefix(GameObject go)
        {
            NavMarker removeMe = null;

            foreach (NavMarker marker in InjectNavMarkerWorldSpacePositioning.Markers)
            {
                if ( marker != null && marker.m_trackingObj == go)
                {
                    removeMe = marker;
                    break;
                }
            }

            InjectNavMarkerWorldSpacePositioning.Markers.Remove(removeMe);
        }
    }

    /// <summary>
    /// Locator markers are also created in PrepareMarker, but we want to leave them alone. Remove from our list here.
    /// </summary>
    [HarmonyPatch(typeof(NavMarkerLayer), nameof(NavMarkerLayer.PlaceLocatorMarker))]
    internal class InjectNavMarkerLocatorMarkerSkip
    {
        private static void Postfix(NavMarker __result)
        {
            InjectNavMarkerWorldSpacePositioning.Markers.Remove(__result);
        }
    }


    /// <summary>
    /// Calls into our VR library to handle positioning, scaling and rotating all nav markers
    /// </summary>
    [HarmonyPatch(typeof(NavMarkerLayer), nameof(NavMarkerLayer.AfterCameraUpdate))]
    internal class InjectNavMarkerWorldSpacePositioning
    {
        public static HashSet<NavMarker> Markers = new HashSet<NavMarker>();

        private static bool Prefix(NavMarkerLayer __instance)
        {
            UpdateAllNavMarkers(Markers);
            return false;
        }

        internal static void UpdateAllNavMarkers(HashSet<NavMarker> m_markersActive)
        {
            bool inElevator = FocusStateManager.CurrentState.Equals(eFocusState.InElevator);

            Vector3 hmdPos = HMD.GetWorldPosition();
            Vector3 hmdFwd = HMD.GetWorldForward();

            foreach (NavMarker n in m_markersActive)
            {
                if (inElevator && n)
                {
                    n.transform.localScale = Vector3.zero;
                    return;
                }

                if (n != null && n.IsVisible && n.m_trackingObj != null)
                {
                    Vector3 trackingObjPos = n.m_trackingObj.transform.position;
                    n.transform.position = trackingObjPos;

                    float dotToCamera = Vector3.Dot((trackingObjPos - hmdPos).normalized, hmdFwd);

                    if (dotToCamera < 0)
                    {
                        n.SetState(NavMarkerState.Inactive);
                    }
                    else
                    {
                        float distanceToCamera = Vector3.Distance(trackingObjPos, hmdPos);
                        Vector3 hmdToTrackObj = (trackingObjPos - hmdPos).normalized;
                        Quaternion rotToCamera = Quaternion.LookRotation(hmdToTrackObj.normalized);
                        n.transform.rotation = rotToCamera;

                        if (distanceToCamera > 60)
                        {
                            n.transform.position = hmdPos + hmdToTrackObj * 60f;
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

                        n.SetDistance(distanceToCamera);

                        n.UpdateTrackingDimension();
                        n.UpdateEnabledPerDimensionState();

                        // Scale up to camera culling distance
                        // If nav marker is beyond that it will place itself back to 60m away
                        float tempScale = 1 + Mathf.Clamp(distanceToCamera / 25f, 0, 2.4f);

                        n.transform.localScale = n.m_initScale * tempScale;
                    }
                }
            }
        }
    }
}