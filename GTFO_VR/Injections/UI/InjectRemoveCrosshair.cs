﻿using System;
using System.Collections.Generic;
using System.Text;
using GTFO_VR;
using HarmonyLib;
using Player;
using UnityEngine;


namespace GTFO_VR_BepInEx.Core
{
    /// <summary>
    /// Disable crosshair GUI layer, if this is not done the crosshair would be visible in the 2D game view on screen
    /// </summary>
    [HarmonyPatch(typeof(GuiManager), nameof(GuiManager.OnFocusStateChanged))]
    class InjectRemoveCrosshairHit
    {
        static void Postfix()
        {
            GuiManager.CrosshairLayer.SetVisible(false);
        }
    }

    /// <summary>
    /// Disable chargeup coroutines because they throw annoying errors
    /// </summary>
    [HarmonyPatch(typeof(CrosshairGuiLayer), nameof(CrosshairGuiLayer.TriggerChargeUpBlink))]
    class InjectRemoveCrosshairChargeupCoroutineErrors
    {
        static bool Prefix()
        {
            return false;
        }
    }

    /// <summary>
    /// Disable chargeup coroutines because they throw annoying errors
    /// </summary>
    [HarmonyPatch(typeof(CrosshairGuiLayer), nameof(CrosshairGuiLayer.TriggerBlink))]
    class InjectRemoveCrosshairChargeupCoroutineErrorTwo
    {
        static bool Prefix()
        {
            return false;
        }
    }

}