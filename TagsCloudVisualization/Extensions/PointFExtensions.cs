using System;
using System.Drawing;

namespace TagsCloudVisualization.Extensions
{
    public static class PointFExtensions
    {
        public static double DistanceTo(this PointF point1, PointF point2)
        {
            return Math.Sqrt(Math.Pow(point1.X - point2.X, 2) + Math.Pow(point1.Y - point2.Y, 2));
        }
    }
}