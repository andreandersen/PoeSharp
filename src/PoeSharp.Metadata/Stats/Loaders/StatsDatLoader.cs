using System.Collections.Generic;
using PoeSharp.Files.Dat;
using PoeSharp.Shared;

namespace PoeSharp.Metadata.Stats.Loaders
{
    public class StatsDatLoader : IDataLoader<DatFileIndex, IReadOnlyDictionary<string, Stat>>
    {
        public IReadOnlyDictionary<string, Stat> Load(DatFileIndex source)
        {
            var statsDat = source["Stats.dat"];
            var stats = new Dictionary<string, Stat>(statsDat.Count);

            foreach (var s in statsDat)
            {
                var id = s["Id"].Value.ToString();
                var isLocal = ((bool)s["IsLocal"].Value);
                var mainHandAlias = ((DatRow)s["MainHandAlias_StatsKey"].Value)?["Id"]?.ToString();
                var offHandAlias = ((DatRow)s["OffHandAlias_StatsKey"].Value)?["Id"]?.ToString();

                stats.Add(id, new Stat(id, isLocal, mainHandAlias, offHandAlias));
            }
            return stats;
        }
    }
}
