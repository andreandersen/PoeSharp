using System;
using System.IO;
using System.Runtime.InteropServices;

namespace PoeSharp.Filetypes.Bundle
{
    public static class BundleIndex
    {
        public static void C(Stream src)
        {
            var bundleCount = (int)src.ReadUInt32();
            var bundleInfo = new BundleInfo[bundleCount];

            for (var i = 0; i < bundleCount; i++)
            {
                var nameLen = (int)src.ReadUInt32();
                var name = src.ReadUtf8String(nameLen);
                var uncompressedSize = (int)src.ReadUInt32();

                bundleInfo[i] = new BundleInfo(
                    uncompressedSize, name.ToString());
            }

            var fileCount = (int)src.ReadUInt32();
            var files = src.Read<BundleFile>(fileCount);

            var pathRepCount = (int)src.ReadUInt32();
            var pathReps = src.Read<PathRepresentation>(pathRepCount);

            Console.WriteLine($"Files:      {fileCount,-10}");
            Console.WriteLine($"PathReps:   {pathRepCount,-10}");

            var length = src.Length - src.Position;

            var innerBundle = src.ReadAndDecompressBundle();

        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    internal readonly struct PathRepresentation : IEquatable<PathRepresentation>
    {
        public readonly ulong Hash;
        public readonly uint PayloadOffset;
        public readonly uint PayloadSize;
        public readonly uint PayloadRecursiveSize;

        public override bool Equals(object obj) =>
            obj is PathRepresentation representation && Equals(representation);

        public bool Equals(PathRepresentation other) => Hash == other.Hash;
        public override int GetHashCode() => HashCode.Combine(Hash);

        public static bool operator ==(PathRepresentation left, PathRepresentation right) =>
            left.Equals(right);
        public static bool operator !=(PathRepresentation left, PathRepresentation right) =>
            !(left == right);
    }

    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    internal readonly struct BundleFile : IEquatable<BundleFile>
    {
        public readonly ulong Hash;
        public readonly uint BundleIndex;
        public readonly uint FileOffset;
        public readonly uint FileSize;

        public override bool Equals(object obj) => obj is BundleFile file && Equals(file);
        public bool Equals(BundleFile other) => Hash == other.Hash;
        public override int GetHashCode() => HashCode.Combine(Hash);

        public static bool operator ==(BundleFile left, BundleFile right) => left.Equals(right);
        public static bool operator !=(BundleFile left, BundleFile right) => !(left == right);
    }


    internal readonly struct BundleInfo : IEquatable<BundleInfo>
    {
        public BundleInfo(int uncompressedSize, string name)
        {
            UncompressedSize = uncompressedSize;
            Name = name;
        }

        public readonly int UncompressedSize;
        public readonly string Name;

        public override bool Equals(object obj) =>
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
