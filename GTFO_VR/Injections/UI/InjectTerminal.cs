using Assets.scripts.KeyboardDefinition;
using GTFO_VR.Core;
using GTFO_VR.Core.UI;
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
    [HarmonyPatch(typeof(LevelGeneration.LG_ComputerTerminal), nameof(LevelGeneration.LG_ComputerTerminal.OnProximityEnter))]
    internal class InjectProximityEnter
    {
        private static void Postfix(LevelGeneration.LG_ComputerTerminal __instance)
        {
            GameObject graphics = __instance.gameObject.transform.Find("Graphics").gameObject;
            if (graphics == null)
            {
                Log.Error("Could not find [Graphics] in terminal!");
                return;
            }

            GameObject terminalCanvas = graphics.transform.Find("text FPS").gameObject;
            if (terminalCanvas == null)
            {
                Log.Error("Could not find [text FPS] canvas in terminal!");
                return;
            }
            GameObject keyboardRoot = new GameObject();
            keyboardRoot.name = "keyboardRoot";

            keyboardRoot.transform.position = terminalCanvas.transform.position;
            keyboardRoot.transform.rotation = terminalCanvas.transform.rotation;
            keyboardRoot.transform.SetParent(__instance.transform);

            KeyboardStyle.colorBrightnessMultiplier = 0.1f;
            TerminalKeyboardInterface.attach(keyboardRoot, terminalCanvas);
        }
    }

    [HarmonyPatch(typeof(LevelGeneration.LG_ComputerTerminal), nameof(LevelGeneration.LG_ComputerTerminal.OnProximityExit))]
    internal class InjectProximityExit
    {
        private static void Postfix(LevelGeneration.LG_ComputerTerminal __instance)
        {
            Transform keyboardRoot = __instance.gameObject.transform.Find("keyboardRoot");
            if (keyboardRoot != null)
            {
                GameObject.Destroy(keyboardRoot.gameObject);
            }        
        }
    }
}
