namespace TagsCloudVisualization.Interfaces
{
    public interface IWordSizeCalculator
    {
        float[] CalculateEmSizes(int[] frequencies);
    }
}