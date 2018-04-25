using System.Collections.Generic;
using System.Linq;
using PoeSharp.Metadata.Stats;

namespace PoeSharp.Metadata.Translations
{
    public class TranslationsIndex
    {
        private readonly List<Translation> _translations;

        private readonly Dictionary<Stat, List<Translation>> _byStat;
        private readonly Dictionary<string, Translation> _byAllStats;

        public IReadOnlyList<Translation> Translations => _translations;

        public TranslationsIndex(List<Translation> translations)
        {
            _translations = translations;
        }

        public List<Translation> this[Stat index] => 
            _byStat.TryGetValue(index, out var res) ? res : null;

        public Translation this[List<Stat> index] => 
            _byAllStats.TryGetValue(string.Join("", index.Select(c => c.Id).OrderBy(c => c)), out var res) ? res : null;
    }
}
