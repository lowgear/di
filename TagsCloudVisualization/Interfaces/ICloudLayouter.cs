using System.Collections.Generic;
using System.Drawing;

namespace TagsCloudVisualization.Interfaces
{
    public interface ICloudLayouter
    {
        Image Layout(IEnumerable<string> words);
    }
}