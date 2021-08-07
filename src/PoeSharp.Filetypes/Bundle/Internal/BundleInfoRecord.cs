namespace PoeSharp.Filetypes.Bundle.Internal
{
    internal sealed class BundleInfoRecord
    {
        public BundleInfoRecord(int uncompressedSize, string name)
        {
            UncompressedSize = uncompressedSize;
            Name = name;
        }

        public readonly int UncompressedSize;
        public readonly string Name;

        public override string ToString() => Name;
    }
}
