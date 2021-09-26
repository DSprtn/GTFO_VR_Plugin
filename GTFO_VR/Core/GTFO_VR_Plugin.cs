using BepInEx;
using BepInEx.IL2CPP;
using GTFO_VR.Core.PlayerBehaviours;
using GTFO_VR.Core.UI;
using GTFO_VR.Core.VR_Input;
using GTFO_VR.Detours;
using GTFO_VR.UI;
using HarmonyLib;
using System.Collections.Generic;
using System.Diagnostics;
using UnhollowerRuntimeLib;
using static GTFO_VR.Util.ExtensionMethods;

namespace GTFO_VR.Core
{
    /// <summary>
    /// Main entry point of the mod. Responsible for managing the config and running all patches if the mod is enabled.
    /// </summary>
    [BepInPlugin(GUID, MODNAME, VERSION)]
    public class GTFO_VR_Plugin : BasePlugin
    {
        public const string
            MODNAME = "GTFO_VR_Plugin",
            AUTHOR = "Spartan",
            GUID = "com." + AUTHOR + "." + MODNAME,
            VERSION = "1.0.0.3";


        public static bool DEBUG_ENABLED = false;

        public override void Load()
        {
            Core.Log.Setup(BepInEx.Logging.Logger.CreateLogSource(MODNAME));
            Core.Log.Info($"Loading VR plugin {VERSION}");

            VRConfig.SetupConfig(Config);

            if (SteamVRRunningCheck())
            {
                InjectVR();
            }
            else
            {
                Log.LogWarning("VR launch aborted, VR is disabled or SteamVR is off!");
            }
        }

        private void InjectVR()
        {
            SetupIL2CPPClassInjections();
            TerminalInputDetours.HookAll();
            BioscannerDetours.HookAll();
            HammerAttackCheckDetour.HookAll();

            Harmony harmony = new Harmony("com.github.dsprtn.gtfovr");
            harmony.PatchAll();
        }

        private void SetupIL2CPPClassInjections()
        {
            ClassInjector.RegisterTypeInIl2Cpp<VRSystems>();
            ClassInjector.RegisterTypeInIl2Cpp<VRAssets>();
            ClassInjector.RegisterTypeInIl2Cpp<VRKeyboard>();
            ClassInjector.RegisterTypeInIl2Cpp<VR_UI_Overlay>();
            ClassInjector.RegisterTypeInIl2Cpp<VRWorldSpaceUI>();
            ClassInjector.RegisterTypeInIl2Cpp<Controllers>();
            ClassInjector.RegisterTypeInIl2Cpp<HMD>();
            ClassInjector.RegisterTypeInIl2Cpp<VRRendering>();
            ClassInjector.RegisterTypeInIl2Cpp<CollisionFade>();
            ClassInjector.RegisterTypeInIl2Cpp<LaserPointer>();
            ClassInjector.RegisterTypeInIl2Cpp<PlayerOrigin>();
            ClassInjector.RegisterTypeInIl2Cpp<VRPlayer>();
            ClassInjector.RegisterTypeInIl2Cpp<Haptics>();
            ClassInjector.RegisterTypeInIl2Cpp<BhapticsIntegration>();
            ClassInjector.RegisterTypeInIl2Cpp<BhapticsElevatorSequence>();
            ClassInjector.RegisterTypeInIl2Cpp<Snapturn>();
            ClassInjector.RegisterTypeInIl2Cpp<Watch>();
            ClassInjector.RegisterTypeInIl2Cpp<VRHammer>();
            ClassInjector.RegisterTypeInIl2Cpp<DividedBarShaderController>();
            ClassInjector.RegisterTypeInIl2Cpp<MovementVignette>();
            ClassInjector.RegisterTypeInIl2Cpp<RadialMenu>();
            ClassInjector.RegisterTypeInIl2Cpp<RadialItem>();
            ClassInjector.RegisterTypeInIl2Cpp<WeaponRadialMenu>();
            ClassInjector.RegisterTypeInIl2Cpp<WeaponAmmoHologram>();
        }

        private bool SteamVRRunningCheck()
        {
            if (!VRConfig.configCheckSteamVR.Value)
            {
                return true;
            }
            List<Process> possibleVRProcesses = new List<Process>();
            
            possibleVRProcesses.AddRange(Process.GetProcessesByName("vrserver"));
            possibleVRProcesses.AddRange(Process.GetProcessesByName("vrcompositor"));

            Core.Log.Debug("VR processes found - " + possibleVRProcesses.Count);
            foreach (Process p in possibleVRProcesses)
            {
                Core.Log.Debug(p.ToString());
            }
            return possibleVRProcesses.Count > 0;
        }
    }
}