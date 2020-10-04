using System;

namespace PoeSharp.Filetypes.Bundle.Internal
{
    internal static class Fnv1aHash64
    {
        private const ulong FNV64_PRIME = 1099511628211;
        private const ulong FNV64_OFFSETBASIS = 14695981039346656037;

        public static ulong Hash64(ReadOnlySpan<byte> buffer)
        {
            ulong result = FNV64_OFFSETBASIS;
            foreach (var b in buffer)
            {
                result = FNV64_PRIME * (result ^ b);
            }

            return result;
        }
    }
}
