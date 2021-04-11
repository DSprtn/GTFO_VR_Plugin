using System;
using UnhollowerBaseLib;
using BepInEx.IL2CPP.Hook;
using System.Runtime.InteropServices;
using Gear;
using UnityEngine;
using GTFO_VR.Core.VR_Input;
using GTFO_VR.Core;

namespace GTFO_VR.Detours
{

    /// <summary>
    /// Patches the bioscanner so the scanning works off of the gun's position instead of the camera's.
    /// </summary>
    public static class BioscannerDetours
    {
        public static Quaternion cachedControllerRotation = Quaternion.identity;
        public static bool inBioScannerFunction;

        public unsafe static void HookAll()
        {
            GTFO_VR_Plugin.log.LogInfo("Creating detours for bioscanner...");

            var tryGetTaggableEnemiesPointer = *(IntPtr*)(IntPtr)UnhollowerUtils
                   .GetIl2CppMethodInfoPointerFieldForGeneratedMethod(typeof(EnemyScanner).GetMethod(nameof(EnemyScanner.TryGetTaggableEnemies)))
                   .GetValue(null);

            FastNativeDetour.CreateAndApply(tryGetTaggableEnemiesPointer,
                OurScannerMethod,
                out OriginalScannerMethod,
                CallingConvention.Cdecl);

            FastNativeDetour.CreateAndApply(
                IL2CPP.il2cpp_resolve_icall("UnityEngine.Transform::" + nameof(Transform.get_rotation_Injected)),
                OurGetRotation, out ourOriginalRotationGetter, CallingConvention.Cdecl);
        }

        private unsafe static void OurGetRotation(IntPtr thisPtr, out Quaternion quat)
        {
            ourOriginalRotationGetter(thisPtr, out quat);
            if (inBioScannerFunction)
            {
                quat = cachedControllerRotation;
            }
        }

        private unsafe static bool OurScannerMethod(IntPtr thisPtr, int maxTags, out IntPtr enemies)
        {
            cachedControllerRotation = Controllers.GetControllerAimRotation();
            inBioScannerFunction = true;
            bool result = OriginalScannerMethod(thisPtr, maxTags, out enemies);
            inBioScannerFunction = false;
            return result;
        }

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private unsafe delegate bool EnemyScannerDelegate(IntPtr thisPtr, int maxTaggable, out IntPtr enemies);

        private static EnemyScannerDelegate OriginalScannerMethod;


        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private unsafe delegate void RotOutDelegate(IntPtr thisPtr, out Quaternion quat);

        private static RotOutDelegate ourOriginalRotationGetter;
    }
}
