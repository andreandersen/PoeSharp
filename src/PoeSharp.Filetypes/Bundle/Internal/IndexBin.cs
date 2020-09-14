namespace PoeSharp.Filetypes.Bundle.Internal
{
    internal struct IndexBin
    {
        public uint UncompressedSize { get; init; }
        public uint TotalPayloadSize { get; init; }
        public uint HeadPayloadSize { get; init; }
    }
}