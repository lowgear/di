using System.Drawing;

namespace TagsCloudVisualization.Interfaces
{
    public interface ICloudLayouter
    {
        Image Layout(string[] words);
    }
}