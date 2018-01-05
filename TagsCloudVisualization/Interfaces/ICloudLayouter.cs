using System.Collections.Generic;
using System.Drawing;
using TagsCloudVisualization.Implementations;
using TagsCloudVisualization.util;

namespace TagsCloudVisualization.Interfaces
{
    public interface ICloudLayouter
    {
        Result<Layout> Layout(IDictionary<string, float> wordsAndSizes, float marginToSizeCoefficient, StringFormat stringFormat, FontFamily fontFamily, FontStyle fontStyle, Brush brush, Pen pen);
    }
}