namespace PoeSharp.Filetypes.Bundle.Internal
{
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    internal readonly struct BundleFileRecord
    {
        public readonly ulong Hash;
        public readonly uint BundleIndex;
        public readonly uint FileOffset;
        public readonly uint FileSize;
    }
}