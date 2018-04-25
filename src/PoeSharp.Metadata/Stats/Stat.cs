using PoeSharp.Shared.EphemeralId;

namespace PoeSharp.Metadata.Stats
{
    public class Stat : WithEphemeralStringId<Stat>
    {
        public Stat(string id, bool isLocal,
            string mainHandAlias, string offHandAlias) : base(id)
        {
            Id = id;
            IsLocal = isLocal;
            MainHandAlias = mainHandAlias;
            OffHandAlias = offHandAlias;
        }

        public string Id { get; }
        public bool IsLocal { get; }
        public bool IsWeaponLocal { get; }
        public string MainHandAlias { get; }
        public string OffHandAlias { get; }
        public bool IsAliased => !string.IsNullOrEmpty(MainHandAlias) || !string.IsNullOrEmpty(OffHandAlias);

        public override string ToString() =>
            $"{Id}, Local: {IsLocal}, Aliased: {IsAliased}";
    }
}