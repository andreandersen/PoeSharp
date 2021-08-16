namespace PoeSharp.Filetypes.Bundle.Internal
{
    internal static class Fnv1aHash64
    {
        private const ulong FNV64_PRIME = 1099511628211;
        private const ulong FNV64_OFFSETBASIS = 14695981039346656037;

        public static ulong Hash64(ReadOnlySpan<byte> buffer)
        {
            var result = FNV64_OFFSETBASIS;
            foreach (var b in buffer)
            {
                result = FNV64_PRIME * (result ^ b);
            }

            return result;
        }
    }

    internal static class Fnv1aHash64Extension
    {
        public static ulong FnvHash(this ReadOnlySpan<char> str) =>
            FnvHashImpl(str);

        public static ulong FnvHash(this string str) =>
            FnvHashImpl(str);

        private static ulong FnvHashImpl(ReadOnlySpan<char> str)
        {
            var len = str.Length;
            if (str[len - 1] == '/')
                len += 1;
            else
                len += 2;

            Span<char> buf = stackalloc char[len];
            str.ToLowerInvariant(buf);
            buf[len - 1] = '+';
            buf[len - 2] = '+';

            var blen = Encoding.UTF8.GetByteCount(buf);
            Span<byte> bytes = stackalloc byte[blen];
            Encoding.UTF8.GetBytes(buf, bytes);

            return Fnv1aHash64.Hash64(bytes);
        }
    }
}