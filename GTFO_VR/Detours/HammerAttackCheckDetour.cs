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
            bool result = false;

            // The original CheckForAttackTargets() is completely circumvented.
            // The only side effect of the original function not handled here is handled by InjectVRHammerSmackDoors
            if (!VRConfig.configUseOldHammer.Value)
            {
                Il2CppSystem.Collections.Generic.List<MeleeWeaponDamageData> ourHits;
                float velocity = VRMeleeWeapon.Current.m_positionTracker.GetSmoothVelocity();

                // Ensure weapon is moving fast-ish, and that the player is still holding the fire button.
                // Releasing fire triggers the normal pancake attack window for about a second.
                if (velocity > 2f && InputMapper.GetButtonKeyMouseGamepad(InputAction.Fire))
                {
                    if (VRMeleeWeapon.Current.CheckForAttackTarget(out ourHits))
                    {
                        result = true;
                    }
                }
                else
                {
                    ourHits = new Il2CppSystem.Collections.Generic.List<MeleeWeaponDamageData>();
                }

                hits = ourHits.Pointer;
            }
            else
            {
               result = OriginalHammerMethod(thisPtr, attackData, sphereRad * VRMeleeWeapon.WeaponHitboxSize, elapsedTime, out hits);
            }

            return result;
        }

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private unsafe delegate bool AttackCheckDelegate(IntPtr thisPtr, IntPtr attackData, float sphereRad,float elapsedTime, out IntPtr hits);

        private static AttackCheckDelegate OriginalHammerMethod;

    }
    
}
