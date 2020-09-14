namespace PoeSharp.Filetypes.Bundle.Internal
{
    internal struct IndexBin
    {
        public uint UncompressedSize { get; internal init; }
        public uint TotalPayloadSize { get; internal init; }
        public uint HeadPayloadSize { get; internal init; }
    }
}