using System;
using System.Runtime.InteropServices;
using Gear;
using UnityEngine;
using GTFO_VR.Core.VR_Input;
using GTFO_VR.Core;
using GTFO_VR.Events;
using System.Reflection;
using Unity.Jobs.LowLevel.Unsafe;
using System.Linq;
using BepInEx.Unity.IL2CPP.Hook;
using Il2CppInterop.Common;

namespace GTFO_VR.Detours
{

    /// <summary>
    /// Patches the InjectPlayerHudEvents
    /// </summary>
    public static class InjectPlayerHudEventsDetour
    {
        private static ScreenLiquidSettingName[] splatTypes = { 
            ScreenLiquidSettingName.enemyBlood_BigBloodBomb,
            ScreenLiquidSettingName.enemyBlood_SmallRandomStreak,
            ScreenLiquidSettingName.enemyBlood_Squirt,
            ScreenLiquidSettingName.spitterJizz,
            ScreenLiquidSettingName.shooterGoo,
            ScreenLiquidSettingName.anemoneGoo
        };

        private static INativeDetour checkForOurSplatMethod;

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
            
            var tryGetSplatPointer = *(IntPtr*)(IntPtr)Il2CppInteropUtils
                   .GetIl2CppMethodInfoPointerFieldForGeneratedMethod(method)
                   .GetValue(null);

            checkForOurSplatMethod = INativeDetour.CreateAndApply(tryGetSplatPointer,
                OurSplatMethod,
                out OriginalSplatMethod);
        }

        private unsafe static void OurSplatMethod(IntPtr thisPtr, ref ScreenLiquidJob job, ref ScreenLiquidSetting setting)
        {
            OriginalSplatMethod(thisPtr, ref job, ref setting);
            if (splatTypes.Contains(job.setting))
            {
                PlayerHudEvents.LiquidSplat();
            }
        }

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private unsafe delegate void SplatDelegate(IntPtr thisPtr, ref ScreenLiquidJob job, ref ScreenLiquidSetting setting);

        private static SplatDelegate OriginalSplatMethod;
    }
}
