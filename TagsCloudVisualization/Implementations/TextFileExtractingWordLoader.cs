using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace TagsCloudVisualization.Implementations
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class TextFileExtractingWordLoader : PreparatingWordLoader
    {
        private static readonly Regex WordRegex = new Regex(@"(?<word>[^\W\d]+)");
        private readonly Encoding encoding;

        public TextFileExtractingWordLoader(Encoding encoding)
        {
            this.encoding = encoding;
        }

        protected override IEnumerable<string> LoadRawWords(string fileName)
        {
            using (var sr = new StreamReader(fileName, encoding))
            {
                return WordRegex
                    .Matches(sr.ReadToEnd())
                    .Cast<Match>()
                    .Select(match => match.Groups["word"].Value);
            }
        }
    }
}