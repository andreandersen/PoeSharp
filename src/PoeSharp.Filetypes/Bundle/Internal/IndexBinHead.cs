namespace PoeSharp.Filetypes.Bundle.Internal
{
    internal struct IndexBinHead
    {
        public EncodingInDecimal FirstFileEncode { get; init; }

        public uint Unk0 { get; init; }

        public ulong UncompressedSize2 { get; init; }
        public ulong TotalPayloadSize2 { get; init; }
        public uint EntryCount { get; init; }

        public uint Unk1 { get; init; }
        public uint Unk2 { get; init; }
        public uint Unk3 { get; init; }
        public uint Unk4 { get; init; }
        public uint Unk5 { get; init; }

        internal enum EncodingInDecimal : uint
        {
            Kraken6 = 8,
            MermaidA = 9,
            LeviathanC = 13
        }
    }
}