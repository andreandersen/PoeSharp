using System.Collections.Generic;
using System.Text.RegularExpressions;
using PoeSharp.Metadata.Stats;

namespace PoeSharp.Metadata.Translations
{
    public class Variation
    {
        private static readonly Regex paramRegex = new Regex(@"(%)(\d+)(%)");
        private static readonly Regex paramRegexDollarPlus = new Regex(@"(%)(\d+)(\$\+d)");
        private static readonly Regex paramRegexDollar = new Regex(@"(%)(\d+)(\$d)");
        private static readonly Regex percentageRegex = new Regex("%%");

        public Variation(string text, Dictionary<Stat, Range> conditions, 
            Dictionary<Stat, List<TranslationHandlers.IHandler>> handlers)
        {
            var paramText = text;
            paramText = paramRegexDollarPlus.Replace(paramText, "+{$2}");
            paramText = paramRegexDollar.Replace(paramText, "{$2}");
            paramText = paramRegex.Replace(paramText, "{$2}");
            paramText = percentageRegex.Replace(paramText, "%");

            TranslationText = paramText;

            var normalizedText = text;
            normalizedText = paramRegexDollarPlus.Replace(normalizedText, "+#");
            normalizedText = paramRegexDollar.Replace(normalizedText, "#");
            normalizedText = paramRegex.Replace(normalizedText, "#");
            normalizedText = percentageRegex.Replace(normalizedText, "%");

            TranslationTextNormalized = normalizedText;

            _conditions = conditions;
            _handlers = handlers;
        }

        public IReadOnlyDictionary<Stat, Range> Conditions => _conditions;
        public IReadOnlyDictionary<Stat, List<TranslationHandlers.IHandler>> Handlers => _handlers;

        public string TranslationText { get; }
        public string TranslationTextNormalized { get; }
        private Dictionary<Stat, Range> _conditions { get; }
        private Dictionary<Stat, List<TranslationHandlers.IHandler>> _handlers { get; }
    }
}
