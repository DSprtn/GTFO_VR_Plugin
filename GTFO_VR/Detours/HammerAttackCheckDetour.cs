using System;
using UnhollowerBaseLib;
using BepInEx.IL2CPP.Hook;
using System.Runtime.InteropServices;
using GTFO_VR.Core;
using GTFO_VR.Core.VR_Input;
using Gear;
using System.Collections.Generic;
using GTFO_VR.Core.PlayerBehaviours;

namespace GTFO_VR.Detours
{
    /// <summary>
    /// Patches the hammer so it can only hit things if the player is swinging his controller
    /// </summary>
    public static class HammerAttackCheckDetour
    {

        public unsafe static void HookAll()
        {
            Log.Info("Creating detours for hammer attack checks...");

            var hammerAttackTargetCheckPointer = *(IntPtr*)(IntPtr)UnhollowerUtils
                   .GetIl2CppMethodInfoPointerFieldForGeneratedMethod(typeof(MeleeWeaponFirstPerson).GetMethod(nameof(MeleeWeaponFirstPerson.CheckForAttackTargets)))
                   .GetValue(null);

            FastNativeDetour.CreateAndApply(hammerAttackTargetCheckPointer,
                OurAttackCheck,
                out OriginalHammerMethod,
                CallingConvention.Cdecl);
        }

        private unsafe static bool OurAttackCheck(IntPtr thisPtr, IntPtr attackData, float sphereRad, float elapsedTime, out IntPtr hits)
        {
            bool result = OriginalHammerMethod(thisPtr, attackData, sphereRad * VRMeleeWeapon.WeaponHitDetectionSphereSize, elapsedTime, out hits);

            if ((!VRConfig.configUseOldHammer.Value || !VRConfig.configUseControllers.Value) && Controllers.mainControllerPose.GetVelocity().magnitude < 0.4f)
            {
                return false;
            }
            return result;
        }

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private unsafe delegate bool AttackCheckDelegate(IntPtr thisPtr, IntPtr attackData, float sphereRad,float elapsedTime, out IntPtr hits);

        private static AttackCheckDelegate OriginalHammerMethod;

    }
}
