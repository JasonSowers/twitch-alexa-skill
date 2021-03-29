using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Transactions;
using FuzzySharp;
using FuzzySharp.Extractor;
using FuzzySharp.SimilarityRatio;
using FuzzySharp.SimilarityRatio.Scorer;
using FuzzySharp.SimilarityRatio.Scorer.Composite;
using FuzzySharp.SimilarityRatio.Scorer.StrategySensitive;

namespace twitch_alexa_skill.Utils
{
    public static class StringMatch
    {
        // 
        public static ConcurrentDictionary<string, double> GetScore(string input, string[] titles)
        {
            var dict = new ConcurrentDictionary<string, double>();

            foreach (var title in titles)
            {
                dict[title] = Levenshtein.GetRatio(input.ToLower(), title.ToLower());
            }

            return dict;
        }

    }

}
