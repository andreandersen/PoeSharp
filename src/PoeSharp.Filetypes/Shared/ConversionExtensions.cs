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

        public unsafe static T To<T>(this ReadOnlySpan<byte> buf)
        {
            fixed (byte* b = &buf[0])
                return Unsafe.ReadUnaligned<T>(b);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static object To(this Span<byte> buf, Type type)
        {
            if (type == typeof(bool))
                return To<bool>(buf);
            if (type == typeof(byte))
                return To<byte>(buf);
            if (type == typeof(sbyte))
                return To<sbyte>(buf);
            if (type == typeof(short))
                return To<short>(buf);
            if (type == typeof(ushort))
                return To<ushort>(buf);
            if (type == typeof(int))
                return To<int>(buf);
            if (type == typeof(uint))
                return To<uint>(buf);
            if (type == typeof(long))
                return To<long>(buf);
            if (type == typeof(ulong))
                return To<ulong>(buf);
            if (type == typeof(float))
                return To<float>(buf);
            if (type == typeof(double))
                return To<double>(buf);
            if (type == typeof(decimal))
                return To<decimal>(buf);

            throw new Exception("wtf");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe Span<T> Cast<T>(this Span<byte> buf)
        {
            var sizeOf = Unsafe.SizeOf<T>();
            fixed (void* ptr = &buf[0])
            {
                return new Span<T>(ptr, buf.Length / sizeOf);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe Span<T> Cast<T>(this byte[] buf)
        {
            var sizeOf = Unsafe.SizeOf<T>();
            fixed (void* ptr = &buf[0])
            {
                return new Span<T>(ptr, buf.Length / sizeOf);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static unsafe T ToImpl<T>(byte[] buf)
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
