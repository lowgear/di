using System.Collections.Generic;
using System.Linq;
using TagsCloudVisualization.Interfaces;

namespace TagsCloudVisualization.Implementations
{
    public abstract class PreparatingWordLoader : IWordLoader
    {
        public string[] LoadWords(string fileName)
        {
            var words = LoadRawWords(fileName);
            return words
                .Select(word => word.ToLower().Trim())
                .ToArray();
        }

        protected abstract IEnumerable<string> LoadRawWords(string fileName);
    }
}
