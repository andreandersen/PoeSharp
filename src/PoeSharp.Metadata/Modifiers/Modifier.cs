using System.Collections.Generic;
using PoeSharp.Shared.EphemeralId;

namespace PoeSharp.Metadata.Modifiers
{
    public class Modifier : WithEphemeralStringId<Modifier>
    {
        private readonly List<string> _addsTags;
        private readonly List<TagWeight> _generationWeights;
        private readonly List<TagWeight> _spawnWeights;
        private readonly List<ModifierStatValueRange> _statValueRanges;

        public Modifier(string id, string name, Domain domain,
            GenerationType generationType, bool isEssenceOnly, string group,
            int requiredLevel, List<ModifierStatValueRange> statValueRanges,
            List<string> addsTags, List<TagWeight> spawnWeights,
            List<TagWeight> generationWeights) : base(id)
        {
            Id = id;
            Name = name;
            Domain = domain;
            GenerationType = generationType;
            IsEssenceOnly = isEssenceOnly;
            Group = group;
            RequiredLevel = requiredLevel;
            _statValueRanges = statValueRanges;
            _addsTags = addsTags;
            _spawnWeights = spawnWeights;
            _generationWeights = generationWeights;
        }

        public string Id { get; }
        public string Name { get; }
        public Domain Domain { get; }
        public GenerationType GenerationType { get; }

        public IReadOnlyList<ModifierStatValueRange> StatValueRanges =>
            _statValueRanges;

        public IReadOnlyList<string> AddsTags => _addsTags;
        public bool IsEssenceOnly { get; }
        public string Group { get; }
        public int RequiredLevel { get; }
        public IReadOnlyList<TagWeight> SpawnWeights => _spawnWeights;
        public IReadOnlyList<TagWeight> GenerationWeights => _generationWeights;

        public override string ToString() =>
            $"{Name} ({Id}, {Group}), Req. Level: {RequiredLevel}. Domain: " +
            $"{Domain.ToString()}, Gen.Type: {GenerationType.ToString()}";
    }
}