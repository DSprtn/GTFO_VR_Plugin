using Bhaptics.Tact;
using System.IO;
using System;

namespace GTFO_VR.Core.PlayerBehaviours
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

        public static void RegisterVisorTactKey(HapticPlayer hapticPlayer, string key)
        {
            RegisterArmsTactKey(hapticPlayer, PATTERNS_FOLDER + "visor\\", key);
        }

        private static void RegisterArmsTactKey(HapticPlayer hapticPlayer, string folder, string key)
        {
            string fileName = key.Substring(key.IndexOf("_") + 1);
            string patternFileContent = File.ReadAllText(folder + fileName + ".tact");
            hapticPlayer.RegisterTactFileStr(key, patternFileContent);
        }

        public static float Clamp(float v, float min, float max)
        {
            return Math.Min(Math.Max(v, min), max);
        }
    }
}