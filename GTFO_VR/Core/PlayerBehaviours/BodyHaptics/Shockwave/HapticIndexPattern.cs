using System.Collections.Generic;

namespace GTFO_VR.Core.PlayerBehaviours.BodyHaptics.Shockwave
{
    public class HapticIndexPattern
    {
        public List<List<HapticIndex>> indices;
        public int delay;

        public HapticIndexPattern(int[,] indices, float intensity, int delay)
        {
            this.indices = new List<List<HapticIndex>>();
            this.delay = delay;

            for (int x = 0; x < indices.GetLength(0); x += 1)
            {
                var hapticIndices = new List<HapticIndex>();
                this.indices.Add(hapticIndices);
                for (int y = 0; y < indices.GetLength(1); y += 1)
                {
                    hapticIndices.Add(new HapticIndex(indices[x, y], intensity));
                }
            }
        }

        public HapticIndexPattern(int[] indices, float intensity, int delay)
        {
            this.indices = new List<List<HapticIndex>>();
            this.delay = delay;

            foreach (int index in indices)
            {
                var hapticIndices = new List<HapticIndex>();
                this.indices.Add(hapticIndices);
                hapticIndices.Add(new HapticIndex(index, intensity));
            }
        }
    }
}