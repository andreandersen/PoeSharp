using System;

namespace PoeSharp.Filetypes.Bundle.Internal
{
    internal readonly struct BundleInfo : IEquatable<BundleInfo>
    {
        public BundleInfo(int uncompressedSize, string name)
        {
            UncompressedSize = uncompressedSize;
            Name = name;
        }

        public readonly int UncompressedSize;
        public readonly string Name;

        public override bool Equals(object? obj) =>
            obj is BundleInfo info && Equals(info);

        public bool Equals(BundleInfo other) =>
            Name == other.Name &&
            UncompressedSize == other.UncompressedSize;

        public override int GetHashCode() =>
            HashCode.Combine(Name, UncompressedSize);

        public static bool operator ==(BundleInfo left, BundleInfo right) =>
            left.Equals(right);

        public static bool operator !=(BundleInfo left, BundleInfo right) =>
            !(left == right);
    }
}
