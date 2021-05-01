using Enemies;
using GTFO_VR.Core;
using GTFO_VR.Core.VR_Input;
using HarmonyLib;

namespace GTFO_VR.Injections.UI
{
    [HarmonyPatch(typeof(InputMapper), nameof(InputMapper.GetBindingName))]
    internal class InjectInteractionPromptActionNames
    {
        private static bool Prefix(InputAction action, ref string __result)
        {
            string res = "";
            if (SteamVR_InputHandler.TryGetActionNameFromInputAction(action, ref res))
            {
                Log.Debug($"Got action name from SteamVR_Input - {res}");
                __result = res;
                return false;
            }
            return true;
        }
    }
}