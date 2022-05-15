﻿using Assets.scripts.KeyboardDefinition;
using GTFO_VR.Core;
using GTFO_VR.Core.PlayerBehaviours;
using GTFO_VR.Core.UI;
using GTFO_VR.Core.VR_Input;
using GTFO_VR.Events;
using GTFO_VR.UI.CANVAS;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace GTFO_VR.Injections.UI
{
    [HarmonyPatch(typeof(LevelGeneration.LG_ComputerTerminal), nameof(LevelGeneration.LG_ComputerTerminal.EnterFPSView))]
    internal class InjectProximityExit
    {
        private static void Postfix(LevelGeneration.LG_ComputerTerminal __instance)
        {
            KeyboardStyle.colorBrightnessMultiplier = 0.1f;

            GameObject terminalCanvas = __instance.m_text.gameObject;

            TerminalKeyboardInterface.create(terminalCanvas);
            Controllers.setupCanvasPointers();
            VRPlayer.hideWielded(true);
        }
    }

    [HarmonyPatch(typeof(LevelGeneration.LG_ComputerTerminal), nameof(LevelGeneration.LG_ComputerTerminal.ExitFPSView))]
    internal class InjectProximityExit2
    {
        private static void Postfix(LevelGeneration.LG_ComputerTerminal __instance)
        {
            VRPlayer.hideWielded(false);

            GameObject keyboardRoot = GameObject.Find("keyboardRoot");
            if (keyboardRoot != null)
            {
                GameObject.Destroy(keyboardRoot);
            }

            Controllers.removeCanvasPointers();

        }
    }
}
