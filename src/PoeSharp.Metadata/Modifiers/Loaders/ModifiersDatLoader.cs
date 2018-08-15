using System.Collections.Generic;
using System.Linq;
using PoeSharp.Files.Dat;
using PoeSharp.Metadata.Stats;
using PoeSharp.Shared;

namespace PoeSharp.Metadata.Modifiers.Loaders
{
    public class ModifiersDatLoader : IDataLoader<DatFileIndex, IReadOnlyDictionary<string, Modifier>>
    {
        IReadOnlyDictionary<string, Stat> _stats;

        public ModifiersDatLoader(IReadOnlyDictionary<string, Stat> stats)
        {
            _stats = stats;
        }

        public IReadOnlyDictionary<string, Modifier> Load(DatFileIndex source)
        {
            var modsDat = source["Mods.dat"];
            var numMods = modsDat.Count;

            var dict = new Dictionary<string, Modifier>(numMods);

            for (int i = 0; i < numMods; i++)
            {
                var m = modsDat[i];

                var id = m["Id"].ToString();
                var name = m["Name"].ToString();
                var genType = (GenerationType)m["GenerationType"].Value;
                var domain = (Domain)m["Domain"].Value;
                var isEssenceOnly = (bool)m["IsEssenceOnlyModifier"].Value;
                var group = m["CorrectGroup"].Value.ToString();
                var requiredLevel = (int)m["Level"].Value;

                var addsTags = ((List<DatRow>)m["TagsKeys"].Value)
                    .Select(c => c["Id"].Value.ToString()).ToList();

                var statRanges = Enumerable.Range(1, 5)
                    .Select(x =>
                    {
                        var key = ((DatRow)m[$"StatsKey{x}"].Value)?["Id"].Value.ToString();
                        if (key == null)
                        {
                            return null;
                        }

                        var stat = _stats[key];
                        var min = (int)m[$"Stat{x}Min"].Value;
                        var max = (int)m[$"Stat{x}Max"].Value;

                        // Can't recall what this was for. Pending deletion.
                        // var statData = (List<object>)m[$"Data{x}"].Value;

                        return new ModifierStatValueRange(stat, min, max);
                    })
                    .Where(c => c != null)
                    .ToList();


                var spawnWeightTags = ((List<DatRow>)m["SpawnWeight_TagsKeys"].Value)
                    .Select(c => c["Id"].Value.ToString()).ToList();
                var spawnWeightValues = ((List<object>)m["SpawnWeight_Values"].Value)
                    .Cast<uint>().ToList();

                var spawnWeights = spawnWeightTags
                    .Zip(spawnWeightValues, (s, v) => new TagWeight(s, (int)v))
                    .ToList();

                var generationWeightTags = ((List<DatRow>)m["GenerationWeight_TagsKeys"].Value)
                    .Select(c => c["Id"].Value.ToString()).ToList();
                var generationWeightValues = ((List<object>)m["GenerationWeight_Values"].Value)
                    .Cast<int>().ToList();

                var generationWeights = generationWeightTags
                    .Zip(generationWeightValues, (s, v) => new TagWeight(s, (int)v))
                    .ToList();

                var mod = new Modifier(id, name, domain, genType, isEssenceOnly, group,
                    requiredLevel, statRanges, addsTags, spawnWeights, generationWeights);

                dict.Add(id, mod);
            }

            return dict;
        }
    }
}
