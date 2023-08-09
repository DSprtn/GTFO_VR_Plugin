using System;
using UnhollowerBaseLib;
using BepInEx.IL2CPP.Hook;
using System.Runtime.InteropServices;
using Gear;
using UnityEngine;
using GTFO_VR.Core.VR_Input;
using GTFO_VR.Core;
using GTFO_VR.Events;
using System.Reflection;
using Unity.Jobs.LowLevel.Unsafe;

namespace GTFO_VR.Detours
{

    /// <summary>
    /// Patches the InjectPlayerHudEvents
    /// </summary>
    public static class InjectPlayerHudEventsDetour
    {
        public unsafe static void HookAll()
        {            
            Log.Info("Patching InjectPlayerHudEvents functions...");

            // Couldn't get methodinfo via getMethod(), using getMethods() worked
            Type objType = typeof(GlassLiquidSystem);
            MethodInfo method = null;
            MethodInfo[] info = objType.GetMethods(BindingFlags.Public | BindingFlags.Instance);
            for (int i = 0; i < info.Length; i++)
            {
                if (info[i].Name == "Splat" && method is null)
                {
                    method = info[i];
                }
            }
            
            var tryGetSplatPointer = *(IntPtr*)(IntPtr)UnhollowerUtils
                   .GetIl2CppMethodInfoPointerFieldForGeneratedMethod(method)
                   .GetValue(null);

            FastNativeDetour.CreateAndApply(tryGetSplatPointer,
                OurSplatMethod,
                out OriginalSplatMethod,
                CallingConvention.Cdecl);
        }

        private unsafe static void OurSplatMethod(IntPtr thisPtr, ref ScreenLiquidJob job, ref ScreenLiquidSetting setting)
        {
            OriginalSplatMethod(thisPtr, ref job, ref setting);
            Log.Info(job.setting.ToString());
            PlayerHudEvents.LiquidSplat();
        }

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private unsafe delegate void SplatDelegate(IntPtr thisPtr, ref ScreenLiquidJob job, ref ScreenLiquidSetting setting);

        private static SplatDelegate OriginalSplatMethod;
    }
}
