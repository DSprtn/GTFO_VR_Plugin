using System;
using UnhollowerBaseLib;
using BepInEx.IL2CPP.Hook;
using System.Runtime.InteropServices;
using System.Reflection;
using GTFO_VR_BepInEx.Core;

namespace GTFO_VR.Core
{
    public static class NativeDetours
    {

        public static void HookAll()
        {
            GTFO_VR_Plugin.log.LogInfo($"Patching all native functions...");

            FastNativeDetour.CreateAndApply(IL2CPP.il2cpp_resolve_icall("UnityEngine.Input::" + "get_inputString"), 
                OurGetInputString, out originalInputStringGetter, CallingConvention.Cdecl);


            FastNativeDetour.CreateAndApply(IL2CPP.il2cpp_resolve_icall("UnityEngine.Input::" + "get_anyKeyDown"), 
                OurGetAnyInput, out originalAnyInputDownGetter, CallingConvention.Cdecl);
        }

        private unsafe static IntPtr OurGetInputString()
        {
            string input = IL2CPP.Il2CppStringToManaged(originalInputStringGetter());
            IntPtr vr_input = IL2CPP.ManagedStringToIl2Cpp(VR_Keyboard.GetKeyboardInput() + input);
            return vr_input;
        }

        private unsafe static bool OurGetAnyInput()
        {
            bool vr_input = VR_Keyboard.GetKeyboardInput() != "";
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
