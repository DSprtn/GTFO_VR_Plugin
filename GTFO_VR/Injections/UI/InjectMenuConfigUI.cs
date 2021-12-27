using CellMenu;
using GTFO_VR.Core;
using HarmonyLib;
using Il2CppSystem.Collections.Generic;
using System;
using TMPro;
using UnityEngine;

namespace GTFO_VR.Injections.UI
{
    
    [HarmonyPatch(typeof(CM_PageSettings), nameof(CM_PageSettings.ToggleValue))]
    internal class InjectSettingListenBoolChange
    {
        private static void Postfix(CM_PageSettings __instance, eCellSettingID setting)
        {
            VRConfig.ConfigChanged(setting, __instance.GetToggleValue(setting));
        }
    }

    [HarmonyPatch(typeof(CM_PageSettings), nameof(CM_PageSettings.SetFloatValue))]
    internal class InjectSettingListenFloatChange
    {
        private static void Postfix(CM_PageSettings __instance, eCellSettingID setting, float value)
        {
            VRConfig.ConfigChanged(setting, value);
        }
    }

    [HarmonyPatch(typeof(CM_PageSettings), nameof(CM_PageSettings.SetStringValue))]
    internal class InjectSettingListenStringChange
    {
        private static void Postfix(CM_PageSettings __instance, eCellSettingID setting, string value)
        {
            VRConfig.ConfigChanged(setting, value);
        }
    }

    [HarmonyPatch(typeof(CM_PageSettings), nameof(CM_PageSettings.SetIntValue))]
    internal class InjectSettingListenIntChange
    {
        private static void Postfix(CM_PageSettings __instance, eCellSettingID setting, int value)
        {
            VRConfig.ConfigChanged(setting, value);
        }
    }

    /// <summary>
    /// Inject hacky string drop down menu's
    /// </summary>
    [HarmonyPatch(typeof(CM_SettingsStringArrayDropdownButton), nameof(CM_SettingsStringArrayDropdownButton.Setup))]
    internal class InjectStringDropdownValueSetup
    {
        private static void Postfix(CM_SettingsStringArrayDropdownButton __instance, eCellSettingID id)
        {
            var stringValues = VRConfig.TryGetStringDropdownValues(id);

            if(stringValues != null) {
                __instance.m_values = stringValues;

                __instance.UpdateValueWithSelected();
            }
        }
    }

    /// <summary>
    /// Add config menu for VR stuff
    /// </summary>
    [HarmonyPatch(typeof(CM_PageSettings), nameof(CM_PageSettings.Setup))]
    internal class InjectMenuConfigUI
    {
        private static void Postfix(CM_PageSettings __instance)
        {
            int VR_SETTINGS_MENU_ID = 1001;
            const int VR_SETTINGS_TITLE_TEXT_ID = 10000001;

            List<iSettingsFieldData> pageSettings = VRConfig.InjectConfigIntoGame();
            VRConfig.VRTextMappings.Add(VR_SETTINGS_TITLE_TEXT_ID, "VR Settings");
           __instance.CreateSubmenu(VR_SETTINGS_TITLE_TEXT_ID, (eSettingsSubMenuId)VR_SETTINGS_MENU_ID, pageSettings, false, false);
        }
    }

}
