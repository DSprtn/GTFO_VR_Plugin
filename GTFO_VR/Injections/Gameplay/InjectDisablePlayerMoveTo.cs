using HarmonyLib;
using Player;

namespace GTFO_VR.Injections.Gameplay
{
    /// <summary>
    /// Game will attempt to move player closer to terminal, but uses camera forward and sends you sliding off to the side. Disable when in terminal.
    /// </summary>
    ///
    [HarmonyPatch(typeof(PlayerCharacterController), nameof(PlayerCharacterController.MoveTo))]
    internal class InjectExternalForce
    {
        private static bool Prefix(PlayerCharacterController __instance)
        {

            if (__instance.m_owner.IsLocallyOwned && FocusStateManager.CurrentState == eFocusState.ComputerTerminal)
            {
                return false;
            }
            return true;
        }
    }
}