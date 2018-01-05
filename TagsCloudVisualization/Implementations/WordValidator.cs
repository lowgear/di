using System;
using System.Collections.Generic;
using TagsCloudVisualization.Interfaces;

namespace TagsCloudVisualization.Implementations
{
    // ReSharper disable once ClassNeverInstantiated.Global
    class WordValidator : IWordValidator
    {
        private readonly int lengthLimit;
        private readonly ISet<string> excludedWords;
        private static readonly StringComparer Comparer = StringComparer.OrdinalIgnoreCase;

        public WordValidator(IEnumerable<string> excludedWords, int lengthLimit = 0)
        {
            this.lengthLimit = lengthLimit;
            this.excludedWords = new HashSet<string>(excludedWords, Comparer);
            if (lengthLimit < 0)
                throw new ArgumentException($"Length limit should be positive or 0 for no limitation but found {lengthLimit}");
        }

        public WordValidator()
        {
            excludedWords = new HashSet<string>(Comparer);
        }

        public bool IsValid(string word)
        {
            return (lengthLimit == 0 || word.Length <= lengthLimit) && !excludedWords.Contains(word);
        }

        public void AddExcludedWord(string word)
        {
            excludedWords.Add(word);
        }
    }
}