using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using PoeSharp.Metadata.Stats;
using PoeSharp.Shared;

namespace PoeSharp.Metadata.Translations.Loaders
{
    public class TranslationsTextFileLoader : IDataLoader<Stream, TranslationsIndex>
    {
        private readonly IReadOnlyDictionary<string, Stat> _stats;

        public TranslationsTextFileLoader(IReadOnlyDictionary<string, Stat> stats)
        {
            _stats = stats;
        }

        public TranslationsIndex Load(Stream source)
        {
            using (var sr = new StreamReader(source))
            {
                var str = sr.ReadToEnd();
                var descriptions = str
                    .Split(new[] { "description\r\n" }, StringSplitOptions.RemoveEmptyEntries)
                    .Where(c => c.StartsWith("\t"))
                    .ToList();

                var translations = new List<Translation>(descriptions.Count);

                foreach (var desc in descriptions)
                {
                    var lines = desc.Split(new[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
                    var statStrings = lines[0]
                        .Trim().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)
                        .Skip(1).Select(c => c.Trim()).ToList();

                    if (!statStrings.All(c => _stats.ContainsKey(c)))
                    {
                        throw new Exception("Not all stats were in stats dictionary");
                    }

                    var variations = lines.Skip(2).Take(int.Parse(lines[1]))
                        .Select(c => ExtractVariation(c, statStrings)).ToList();

                    translations.Add(new Translation(variations));
                }

                var translationsIndex = new TranslationsIndex(translations);
                return translationsIndex;
            }
        }

        private Variation ExtractVariation(string c, List<string> statStrings)
        {
            var i = c.IndexOf('"');
            var ii = c.IndexOf('"', i + 1);

            var text = c.Substring(i + 1, ii - i - 1);

            var conditionStrings = c.Substring(0, i).Trim().Split(' ').ToList();
            var conditions = statStrings.Select((s, n) =>
            {
                var nums = conditionStrings[n].Split('|')
                    .Select(o => int.TryParse(o, out var r) ? (int?)r : null)
                    .ToList();
                return (Id: s, Val: new Range(nums.First(), nums.Last()));
            }).ToDictionary(p => _stats[p.Id], p => p.Val);

            var handlerDict = new ConcurrentDictionary<Stat, List<TranslationHandlers.IHandler>>();
            if (c.Length > ii)
            {
                var extra = c.Substring(ii + 1).Trim().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                for (var j = 0; j < extra.Length; j += 2)
                {
                    var arg = extra[j];
                    if (arg == "canonical_line")
                    {
                        j--;
                        continue;
                    }
                    if (arg.StartsWith("reminder") || arg == "canonical_stat")
                    {
                        continue;
                    }

                    if (int.TryParse(extra[j + 1], out var statIdx))
                    {
                        if (!TranslationHandlers.Handlers.ContainsKey(arg))
                        {
                            throw new Exception($"Handler not found, {arg} for {text}");
                        }

                        var stat = conditions.ElementAt(statIdx - 1).Key;
                        handlerDict.AddOrUpdate(stat,
                            (_) => new List<TranslationHandlers.IHandler> { TranslationHandlers.Handlers[arg] },
                            (_, l) => { l.Add(TranslationHandlers.Handlers[arg]); return l; });
                    }
                }
            }

            return new Variation(text, conditions, handlerDict.ToDictionary(o => o.Key, o => o.Value));
        }
    }
}
