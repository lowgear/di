using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using TagsCloudVisualization.util;

namespace TagsCloudVisualization.Implementations
{
    public class Layout
    {
        private readonly Size size;
        private readonly IEnumerable<Tuple<string, float, PointF>> layouts;
        private readonly StringFormat stringFormat;
        private readonly FontFamily fontFamily;
        private readonly FontStyle fontStyle;
        private readonly Brush brush;
        private readonly Pen pen;

        public Layout(Size size, IEnumerable<Tuple<string, float, PointF>> layouts, StringFormat stringFormat,
            FontFamily fontFamily, FontStyle fontStyle, Brush brush, Pen pen)
        {
            this.size = size;
            this.layouts = layouts;
            this.stringFormat = stringFormat;
            this.fontFamily = fontFamily;
            this.fontStyle = fontStyle;
            this.brush = brush;
            this.pen = pen;
        }

        public Result<Image> DrawLayout(
            CompositingQuality compositingQuality = CompositingQuality.HighQuality,
            TextRenderingHint textRenderingHint = TextRenderingHint.AntiAliasGridFit,
            SmoothingMode smoothingMode = SmoothingMode.HighQuality,
            InterpolationMode interpolationMode = InterpolationMode.High)
        {
            Bitmap bitmap;
            try
            {
                bitmap = new Bitmap(size.Width, size.Height);
            }
            catch (Exception)
            {
                return Result.Fail<Image>($"{size.Width}x{size.Height} to big size for image.");
            }
            var graphics = Graphics.FromImage(bitmap);

            graphics.InterpolationMode = interpolationMode;
            graphics.SmoothingMode = smoothingMode;
            graphics.TextRenderingHint = textRenderingHint;
            graphics.CompositingQuality = compositingQuality;

            var graphicsPath = new GraphicsPath();
            foreach (var wordAndLayout in layouts)
                graphicsPath.AddString(
                    wordAndLayout.Item1,
                    fontFamily,
                    (int) fontStyle,
                    wordAndLayout.Item2,
                    wordAndLayout.Item3,
                    stringFormat);
            graphics.FillPath(brush, graphicsPath);
            graphics.DrawPath(pen, graphicsPath);

            return bitmap;
        }
    }
}