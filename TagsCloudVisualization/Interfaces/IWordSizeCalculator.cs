using System.Collections.Generic;

namespace TagsCloudVisualization.Interfaces
{
    public interface IWordSizeCalculator
    {
        float[] CalculatePointSizes(IReadOnlyCollection<int> frequencies);
    }
}