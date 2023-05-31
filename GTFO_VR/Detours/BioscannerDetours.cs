using System;
using System.Runtime.InteropServices;
using Gear;
using UnityEngine;
using GTFO_VR.Core.VR_Input;
using GTFO_VR.Core;
using Il2CppInterop.Runtime;
using BepInEx.Unity.IL2CPP.Hook;
using Il2CppInterop.Common;

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
            Log.Info("Patching bioscanner functions...");
            
            var tryGetTaggableEnemiesPointer = *(IntPtr*)(IntPtr)Il2CppInteropUtils
                   .GetIl2CppMethodInfoPointerFieldForGeneratedMethod(typeof(EnemyScanner).GetMethod(nameof(EnemyScanner.TryGetTaggableEnemies)))
                   .GetValue(null);

            INativeDetour.CreateAndApply(tryGetTaggableEnemiesPointer,
                OurScannerMethod,
                out OriginalScannerMethod);

            INativeDetour.CreateAndApply(
                IL2CPP.il2cpp_resolve_icall("UnityEngine.Transform::" + nameof(Transform.get_rotation_Injected)),
                OurGetRotation, out ourOriginalRotationGetter);
        }

        private unsafe static void OurGetRotation(IntPtr thisPtr, out Quaternion quat)
        {
            ourOriginalRotationGetter(thisPtr, out quat);
            if (inBioScannerFunction)
            {
                quat = cachedControllerRotation;
            }
        }

        private unsafe static bool OurScannerMethod(IntPtr thisPtr, int maxTags, bool onAim, out IntPtr enemies)
        {
            cachedControllerRotation = Controllers.GetControllerAimRotation();
            inBioScannerFunction = true;
            // Always behave as if aiming or we will tag only single enemies
            bool result = OriginalScannerMethod(thisPtr, maxTags, false, out enemies);
            inBioScannerFunction = false;
            return result;
        }

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private unsafe delegate bool EnemyScannerDelegate(IntPtr thisPtr, int maxTaggable, bool onAim, out IntPtr enemies);

        private static EnemyScannerDelegate OriginalScannerMethod;


        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private unsafe delegate void RotOutDelegate(IntPtr thisPtr, out Quaternion quat);

        private static RotOutDelegate ourOriginalRotationGetter;
    }
}
