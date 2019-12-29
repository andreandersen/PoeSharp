using System;
using System.Buffers.Text;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Utf8;

namespace PoeSharp.Filetypes
{
    public static class ConversionExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe static T To<T>(this byte[] buf)
        {
            fixed (byte* b = &buf[0])
                return Unsafe.ReadUnaligned<T>(b);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe static T To<T>(this Span<byte> buf)
        {
            fixed (byte* b = &buf[0])
                return Unsafe.ReadUnaligned<T>(b);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Span<byte> FromUnicodeBytesToUtf8(this in Span<byte> utf16bytes)
        {
            Encodings.Utf8.FromUtf16Length(utf16bytes, out int bytesNeeded);
            Span<byte> utf8bytes = new byte[bytesNeeded];
            Encodings.Utf8.FromUtf16(utf16bytes, utf8bytes, out _, out _);
            return utf8bytes;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Span<byte> FromUnicodeBytesToUtf8(this in ReadOnlySpan<byte> utf16bytes) =>
            FromUnicodeBytesToUtf8(utf16bytes);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string FromUtf8BytesToString(this in ReadOnlySpan<byte> utf8Bytes) =>
            new Utf8Span(utf8Bytes).ToString();
    }
}
