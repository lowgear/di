using System.Drawing;

namespace TagsCloudVisualization.Interfaces
{
    public interface IMarginCalculator
    {
        RectangleF CalculateBounds(RectangleF rectangle);
    }
}