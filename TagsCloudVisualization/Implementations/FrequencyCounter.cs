using System;
using System.Collections.Generic;
using System.Linq;
using TagsCloudVisualization.Interfaces;

namespace TagsCloudVisualization.Implementations
{
    public class FrequencyCounter : IWordFrequencyCounter
    {
        public IEnumerable<Tuple<string, int>> CountFrequencies(string[] words)
        {
            return words
                .GroupBy(word => word)
                .Select(group => Tuple.Create(group.Key, group.Count()))
                .ToArray();
        }
    }
}