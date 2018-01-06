using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using TagsCloudVisualization.Interfaces;
using TagsCloudVisualization.util;

namespace TagsCloudVisualization.Implementations
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class CloudLayouter : ICloudLayouter
    {
        private readonly IRectangleLayouter rectangleLayouter;
        private readonly IMarginCalculator marginCalculator;

        public CloudLayouter(IRectangleLayouter rectangleLayouter,
            IMarginCalculator marginCalculator)
        {
            this.rectangleLayouter = rectangleLayouter;
            this.marginCalculator = marginCalculator;
        }

        public Result<Layout> Layout(IDictionary<string, float> wordsAndSizes,
            float marginToSizeCoefficient,
            StringFormat stringFormat, FontFamily fontFamily, FontStyle fontStyle, Brush brush, Pen pen)
        {
            try
            {
                var orderedWordsAndSizes = wordsAndSizes.OrderByDescending(kv => kv.Value).ToArray();

                var rectangles = orderedWordsAndSizes
                    .Select(kv => MeasureWord(
                        kv.Key,
                        kv.Value,
                        stringFormat,
                        fontFamily,
                        fontStyle))
                    .Select(marginCalculator.CalculateBounds)
                    .ToArray();
                var locations =
                    rectangleLayouter.LayoutRectangles(rectangles.Select(r => new SizeF(r.Width, r.Height)));

                var layouts = locations.Zip(rectangles, Tuple.Create).ToArray();

                var minX = layouts
                    .Min(layout => layout.Item1.X);
                var minY = layouts
                    .Min(layout => layout.Item1.Y);
                var maxX = layouts
                    .Max(layout => layout.Item1.X + layout.Item2.Right);
                var maxY = layouts
                    .Max(layout => layout.Item1.Y + layout.Item2.Bottom);

                var offset = new SizeF(-minX, -minY);

                var width = (int) Math.Ceiling(maxX - minX);
                var height = (int) Math.Ceiling(maxY - minY);
                var size = new Size(width, height);

                var locationsOnImage = layouts.Select(layout =>
                {
                    var subOffset = new SizeF(layout.Item2.X, layout.Item2.Y);
                    return PointF.Subtract(PointF.Add(layout.Item1, offset),
                        subOffset);
                });
                var imageLayouts = orderedWordsAndSizes.Zip(locationsOnImage,
                    (ws, loc) => Tuple.Create(ws.Key, ws.Value, loc));
                return new Layout(size, imageLayouts, stringFormat, fontFamily, fontStyle,
                    brush, pen);
            }
            catch (Exception e)
            {
                return Result.Fail<Layout>(e.Message);
            }
        }

        private static RectangleF MeasureWord(string word, float pointSize, StringFormat stringFormat,
            FontFamily fontFamily, FontStyle fontStyle)
        {
            var path = new GraphicsPath();
            path.AddString(
                word,
                fontFamily,
                (int) fontStyle,
                pointSize,
                PointF.Empty,
                stringFormat);
            return path.GetBounds();
        }
    }
}
