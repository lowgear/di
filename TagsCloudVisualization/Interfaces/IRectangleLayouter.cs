using System.Collections.Generic;
using System.Drawing;

namespace TagsCloudVisualization.Interfaces
{
    public interface IRectangleLayouter
    {
        IEnumerable<PointF> LayoutRectangles(IEnumerable<SizeF> rectangles);
    }
}