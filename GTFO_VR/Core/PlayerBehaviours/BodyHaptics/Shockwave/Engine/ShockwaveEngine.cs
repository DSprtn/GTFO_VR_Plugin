using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace GTFO_VR.Core.PlayerBehaviours.BodyHaptics.Shockwave.Engine
{
    public class ShockwaveEngine
    {
        #region Mirror
        private static readonly int[,] VerticalMirror =
        {
            // Arms front
            { 54, 46 },
            { 52, 44 },
            { 50, 42 },
            { 48, 40 },
            // Arms back
            { 47, 55 },
            { 45, 53 },
            { 43, 51 },
            { 41, 49 },
            // Legs front
            { 70, 62 },
            { 68, 60 },
            { 66, 58 },
            { 64, 56 },
            // Legs back
            { 63, 71 },
            { 61, 69 },
            { 59, 67 },
            { 57, 65 },
            // Body front
            { 6, 1 },
            { 7, 0 },
            { 14, 9 },
            { 15, 8 },
            { 22, 17 },
            { 23, 16 },
            { 30, 25 },
            { 31, 24 },
            { 38, 33 },
            { 39, 32 },
            // Body back
            { 2, 5 },
            { 3, 4 },
            { 10, 13 },
            { 11, 12 },
            { 18, 21 },
            { 19, 20 },
            { 26, 29 },
            { 27, 28 },
            { 34, 37 },
            { 35, 36 },
        };
        #endregion

        public static void PlayPattern(HapticGroupPattern pattern, Func<bool> mustInterrupt = null)
        {
            Task.Run(() => PlayPatternFunc(pattern, mustInterrupt));
        }

        public static async Task PlayPatternFunc(HapticGroupPattern pattern, Func<bool> mustInterrupt = null)
        {
            int delay = pattern.delay;

            foreach (HapticGroupInfo hapticGroupInfo in pattern.groupInfos)
            {
                ShockwaveManager.Instance?.SendHapticGroup(hapticGroupInfo.group, hapticGroupInfo.intensity, delay);
                await Task.Delay(delay);

                if (mustInterrupt != null && mustInterrupt())
                {
                    break;
                }
            }
        }

        public static void PlayPattern(HapticIndexPattern pattern, Func<bool> mustInterrupt = null)
        {
            Task.Run(() => PlayPatternFunc(pattern, mustInterrupt));
        }

        public static async Task PlayPatternFunc(HapticIndexPattern pattern, Func<bool> mustInterrupt = null)
        {
            int delay = pattern.delay;

            foreach (List<HapticIndex> patternIndexes in pattern.indices)
            {
                var indexes = new List<int>();
                var intensities = new List<float>();
                foreach (HapticIndex hapticIndex in patternIndexes)
                {
                    indexes.Add(hapticIndex.index);
                    intensities.Add(hapticIndex.intensity);
                }
                ShockwaveManager.Instance?.sendHapticsPulse(indexes.ToArray(), intensities.ToArray(), delay);
                await Task.Delay(delay);

                if (mustInterrupt != null && mustInterrupt())
                {
                    break;
                }
            }
        }

        public static int[] GetPatternMirror(int[] basePattern)
        {
            int[] pattern = new int[basePattern.Length];

            for (int i = 0; i < basePattern.Length; i++)
            {
                bool foundMirror = false;
                for (int j = 0; j < VerticalMirror.GetLength(0); j++)
                {
                    if (VerticalMirror[j, 0] == basePattern[i])
                    {
                        pattern[i] = VerticalMirror[j, 1];
                        foundMirror = true;
                        break;
                    }
                    else if (VerticalMirror[j, 1] == basePattern[i])
                    {
                        pattern[i] = VerticalMirror[j, 0];
                        foundMirror = true;
                        break;
                    }
                }

                Debug.Assert(foundMirror, $"Could not find mirror for pattern index {basePattern[i]}");
            }

            return pattern;
        }

        public static HapticIndexPattern GetPatternMirror(HapticIndexPattern basePattern)
        {
            HapticIndexPattern pattern = new HapticIndexPattern(basePattern);

            for (int i = 0; i < pattern.indices.Count; i++)
            {
                List<int> patternIndices = new List<int>();
                foreach (HapticIndex hapticIndex in pattern.indices[i])
                {
                    patternIndices.Add(hapticIndex.index);
                }

                int[] mirror = GetPatternMirror(patternIndices.ToArray());
                for (int j = 0; j < patternIndices.Count; j++)
                {
                    pattern.indices[i][j] = new HapticIndex(mirror[j], pattern.indices[i][j].intensity);
                }
            }

            return pattern;
        }

        public static bool IsActive()
        {
            return VRConfig.configUseShockwave.Value && ShockwaveManager.Instance.Ready;
        }
    }
}