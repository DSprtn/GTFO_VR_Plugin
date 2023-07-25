using System;
using System.Runtime.InteropServices;
using GTFO_VR.Core;
using GTFO_VR.Core.VR_Input;
using Gear;
using System.Collections.Generic;
using GTFO_VR.Core.PlayerBehaviours;
using BepInEx.Unity.IL2CPP.Hook;
using Il2CppInterop.Common;

namespace GTFO_VR.Detours
{
    
    /// <summary>
    /// Patches the hammer so it can only hit things if the player is swinging his controller
    /// </summary>
    public static class HammerAttackCheckDetour
    {
        // Will be garbage collected and cause hard crash unless referenced 
        private static INativeDetour checkForAttackTargetsDetour;

        public unsafe static void HookAll()
        {
            
            if(!VRConfig.configUseControllers.Value)
            {
                Log.Info("Not using motion controllers, skipping hammer attack checks detour...");
                return;
            }
            Log.Info("Creating detours for hammer attack checks...");

            var hammerAttackTargetCheckPointer = *(IntPtr*)(IntPtr)Il2CppInteropUtils
                   .GetIl2CppMethodInfoPointerFieldForGeneratedMethod(typeof(MeleeWeaponFirstPerson).GetMethod(nameof(MeleeWeaponFirstPerson.CheckForAttackTargets)))
                   .GetValue(null);

            checkForAttackTargetsDetour = INativeDetour.CreateAndApply(hammerAttackTargetCheckPointer,
               OurAttackCheck,
                out OriginalHammerMethod);
            
        }

        private unsafe static bool OurAttackCheck(IntPtr thisPtr, IntPtr attackData, float sphereRad, float elapsedTime, out IntPtr hits)
        {
            bool result = OriginalHammerMethod(thisPtr, attackData, sphereRad * VRMeleeWeapon.WeaponHitboxSize, elapsedTime, out hits);

            float velocity = VRMeleeWeapon.Current.VelocityTracker.GetSmoothVelocity();
            if (!VRConfig.configUseOldHammer.Value && velocity < 1.8f)
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
