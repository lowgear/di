using TagsCloudVisualization.Interfaces;

namespace TagsCloudVisualization
{
    internal class RunDependensies : IRunDependensies
    {
        public ICloudLayouter CloudLayouter { get; }
        public IWordLoader WordsLoader { get; }
        public IWordPreparer WordPreparer { get; }
        public IWordValidator WordValidator { get; }
        public IWordSizeCalculator WordSizeCalculator { get; }

        public RunDependensies(ICloudLayouter cloudLayouter, IWordLoader wordsLoader, IWordPreparer wordPreparer, IWordValidator wordValidator, IWordSizeCalculator wordSizeCalculator)
        {
            CloudLayouter = cloudLayouter;
            WordsLoader = wordsLoader;
            WordPreparer = wordPreparer;
            WordValidator = wordValidator;
            WordSizeCalculator = wordSizeCalculator;
        }
    }
}