using System.Collections.Generic;
using System.Drawing;

namespace TagsCloudVisualization.Interfaces
{
    public interface ICloudLayouter
    {
        Image Layout(IEnumerable<string> words, int takeSoMany, float marginToSizeCoefficient, StringFormat stringFormat, FontFamily fontFamily,
            FontStyle fontStyle, Brush brush, Pen pen);
    }
}