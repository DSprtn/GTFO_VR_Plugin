using GTFO_VR.UI;
using GTFO_VR.Events;
using HarmonyLib;
using GTFO_VR.Core;
using GTFO_VR.Core.VR_Input;

namespace GTFO_VR.Injections.UI
{
    [HarmonyPatch(typeof(PUI_CommunicationMenu), nameof(PUI_CommunicationMenu.Setup))]
    internal class InjectCommsSetupTrace
    {
        private static void Postfix( PUI_CommunicationMenu __instance )
        {
            VRWorldSpaceUI.SetCommsGUIRef(__instance);
        }
    }

}