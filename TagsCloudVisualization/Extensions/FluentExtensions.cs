using System.Collections.Generic;
using System.Drawing;
using FluentAssertions;

namespace TagsCloudVisualization.Extensions
{
    public static class FluentExtensions
    {
        public static void ShouldNotIntersect(this IReadOnlyList<Rectangle> rectangles)
        {
            for (var i = 1; i < rectangles.Count; i++)
            for (var j = 0; j < i; j++)
                rectangles[i].IntersectsWith(rectangles[j]).Should().BeFalse();
        }
    }
}
