using System;
using System.Collections.Generic;
using TagsCloudVisualization.Interfaces;

namespace TagsCloudVisualization.Implementations
{
    // ReSharper disable once ClassNeverInstantiated.Global
    class WordValidator : IWordValidator
    {
        private readonly ISet<string> excludedWords;

        public WordValidator(IEnumerable<string> excludedWords)
        {
            this.excludedWords = new HashSet<string>(excludedWords, StringComparer.OrdinalIgnoreCase);
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