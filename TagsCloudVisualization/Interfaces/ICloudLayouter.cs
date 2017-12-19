using System.Collections.Generic;
using System.Drawing;
using TagsCloudVisualization.util;

namespace TagsCloudVisualization.Interfaces
{
    public interface ICloudLayouter
    {
        Result<Image> Layout(IEnumerable<string> words, int takeSoMany, float marginToSizeCoefficient, StringFormat stringFormat, FontFamily fontFamily, FontStyle fontStyle, Brush brush, Pen pen);
    }
}