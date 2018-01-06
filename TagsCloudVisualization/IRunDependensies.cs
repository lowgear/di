using TagsCloudVisualization.Interfaces;

namespace TagsCloudVisualization
{
    internal interface IRunDependensies
    {
        ICloudLayouter CloudLayouter     {get;}
        IWordLoader WordsLoader       {get;}
        IWordPreparer WordPreparer      {get;}
        IWordValidator WordValidator     {get;}
        IWordSizeCalculator WordSizeCalculator{get;}
    }
}