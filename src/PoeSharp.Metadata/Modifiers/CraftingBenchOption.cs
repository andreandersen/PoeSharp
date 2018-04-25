using System.Collections.Generic;
using PoeSharp.Shared.EphemeralId;

namespace PoeSharp.Metadata.Modifiers
{
    public class CraftingBenchOption : WithEphemeralStringId<CraftingBenchOption>
    {
        private static readonly Dictionary<string, string> MasterNames =
            new Dictionary<string, string>
            {
                { "StrMaster", "Haku" },
                { "StrIntMaster", "Elreon" },
                { "StrDexMaster", "Vagan" },
                { "StrDexIntMaster", "Zana" },
                { "PvPMaster", "Leo" },
                { "IntMaster", "Catarina" },
                { "DexMaster", "Tora" },
                { "DexIntMaster", "Vorici" }
            };

        private readonly List<string> _itemClasses;

        public CraftingBenchOption(string masterId, int masterLevel, string modifierId,
            List<string> itemClasses) : base(masterId + masterLevel + modifierId)
        {
            MasterId = masterId;
            MasterLevel = masterLevel;
            ModifierId = modifierId;
            _itemClasses = itemClasses;
        }

        public IReadOnlyList<string> ItemClasses => _itemClasses;
        public string MasterId { get; }
        public int MasterLevel { get; }
        public string ModifierId { get; }
        public string MasterName => MasterNames[MasterId];

        public override string ToString() =>
            $"{MasterName} Level {MasterLevel}, Mod: {ModifierId}";
    }
}