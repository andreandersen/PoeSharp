using System.Collections.Generic;
using System.IO;
using System.Linq;
using PoeSharp.Files.StatDescriptions;
using PoeSharp.Files.StatDescriptions.SyntaxTree;
using PoeSharp.Metadata.Stats;
using PoeSharp.Shared;

namespace PoeSharp.Metadata.Translations.Loaders
{
    public class TranslationsLoader : IDataLoader<Stream, TranslationsIndex>
    {
        private static readonly IReadOnlyDictionary<string, ushort> IgnoreExtras =
            new Dictionary<string, ushort>
            {
                {"canonical_line", 0},
                {"canonical_stat", 1},
                {"reminderstring", 1}
            };

        private readonly IReadOnlyDictionary<string, Stat> _stats;

        public TranslationsLoader(IReadOnlyDictionary<string, Stat> stats) =>
            _stats = stats;

        public TranslationsIndex Load(Stream source)
        {
            string str;
            using (var sr = new StreamReader(source))
            {
                str = sr.ReadToEnd();
            }

            var translations = StatDescriptionParser
                .ParseInstructions(str)
                .OfType<Description>()
                .Select(ConvertDescriptionToTranslation)
                .ToList();

            return new TranslationsIndex(translations);
        }

        private Translation ConvertDescriptionToTranslation(Description desc)
        {
            // I'm just interested in English, as it's the language
            // used with the APIs such as the public stash tab etc
            var variations = desc.Languages
                .First(c => c.Name == "English")
                .Variations
                .Select(c => ConvertDescriptionVariation(c, desc))
                .ToList();

            return new Translation(variations);
        }

        private Variation ConvertDescriptionVariation(DescriptionVariation v,
            Description d)
        {
            var handlers = ExtractHandlers(v, d)
                .GroupBy(c => c.Key)
                .ToDictionary(c => c.Key, c => c.Select(e => e.Value).ToList());

            var conditions = ExtractConditions(v, d);
            var variation = new Variation(v.TranslationTemplate, conditions, handlers);
            return variation;
        }

        private IEnumerable<KeyValuePair<Stat, TranslationHandlers.IHandler>>
            ExtractHandlers(DescriptionVariation v, Description d)
        {
            var parameters = StripIgnoredComments(v)
                .Where(c => !string.IsNullOrWhiteSpace(c))
                .ToList();

            var parameterCount = parameters.Count;
            for (var i = 0; i < parameterCount; i += 2)
            {
                var handlerName = parameters[i];
                var handlerIndex = int.Parse(parameters[i + 1]) - 1;
                var handler = TranslationHandlers.Handlers[handlerName];
                var stat = _stats[d.Stats[handlerIndex]];

                yield return new KeyValuePair<Stat, TranslationHandlers.IHandler>(stat,
                    handler);
            }
        }

        private Dictionary<Stat, Range> ExtractConditions(DescriptionVariation v,
            Description d)
        {
            var ranges = v.RangeText
                .Split(' ')
                .Where(c => !string.IsNullOrWhiteSpace(c))
                .Select((c, i) =>
                {
                    return GetY(d, c, i);
                })
                .ToDictionary(c => c.stat, c => c.range);
            return ranges;
        }

        private (Stat stat, Range range) GetY(Description d, string c, int i)
        {
            var minMax = c.Split('|');

            int? minNullable = default;
            int? maxNullable = default;

            if (int.TryParse(minMax.First(), out var min))
            {
                minNullable = min;
            }

            if (int.TryParse(minMax.Last(), out var max))
            {
                maxNullable = max;
            }

            var stat = _stats[d.Stats[i]];
            var range = new Range(minNullable, maxNullable);

            return (stat, range);
        }

        private IEnumerable<string> StripIgnoredComments(DescriptionVariation v)
        {
            var extraInstructionsArr = v
                .ExtraInstructions
                .Split(' ')
                .ToArray();

            var count = extraInstructionsArr.Length;

            for (var i = 0; i < count; i++)
            {
                if (IgnoreExtras.TryGetValue(extraInstructionsArr[i], out var inc))
                {
                    i += inc;
                    continue;
                }

                yield return extraInstructionsArr[i];
            }
        }
    }
}