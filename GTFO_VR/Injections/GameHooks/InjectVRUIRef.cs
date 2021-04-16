using GTFO_VR.Core;
using GTFO_VR.Core.UI;
using HarmonyLib;
using UnityEngine;

namespace GTFO_VR.Injections.GameHooks
{

    [HarmonyPatch(typeof(UI_Pass), nameof(UI_Pass.Awake))]
    internal class InjectVRUIRef
    {
        private static void Prefix(UI_Pass __instance)
        {
            VR_UI_Overlay.UI_ref = __instance;
        }
    }
}
