namespace TagsCloudVisualization.Interfaces
{
    public interface IWordSizeCalculator
    {
        float[] CalculatePointSizes(int[] frequencies);
    }
}