namespace TagsCloudVisualization.Interfaces
{
    public interface IWordValidator
    {
        bool IsValid(string word);
        void AddExcludedWord(string word);
    }
}