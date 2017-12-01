using System.Collections.Generic;
using System.IO;
using System.Text;

namespace TagsCloudVisualization.Implementations
{
    public class TextFileLineByLineWordLoader : PreparatingWordLoader
    {
        private readonly Encoding encoding;

        public TextFileLineByLineWordLoader(Encoding encoding)
        {
            this.encoding = encoding;
        }

        protected override IEnumerable<string> LoadRawWords(string fileName)
        {
            var words = new LinkedList<string>();
            using (var sr = new StreamReader(fileName, encoding))
            {
                while (true)
                {
                    var word = sr.ReadLine();
                    if (word == null)
                        break;
                    words.AddLast(word.Trim());
                }
            }
            return words;
        }
    }
}
