using System.Drawing;

namespace TagsCloudVisualization.Interfaces
{
    public interface IRectangleLayouter
    {
        PointF[] LayoutRectangles(SizeF[] rectangles);
    }
}