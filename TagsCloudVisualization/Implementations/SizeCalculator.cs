using System.Linq;
using TagsCloudVisualization.Interfaces;

namespace TagsCloudVisualization.Implementations
{
    public class SizeCalculator : IWordSizeCalculator
    {
        private readonly float minPointSize;

        public SizeCalculator(float minPointSize)
        {
            this.minPointSize = minPointSize;
        }

        public float[] CalculatePointSizes(int[] frequencies)
        {
            var minFrequency = frequencies.Min();
            return frequencies.Select(i => i * minPointSize / minFrequency).ToArray();
        }
    }
}