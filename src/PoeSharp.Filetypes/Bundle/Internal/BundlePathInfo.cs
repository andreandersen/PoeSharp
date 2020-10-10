using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Runtime.InteropServices;
using System.Text;

namespace PoeSharp.Filetypes.Bundle.Internal
{
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    internal readonly struct BundlePathInfo : IEquatable<BundlePathInfo>
    {
        public readonly ulong Hash;
        public readonly int PayloadOffset;
        public readonly int PayloadSize;
        public readonly int PayloadRecursiveSize;

        public override bool Equals(object? obj) =>
            obj is BundlePathInfo representation && Equals(representation);

        public bool Equals(BundlePathInfo other) => Hash == other.Hash;
        public override int GetHashCode() => HashCode.Combine(Hash);

        public static bool operator ==(BundlePathInfo left, BundlePathInfo right) =>
            left.Equals(right);
        public static bool operator !=(BundlePathInfo left, BundlePathInfo right) =>
            !(left == right);

        public ImmutableArray<PathString> MapFilenames(Span<byte> data)
        {
            const byte nullTerm = 0;
            var isBasePhase = false;

            var d = data[PayloadOffset..(PayloadOffset + PayloadSize)];
            
            var bases = new List<string>();
            var results = new List<PathString>();

            while (d.Length > 0)
            {
                var command = d[..4].To<int>();
                d = d[4..];
                
                if (command == 0)
                {
                    isBasePhase = !isBasePhase;
                    if (isBasePhase)
                        bases.Clear();

                    continue;
                }

                var nameLen = d.IndexOf(nullTerm);
                var path = Encoding.ASCII.GetString(d[..nameLen]);

                d = d[(nameLen + 1)..];
                var index = command - 1;
                if (index < bases.Count)
                {
                    path = $"{bases[index]}{path}";
                }

                if (isBasePhase)
                {
                    bases.Add(path);
                }
                else
                {
                    results.Add(new PathString(path));
                }
            }

            return results.ToImmutableArray();
        }
    }
}
