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

    [HarmonyPatch(typeof(PUI_CommunicationMenu), nameof(PUI_CommunicationMenu.Update))]
    internal class InjectCommsSelectAction
    {
        private static bool Prefix(PUI_CommunicationMenu __instance)
        {
            // Check input and perform the action, like the original update() should be doing.
            // Must happen in Prefix(), as update() will change game state and cause both toggle and select actions to trigger in same frame.
            if (SteamVR_InputHandler.GetActionDown(InputAction.SelectCommunicationMenu))
            {
                if (__instance.m_active)
                {
                    // MoveNext refers to the next node i.e. the child node, so it's actually "select highlighted".
                    VRWorldSpaceUI.comms.MoveNext(__instance.m_highlightedButtonIndex);

                    // Skip the original update function so this doesn't get triggered twice if the base game ever fixes their inputs.
                    return false;
                }
            }

            return true;
        }
    }

}