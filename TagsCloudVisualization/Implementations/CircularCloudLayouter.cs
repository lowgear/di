using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Linq;
using MoreLinq;
using TagsCloudVisualization.Interfaces;

namespace TagsCloudVisualization.Implementations
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class CircularCloudLayouter : ICloudLayouter
    {
        private readonly IWordFrequencyCounter wordFrequencyCounter;
        private readonly IWordSizeCalculator wordSizeCalculator;
        private readonly IRectangleLayouter rectangleLayouter;
        private readonly FontFamily fontFamily;
        private readonly Brush brush;
        private readonly StringFormat stringFormat;
        private readonly int style;
        private readonly Pen pen;

        public CircularCloudLayouter(IWordFrequencyCounter wordFrequencyCounter,
            IWordSizeCalculator wordSizeCalculator, IRectangleLayouter rectangleLayouter,
            FontFamily fontFamily, Brush brush,
            Func<StringFormat> stringFormatSupplier, Func<FontStyle> styleSupplier, Pen pen)
        {
            this.wordFrequencyCounter = wordFrequencyCounter;
            this.wordSizeCalculator = wordSizeCalculator;
            this.rectangleLayouter = rectangleLayouter;
            this.fontFamily = fontFamily;
            this.brush = brush;
            this.pen = pen;
            style = (int) styleSupplier();
            stringFormat = stringFormatSupplier();
        }

        public Image Layout(IEnumerable<string> words)
        {
            words = words.ToArray();

            var uniqWordsAndFrequencies = wordFrequencyCounter.CountFrequencies(words)
                .OrderBy(t => t.Item2, OrderByDirection.Descending).ToArray();
            var pointSizes =
                wordSizeCalculator.CalculateEmSizes(uniqWordsAndFrequencies.Select(t => t.Item2).ToArray());
            var rectangles = Enumerable
                .Range(0, uniqWordsAndFrequencies.Length)
                .Select(i => MeasureWord(uniqWordsAndFrequencies[i].Item1, pointSizes[i]))
                .ToArray();
            var layouts = rectangleLayouter.LayoutRectangles(rectangles.Select(r => new SizeF(r.Width, r.Height)));

            var minX = layouts
                .Min(layout => layout.X);
            var minY = layouts
                .Min(layout => layout.Y);
            var maxX = Enumerable
                .Range(0, uniqWordsAndFrequencies.Length)
                .Max(i => layouts[i].X + rectangles[i].Right);
            var maxY = Enumerable
                .Range(0, uniqWordsAndFrequencies.Length)
                .Max(i => layouts[i].Y + rectangles[i].Bottom);

            var offset = new SizeF(-minX, -minY);

            var width = (int) Math.Ceiling(maxX - minX);
            var height = (int) Math.Ceiling(maxY - minY);
            var bitmap = new Bitmap(width, height);
            var graphics = Graphics.FromImage(bitmap);
            SetUpGraphics(graphics);
            var graphicsPath = new GraphicsPath();
            foreach (var i in Enumerable.Range(0, uniqWordsAndFrequencies.Length))
            {
                var word = uniqWordsAndFrequencies[i].Item1;
                var location = layouts[i];
                var pointSize = pointSizes[i];
                var subOffset = new SizeF(rectangles[i].X, rectangles[i].Y);
                graphicsPath.AddString(
                    word,
                    fontFamily,
                    style,
                    pointSize,
                    PointF.Subtract(PointF.Add(location, offset), subOffset),
                    stringFormat);
            }
            graphics.FillPath(brush, graphicsPath);
            graphics.DrawPath(pen, graphicsPath);

            return bitmap;
        }

        private static void SetUpGraphics(Graphics graphics)
        {
            graphics.InterpolationMode = InterpolationMode.High;
            graphics.SmoothingMode = SmoothingMode.HighQuality;
            graphics.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;
            graphics.CompositingQuality = CompositingQuality.HighQuality;
        }

        private RectangleF MeasureWord(string word, float pointSize)
        {
            var path = new GraphicsPath();
            path.AddString(word,
                fontFamily,
                style,
                pointSize,
                PointF.Empty,
                stringFormat);
            return path.GetBounds();
        }
    }
}
