using System;
using System.Linq;
using TagsCloudVisualization.Interfaces;

namespace TagsCloudVisualization.Implementations
{
    public class SizeCalculater : IWordSizeCalculator
    {
        private const float MinEmSize = 10;

        public float[] CalculateEmSizes(int[] frequencies)
        {
            return frequencies.Select(i => (float)Math.Max(i * 6, MinEmSize)).ToArray();
        }
    }
}