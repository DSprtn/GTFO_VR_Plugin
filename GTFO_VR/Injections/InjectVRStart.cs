using GTFO_VR.Core;
using GTFO_VR.Core.UI;
using HarmonyLib;
using UnityEngine;


namespace GTFO_VR.Injections
{
    /// <summary>
    /// Entry point for loading and initiating all things VR
    /// </summary>

    [HarmonyPatch(typeof(UI_Pass), nameof(UI_Pass.Awake))]
    class InjectVRStart
    {
        static void Prefix(UI_Pass __instance)
        {
            if (!VR_Global.VR_ENABLED)
            {
                new GameObject("VR_Globals").AddComponent<VR_Global>();
                VR_UI_Overlay.UI_ref = __instance;
            }
        }
    }
}
