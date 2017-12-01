using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using MoreLinq;

namespace TagsCloudVisualization.Extensions
{
    public static class RectangleFExtensions
    {
        public static IEnumerable<PointF> Vertices(this RectangleF rectangle)
        {
            return new[] {rectangle.Left, rectangle.Right}
                .Cartesian(new[] {rectangle.Top, rectangle.Bottom},
                    (x, y) => new PointF(x, y));
        }

        public static double Area(this Rectangle rectangle)
        {
            return rectangle.Height * rectangle.Width;
        }

        public static IEnumerable<PointF> LocationsIfHadVertex(this RectangleF rectangle, PointF p)
        {
            yield return p;
            yield return PointF.Subtract(p, rectangle.Size);
            yield return new PointF(p.X, p.Y - rectangle.Size.Height);
            yield return new PointF(p.X - rectangle.Size.Width, p.Y);
        }

        public static double MaxDistanceTo(this RectangleF rectangle, PointF point)
        {
            return rectangle.Vertices()
                .Max(p => p.DistanceTo(point));
        }
    }
}