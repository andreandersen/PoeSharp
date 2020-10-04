using System;
using System.Text;

namespace PoeSharp.Filetypes.Bundle.Internal
{
    public readonly struct PathString : IEquatable<PathString>
    {
        public PathString(string path)
        {
            Path = path;

            var computePath = path
                .ToLowerInvariant()
                .TrimEnd('/') + "++";

            var toHash = Encoding.UTF8
                .GetBytes(computePath).AsSpan();

            Hash = Fnv1aHash64.Hash64(toHash);
        }

        public readonly ulong Hash;
        public readonly string Path;

        public bool Equals(PathString other) => 
            Hash == other.Hash;

        public override bool Equals(object obj) => 
            obj is PathString other && Equals(other);

        public override int GetHashCode() => Hash.GetHashCode();

        public static bool operator ==(PathString left, PathString right) => 
            left.Equals(right);

        public static bool operator !=(PathString left, PathString right) => 
            !left.Equals(right);
    }
}