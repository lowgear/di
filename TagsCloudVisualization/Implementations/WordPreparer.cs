using TagsCloudVisualization.Interfaces;

namespace TagsCloudVisualization.Implementations
{
    public class WordPreparer : IWordPreparer
    {
        public string PrepareWord(string word) => word.ToLower().Trim();
    }
}
