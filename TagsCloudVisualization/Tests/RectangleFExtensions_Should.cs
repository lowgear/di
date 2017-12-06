using System.Drawing;
using FluentAssertions;
using NUnit.Framework;
using TagsCloudVisualization.Extensions;

namespace TagsCloudVisualization.Tests
{
    [TestFixture]
    public class RectangleFExtensions_Should
    {
        [Test]
        public void LocationsIfHadVertex_ReturnsProperLocations()
        {
            new RectangleF(PointF.Empty, new Size(1, 2)).LocationsIfHadVertex(new PointF(2, 3))
                .ShouldBeEquivalentTo(new[]
                {
                    new PointF(2, 3),
                    new PointF(1, 3),
                    new PointF(2, 1),
                    new PointF(1, 1)
                });
        }

        [Test]
        public void RectangleVertices_ShouldReturnAllItsVertices()
        {
            new RectangleF(PointF.Empty, new Size(1, 2))
                .Vertices()
                .ShouldBeEquivalentTo(new[]
                {
                    new PointF(0, 0),
                    new PointF(0, 2),
                    new PointF(1, 0),
                    new PointF(1, 2)
                });
        }

        [TestCase(0, -1, 1, -1, TestName = "ShouldReturnDistanceToUpperRigthCorner")]
        [TestCase(-1, -1, -1, -1, TestName = "ShouldReturnDistanceToUpperLeftCorner")]
        [TestCase(0, 0, 1, 1, TestName = "ShouldReturnDistanceToLowerRigthCorner")]
        [TestCase(-1, 0, -1, 1, TestName = "ShouldReturnDistanceToLowerLeftCorner")]
        public void MaxDistanceTo_Origin(int rectX, int rectY, int cornerX, int cornerY)
        {
            new RectangleF(rectX, rectY, 1, 1).MaxDistanceTo(PointF.Empty)
                .Should()
                .Be(new PointF(cornerX, cornerY).DistanceTo(PointF.Empty));
        }
    }
}