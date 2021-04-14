using GTFO_VR.UI;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GTFO_VR.Injections
{

    /// <summary>
    /// Get references to UI components we want to put into world space
    /// </summary>
    [HarmonyPatch(typeof(PlayerGuiLayer), nameof(PlayerGuiLayer.Setup))]
    class InjectPlayerGUIRef
    {
        static void Postfix(PlayerGuiLayer __instance)
        {
            VRWorldSpaceUI.SetPlayerGUIRef(__instance, __instance.m_compass, __instance.m_wardenIntel);
            __instance.m_compass.transform.localScale *= .75f;
        }
    }
}
