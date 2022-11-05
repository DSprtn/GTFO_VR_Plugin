using GTFO_VR.Core;
using HarmonyLib;
using Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GTFO_VR.Injections.UI
{

    [HarmonyPatch(typeof(GameDataTextLocalizationService), nameof(GameDataTextLocalizationService.GetString))]
    internal class InjectLocalizedVRText
    {
        private static bool Prefix(uint id, ref string __result)
        {
            if(VRConfig.VRTextMappings.ContainsKey(id)) {
                __result = VRConfig.VRTextMappings[id];
                return false;
            }
            return true;
        }
    }
}
