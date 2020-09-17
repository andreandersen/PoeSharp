using System;
using System.Runtime.CompilerServices;
using System.Text;

namespace PoeSharp.Filetypes
{
    public static class ConversionExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe T To<T>(this byte[] buf)
        {
            fixed (byte* b = &buf[0])
            {
                return Unsafe.Read<T>(b);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe T To<T>(this Span<byte> buf)
        {
            fixed (byte* b = &buf[0])
            {
                return Unsafe.Read<T>(b);
            }
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ReadOnlySpan<char> FromUnicodeBytesToUtf8(this in Span<byte> utf16bytes)
        {
            return utf16bytes.Length == 0 ? ReadOnlySpan<char>.Empty : Encoding.Unicode.GetString(utf16bytes);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Span<byte> FromUnicodeBytesToUtf8(this in ReadOnlySpan<byte> utf16bytes) =>
            FromUnicodeBytesToUtf8(utf16bytes);
    }
}
