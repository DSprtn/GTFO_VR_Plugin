using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using CellMenu;
using Globals;
using GTFO_VR;
using GTFO_VR.UI;
using HarmonyLib;
using Player;
using UnityEngine;


namespace GTFO_VR_BepInEx.Core
{

    [HarmonyPatch(typeof(InteractionGuiLayer), "Setup")]
    class InjectInteractionPromptRef
    {
        static void Postfix(InteractionGuiLayer __instance)
        {
            VRWorldSpaceUI.SetInteractionPromptRef(__instance.m_message, __instance.m_interactPrompt, __instance);
        }

    }

}
