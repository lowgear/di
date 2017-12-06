using System.Collections.Generic;
using System.Text;

namespace TagsCloudVisualization.Interfaces
{
    public interface IWordLoader
    {
        IEnumerable<string> LoadWords(string fileName, Encoding encoding);
    }
}