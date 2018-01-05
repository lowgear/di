using System.Drawing;
using static System.Math;

namespace TagsCloudVisualization.Extensions
{
    public static class PointFExtensions
    {
        public static double DistanceTo(this PointF A, PointF B)
        {
            return Sqrt(Pow(A.X - B.X, 2) + Pow(A.Y - B.Y, 2));
        }
    }
}