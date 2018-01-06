using System;
using System.Collections.Generic;
using System.Linq;

namespace TagsCloudVisualization.Implementations
{
    public static class FrequencyCounter
    {
        public static Tuple<T, int>[] CountFrequencies<T>(IEnumerable<T> elements)
        {
            return elements
                .GroupBy(element => element)
                .Select(group => Tuple.Create(group.Key, group.Count()))
                .ToArray();
        }
    }
}