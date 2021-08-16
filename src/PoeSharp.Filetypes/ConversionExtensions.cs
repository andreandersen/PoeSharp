namespace PoeSharp.Filetypes
{
    public static class ConversionExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T To<T>(this Span<byte> buf) where T : struct =>
            Unsafe.As<byte, T>(ref buf[0]);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T To<T>(this ReadOnlySpan<byte> buf) where T : struct =>
            Unsafe.As<byte, T>(ref MemoryMarshal.GetReference(buf));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T ConsumeTo<T>(this ref Span<byte> buf)
        {
            var size = Unsafe.SizeOf<T>();
            var t = buf.Slice(0, size);
            var ret = Unsafe.As<byte, T>(ref t[0]);
            buf = buf[size..];
            return ret;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Span<T> Consume<T>(
            this ref Span<T> buf, int length) 
            where T : unmanaged
        {
            var ret = buf.Slice(0, length);
            buf = buf[length..];
            return ret;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Span<T> ConsumeTo<T>(
            this ref Span<byte> buf, int elements)
            where T : unmanaged
        {
            var size = Unsafe.SizeOf<T>();
            var t = buf.Slice(0, elements * size);
            var ret = MemoryMarshal.Cast<byte, T>(t);
            buf = buf[(elements * size)..];
            return ret;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ReadOnlySpan<T> As<T>(
            this ReadOnlySpan<byte> buf, int elements)
            where T : unmanaged
        {
            var size = Unsafe.SizeOf<T>();
            var t = buf.Slice(0, elements * size);
            var ret = MemoryMarshal.Cast<byte, T>(t);
            buf = buf[(elements * size)..];
            return ret;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ReadOnlySpan<char> FromUnicodeBytesToUtf8
            (this in Span<byte> utf16bytes) => utf16bytes.Length == 0 ?
                ReadOnlySpan<char>.Empty :
                Encoding.Unicode.GetString(utf16bytes);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string ToUnicodeText(this Span<byte> bytes) =>
            Encoding.Unicode.GetString(bytes);
    }
}