using System.IO;
using Bhaptics.Tact;

namespace GTFO_VR.Core.PlayerBehaviours.BodyHaptics.Bhaptics
{
    public class BhapticsUtils
    {
        private static readonly string PATTERNS_FOLDER = "BepInEx\\plugins\\bhaptics-patterns\\";

        public static void RegisterVestTactKey(HapticPlayer hapticPlayer, string key)
        {
            RegisterArmsTactKey(hapticPlayer, PATTERNS_FOLDER + "vest\\", key);
        }

        public static void RegisterArmsTactKey(HapticPlayer hapticPlayer, string key)
        {
            RegisterArmsTactKey(hapticPlayer, PATTERNS_FOLDER + "arms\\", key);
        }

        private static void RegisterArmsTactKey(HapticPlayer hapticPlayer, string folder, string key)
        {
            string fileName = key.Substring(key.IndexOf("_") + 1);
            string patternFileContent = File.ReadAllText(folder + fileName + ".tact");
            hapticPlayer.RegisterTactFileStr(key, patternFileContent);
        }
    }
}