using System;
using System.Runtime.InteropServices;

namespace PoeSharp.Filetypes.Bundle.Internal
{
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    internal readonly struct BundleFileInfo : IEquatable<BundleFileInfo>
    {
        public readonly ulong Hash;
        public readonly uint BundleIndex;
        public readonly uint FileOffset;
        public readonly uint FileSize;

        public override bool Equals(object obj) => obj is BundleFileInfo file && Equals(file);
        public bool Equals(BundleFileInfo other) => Hash == other.Hash;
        public override int GetHashCode() => HashCode.Combine(Hash);

        public static bool operator ==(BundleFileInfo left, BundleFileInfo right) => left.Equals(right);
        public static bool operator !=(BundleFileInfo left, BundleFileInfo right) => !(left == right);
    }
}
