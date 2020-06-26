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
    /// <summary>
    /// Unlocks cursor so steamVR desktop can be used freely
    /// </summary>

    [HarmonyPatch(typeof(InteractionGuiLayer),"Setup")]
    class InjectInteractionPromptRef
    {
        static void Postfix(PUI_InteractionPrompt ___m_interactPrompt, PUI_InteractionPrompt ___m_message, InteractionGuiLayer __instance)
        {

            VRWorldSpaceUI.SetRef(___m_message, ___m_interactPrompt);
            VRWorldSpaceUI.interactGUI = __instance;


        }
    
}

}
