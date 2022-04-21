using GTFO_VR.Core;
using HarmonyLib;
using Il2CppSystem.Collections.Generic;
using System;
using UnhollowerBaseLib;
using UnhollowerRuntimeLib;
using UnityEngine;
using UnityEngine.Rendering;

namespace GTFO_VR.Injections.GameHooks
{
    /// <summary>
    /// EarlyTransparentRenderer objects are duplicate-rendered an increasing number of times for every cull/uncull. This makes it render normally at the cost of not rendering through transparent objects.
    /// </summary>
    [HarmonyPatch(typeof(EarlyTransparentRenderer), nameof(EarlyTransparentRenderer.OnWillRenderObject))]
    internal class InjectEarlyTransparentRendererFix
    {
        private static bool Prefix(EarlyTransparentRenderer __instance)
        {
            // The role of EarlyTransparentRenderer is to make certain transparent objects visible when viewed through other transparent objects ( glass ).
            //
            // OnWillRenderObject() adds this instance, if enabled, to the static VisibleInstances HashSet, which is processed in ClusteredRenderer.OnPreRender().
            // That calls InjectDrawCommand() which hides the MeshRendere on the parent GameObject while also submitting it to ClusteredRendering.m_earlyTransparentCmd.
            // m_earlyTransparentCmd is added to the camera command buffer in ClusteredRendering.SetupDeferredCmd().
            // ClusteredRendering.OnPostRender() calls Restore() to re-enable the MeshRenderer disabled earlier, and we've come full circle.
            //
            // In VR, OnWillRenderObject() will be called multiple times for the same instance, increasing for every time the object is culled and unculled. Something is
            // rendering it multiple times, tanking performance. The actual problem appears to be toggling the MeshRenderer. If we enable m_drawTwice, the only difference
            // is that the MeshRenderer will not be disabled ( Sentry laser beam does this ) and the problem does not occur. However, all the problematic objects use the
            // GTFO/TankGlass shader, which is very expensive, and rendering it twice also makes affected objects less transparent than they should be.
            // If we skip OnWillRenderObject() completely, it will simply render once, as a normal object, with no performance issues.
            // The downside to this is that it will not render when viewed through other transparent surfaces ( glass ). This isn't somethin that happens very often.
            if (VRConfig.configEarlyTransparentRendererFix.Value)
            {
                // Anything that already has m_drawTwice enabled will not cause problems, so skip them as to not needlessly break things.
                // This probably only applies to rain effects in the elevator and the sentry laser beam.
                return __instance.m_drawTwice;
            }

            return true;
        }
    }
    
}
