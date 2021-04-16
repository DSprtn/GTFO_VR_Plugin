using GTFO_VR.UI;
using HarmonyLib;

namespace GTFO_VR.Injections.GameHooks
{
    /// <summary>
    /// Get references for
    /// </summary>
    [HarmonyPatch(typeof(InteractionGuiLayer), nameof(InteractionGuiLayer.Setup))]
    internal class InjectInteractionPromptRef
    {
        private static void Postfix(InteractionGuiLayer __instance)
        {
            VRWorldSpaceUI.SetInteractionPromptRef(__instance.m_message, __instance.m_interactPrompt, __instance);
        }
    }
}