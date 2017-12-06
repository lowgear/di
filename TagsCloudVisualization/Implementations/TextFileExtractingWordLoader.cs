using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using TagsCloudVisualization.Interfaces;

namespace TagsCloudVisualization.Implementations
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class TextFileExtractingWordLoader : IWordLoader
    {
        private static readonly Regex WordRegex = new Regex(@"(?<word>[^\W\d]+)");

        public IEnumerable<string> LoadWords(string fileName, Encoding encoding)
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