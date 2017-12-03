using System.Drawing;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using TagsCloudVisualization.Extensions;
using TagsCloudVisualization.Implementations;

namespace TagsCloudVisualization.Tests
{
    [TestFixture]
    public class MultithreadedAlignmentLayouterTests
    {
        [Test]
        public void Rectangle_ShouldBeOfSameSizeAsGiven()
        {
            var layouter = new MultithreadedAlignmentLayouter();
            var size = new SizeF(2, 3);
            var expected = new PointF(-size.Width / 2, -size.Height / 2);

            var point = layouter.LayoutRectangles(new[] {size})[0];

            point.Should().Be(expected);
        }

        [TestCase(1)]
        [TestCase(3)]
        [TestCase(10)]
        [TestCase(125)]
        public void NumberOfRectangle_ShouldBeSameAsGiven(int n)
        {
            var layouter = new MultithreadedAlignmentLayouter();
            var size = new SizeF(2, 3);

            var points = layouter.LayoutRectangles(Enumerable.Repeat(size, n));

            points.Length.Should().Be(n);
        }

        [TestCase(1)]
        [TestCase(3)]
        [TestCase(10)]
        [TestCase(125)]
        public void Rectangles_ShouldNotIntersect(int n)
        {
            var layouter = new MultithreadedAlignmentLayouter();
            var size = new SizeF(2, 3);

            var points = layouter.LayoutRectangles(Enumerable.Repeat(size, n));

            var rectangles = points.Select(point => new RectangleF(point, size)).ToList();
            rectangles.ShouldNotIntersect();
        }
    }
}