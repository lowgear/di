using System;
using System.Collections.Generic;

namespace TagsCloudVisualization.Interfaces
{
    public interface IWordFrequencyCounter
    {
        IEnumerable<Tuple<string, int>> CountFrequencies(string[] words);
    }
}