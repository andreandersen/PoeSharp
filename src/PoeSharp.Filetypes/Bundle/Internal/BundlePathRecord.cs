using Microsoft.Toolkit.HighPerformance;
using Microsoft.Toolkit.HighPerformance.Buffers;

namespace PoeSharp.Filetypes.Bundle.Internal
{
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    internal readonly struct BundlePathRecord : IEquatable<BundlePathRecord>
    {
        public readonly ulong Hash;
        public readonly int PayloadOffset;
        public readonly int PayloadSize;
        public readonly int PayloadRecursiveSize;

        public override bool Equals(object? obj) =>
            obj is BundlePathRecord representation && Equals(representation);

        public bool Equals(BundlePathRecord other) => Hash == other.Hash;
        public override int GetHashCode() => HashCode.Combine(Hash);

        public static bool operator ==(BundlePathRecord left, BundlePathRecord right) =>
            left.Equals(right);
        public static bool operator !=(BundlePathRecord left, BundlePathRecord right) =>
            !(left == right);

        public PathString[] MapFilenames(Span<byte> innerBundle)
        {
            const byte nullTerm = 0;
            var isBasePhase = false;

            var data = innerBundle.Slice(PayloadOffset, PayloadSize);

            isBasePhase = false;
            var bases = new List<string>();
            var results = new List<PathString>();

            while (data.Length > 0)
            {
                var command = data.ConsumeTo<int>();

                if (command == 0)
                {
                    isBasePhase = !isBasePhase;
                    if (isBasePhase)
                    {
                        bases.Clear();
                    }

                    continue;
                }

                var nameLen = data.IndexOf(nullTerm);
                var path = Encoding.ASCII.GetString(data.Slice(0, nameLen));

                data = data[(nameLen + 1)..];

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

            return results.ToArray();
        }
    }
}