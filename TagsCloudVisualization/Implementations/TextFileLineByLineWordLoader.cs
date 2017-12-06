using System.Collections.Generic;
using System.IO;
using System.Text;
using TagsCloudVisualization.Interfaces;

namespace TagsCloudVisualization.Implementations
{
    // ReSharper disable once UnusedMember.Global
    public class TextFileLineByLineWordLoader : IWordLoader
    {
        public IEnumerable<string> LoadWords(string fileName, Encoding encoding)
        {
            var words = new LinkedList<string>();
            using (var sr = new StreamReader(fileName, encoding))
            {
                while (true)
                {
                    var word = sr.ReadLine();
                    if (word == null)
                        break;
                    words.AddLast(word);
                }
            }
            return words;
        }
    }
}
