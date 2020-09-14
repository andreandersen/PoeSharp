namespace PoeSharp.Filetypes.Bundle.Internal
{
    internal struct IndexBinHead
    {
        public EncodingInDecimal FirstFileEncode { get; internal init; }

        public uint Unk0 { get; init; }

        public ulong UncompressedSize2 { get; internal init; }
        public ulong TotalPayloadSize2 { get; internal init; }
        public uint EntryCount { get; internal init; }

        public uint Unk1 { get; internal init; }
        public uint Unk2 { get; internal init; }
        public uint Unk3 { get; internal init; }
        public uint Unk4 { get; internal init; }
        public uint Unk5 { get; internal init; }
    }
}