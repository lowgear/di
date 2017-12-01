using System;
using System.Drawing;
using System.Drawing.Text;
using System.Linq;
using MoreLinq;
using TagsCloudVisualization.Interfaces;

namespace TagsCloudVisualization.Implementations
{
    public class CircularCloudLayouter : ICloudLayouter
    {
        private readonly IWordFrequencyCounter wordFrequencyCounter;
        private readonly IWordSizeCalculator wordSizeCalculator;
        private readonly IRectangleLayouter rectangleLayouter;
        private readonly FontFamily fontFamily;
        private readonly Brush brush;
        private TextRenderingHint textRenderingHintSupplier;

        public CircularCloudLayouter(IWordFrequencyCounter wordFrequencyCounter, IWordSizeCalculator wordSizeCalculator, IRectangleLayouter rectangleLayouter, FontFamily fontFamily, Brush brush, Func<TextRenderingHint> textRenderingHintSupplier)
        {
            this.wordFrequencyCounter = wordFrequencyCounter;
            this.wordSizeCalculator = wordSizeCalculator;
            this.rectangleLayouter = rectangleLayouter;
            this.fontFamily = fontFamily;
            this.brush = brush;
            this.textRenderingHintSupplier = textRenderingHintSupplier();
        }

        public Image Layout(string[] words)
        {
            var uniqWordsAndFrequencies = wordFrequencyCounter.CountFrequencies(words)
                .OrderBy(t => t.Item2, OrderByDirection.Descending).ToArray();
            var emSizes = wordSizeCalculator.CalculateEmSizes(uniqWordsAndFrequencies.Select(t => t.Item2).ToArray());

            var graphics = Graphics.FromImage(new Bitmap(1, 1));
            var fonts = Enumerable
                .Range(0, uniqWordsAndFrequencies.Length)
                .Select(i => new Font(fontFamily, emSizes[i]))
                .ToArray();
            var sizes = Enumerable
                .Range(0, uniqWordsAndFrequencies.Length)
                .Select(i => graphics.MeasureString(uniqWordsAndFrequencies[i].Item1, fonts[i]))
                .ToArray();
            var layouts = rectangleLayouter.LayoutRectangles(sizes);

            var minX = layouts
                .Min(layout => layout.X);
            var minY = layouts
                .Min(layout => layout.Y);
            var maxX = Enumerable
                .Range(0, uniqWordsAndFrequencies.Length)
                .Max(i => layouts[i].X + sizes[i].Width);
            var maxY = Enumerable
                .Range(0, uniqWordsAndFrequencies.Length)
                .Max(i => layouts[i].Y + sizes[i].Height);

            var offset = new SizeF(-minX, -minY);

            var width = (int) Math.Ceiling(maxX - minX);
            var height = (int) Math.Ceiling(maxY - minY);
            var bitmap = new Bitmap(width, height);
            graphics = Graphics.FromImage(bitmap);
            graphics.TextRenderingHint = textRenderingHintSupplier;
            foreach (var i in Enumerable.Range(0, uniqWordsAndFrequencies.Length))
            {
                var word = uniqWordsAndFrequencies[i].Item1;
                var font = fonts[i];
                var location = layouts[i];
                graphics.DrawString(word, font, brush, PointF.Add(location, offset));
            }

            return bitmap;
        }
    }
}