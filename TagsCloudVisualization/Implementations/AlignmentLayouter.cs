using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using MoreLinq;
using TagsCloudVisualization.Extensions;
using TagsCloudVisualization.Interfaces;

namespace TagsCloudVisualization.Implementations
{
    public class AlignmentLayouter : IRectangleLayouter
    {
        private static readonly PointF Center = PointF.Empty;

        private static PointF PutNextRectangle(SizeF rectangleSize, IList<RectangleF> existingRectangles, ISet<PointF> existingVertices)
        {
            var res = new RectangleF(PointF.Empty, rectangleSize);
            if (existingRectangles.Count == 0)
            {
                var halfSize = new SizeF(rectangleSize.Width / 2, rectangleSize.Height / 2);
                res.Location = PointF.Subtract(Center, halfSize);
            }
            else
                res.Location = ChooseLocationForRectangle(rectangleSize, existingRectangles, existingVertices);

            existingRectangles.Add(res);
            foreach (var vertex in res.Vertices())
                existingVertices.Add(vertex);

            return res.Location;
        }

        private static PointF ChooseLocationForRectangle(SizeF rectangleSize, IList<RectangleF> existingRectangles, IEnumerable<PointF> existingVertices)
        {
            var rectangle = new RectangleF(PointF.Empty, rectangleSize);

            return existingVertices
                .SelectMany(point => rectangle.LocationsIfHadVertex(point))
                .MinBy(p =>
                {
                    rectangle.Location = p;
                    if (existingRectangles.Any(r => r.IntersectsWith(rectangle)))
                        return double.MaxValue;
                    return rectangle.MaxDistanceTo(Center);
                });
        }

        public PointF[] LayoutRectangles(SizeF[] sizes)
        {
            IList<RectangleF> existingRectangles = new List<RectangleF>();
            ISet<PointF> existingVertices = new HashSet<PointF>();
            return sizes.Select(size => PutNextRectangle(size, existingRectangles, existingVertices)).ToArray();
        }
    }
}
