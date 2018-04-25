using System.Collections.Generic;
using PoeSharp.Shared.EphemeralId;

namespace PoeSharp.Metadata.Modifiers
{
    public class EssenceModifier : WithEphemeralStringId<EssenceModifier>
    {
        private readonly Dictionary<string, string> _itemClasses;

        public EssenceModifier(string id, string name, int droplevel,
            Dictionary<string, string> itemClasses) : base(id)
        {
            Id = id;
            Name = name;
            DropLevel = droplevel;
            _itemClasses = itemClasses;
        }

        public string Id { get; }
        public string Name { get; }
        public int DropLevel { get; }
        public IReadOnlyDictionary<string, string> ItemClasses => _itemClasses;

        public override string ToString() =>
            $"{Name} ({Id}), DropLevel: {DropLevel}";
    }
}