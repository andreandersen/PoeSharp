namespace PoeSharp.Filetypes.Bundle.Internal
{
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    internal readonly struct EncodedBundleHeader
    {
        public readonly uint UncompressedSize;
        public readonly uint TotalPayloadSize;
        public readonly uint HeadPayloadSize;
        public readonly EncodingInDecimal FirstFileEncode;
        public readonly uint Unk0;
        public readonly ulong UncompressedSize2;
        public readonly ulong TotalPayloadSize2;
        public readonly uint EntryCount;
        public readonly uint UncompressedBlockGranularity;
        public readonly uint Unk1;
        public readonly uint Unk2;
        public readonly uint Unk3;
        public readonly uint Unk4;
    }

    public enum EncodingInDecimal : uint
    {
        Kraken6 = 8,
        MermaidA = 9,
        LeviathanC = 13
    }
}