using System.Drawing;
using TagsCloudVisualization.Interfaces;

namespace TagsCloudVisualization.Implementations
{
    public class RelativeMarginCalculator : IMarginCalculator
    {
        private readonly float marginCoefficient;
        private readonly float horizintalMarginCoefficient = 4;

        public RelativeMarginCalculator(float marginCoefficient)
        {
            this.marginCoefficient = marginCoefficient;
        }

        public RectangleF CalculateBounds(RectangleF rectangle)
        {
            var margin = rectangle.Height * marginCoefficient;
            rectangle.X -= margin * horizintalMarginCoefficient;
            rectangle.Width += margin * 2 * horizintalMarginCoefficient;
            rectangle.Y -= margin;
            rectangle.Height += margin * 2;

            return rectangle;
        }
    }
}