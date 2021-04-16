using System;
using UnhollowerBaseLib;
using BepInEx.IL2CPP.Hook;
using System.Runtime.InteropServices;
using GTFO_VR.Core;
using GTFO_VR.Core.VR_Input;

namespace GTFO_VR.Detours
{
    /// <summary>
    /// Patches the terminal input so it also works with the VR keyboard.
    /// </summary>
    public static class TerminalInputDetours
    {

        public static void HookAll()
        {
            Log.Info($"Patching all native functions...");

            FastNativeDetour.CreateAndApply(IL2CPP.il2cpp_resolve_icall("UnityEngine.Input::" + "get_inputString"),
                OurGetInputString, out originalInputStringGetter, CallingConvention.Cdecl);

            FastNativeDetour.CreateAndApply(IL2CPP.il2cpp_resolve_icall("UnityEngine.Input::" + "get_anyKeyDown"),
                OurGetAnyInput, out originalAnyInputDownGetter, CallingConvention.Cdecl);
        }

        private unsafe static IntPtr OurGetInputString()
        {
            string input = IL2CPP.Il2CppStringToManaged(originalInputStringGetter());
            IntPtr vr_input = IL2CPP.ManagedStringToIl2Cpp(VRKeyboard.GetKeyboardInput() + input);
            return vr_input;
        }

        private unsafe static bool OurGetAnyInput()
        {
            bool vr_input = VRKeyboard.GetKeyboardInput() != "";
            return vr_input || originalAnyInputDownGetter();
        }

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private unsafe delegate IntPtr StringGetter();

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private unsafe delegate bool BoolGetter();

        private static StringGetter originalInputStringGetter;

        private static BoolGetter originalAnyInputDownGetter;
       
    }
}
