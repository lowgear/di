using System;
using System.Collections.Generic;
using TagsCloudVisualization.Interfaces;

namespace TagsCloudVisualization.Implementations
{
    // ReSharper disable once ClassNeverInstantiated.Global
    class WordValidator : IWordValidator
    {
        private readonly ISet<string> excludedWords;
        private static readonly StringComparer Comparer = StringComparer.OrdinalIgnoreCase;

        public WordValidator(IEnumerable<string> excludedWords)
        {
            this.excludedWords = new HashSet<string>(excludedWords, Comparer);
        }

        public WordValidator()
        {
            excludedWords = new HashSet<string>(Comparer);
        }

        public bool IsValid(string word)
        {
            return !excludedWords.Contains(word);
        }

        public void AddExcludedWord(string word)
        {
            excludedWords.Add(word);
        }
    }
}