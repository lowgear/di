using System.Collections.Generic;
using System.Drawing;

namespace TagsCloudVisualization.Interfaces
{
    public interface IRectangleLayouter
    {
        PointF[] LayoutRectangles(IEnumerable<SizeF> rectangles);
    }
}