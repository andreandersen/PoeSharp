﻿using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace PoeSharp.Filetypes
{
    public static class ConversionExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T To<T>(this Span<byte> buf) => Unsafe.As<byte, T>(ref buf[0]);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T To<T>(this ReadOnlySpan<byte> buf)
        {
            return Unsafe.As<byte, T>(ref MemoryMarshal.GetReference(buf));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T ConsumeTo<T>(this ref Span<byte> buf)
        {
            var size = Unsafe.SizeOf<T>();
            var t = buf.Slice(0, size);
            var ret = Unsafe.As<byte, T>(ref t[0]);
            buf = buf.Slice(size);
            return ret;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static object To(this Span<byte> buf, TypeCode type)
        {
            object ret = type switch
            {
                TypeCode.Boolean => buf.To<bool>(),
                TypeCode.Byte => buf.To<byte>(),
                TypeCode.SByte => buf.To<sbyte>(),
                TypeCode.Int16 => buf.To<short>(),
                TypeCode.UInt16 => buf.To<ushort>(),
                TypeCode.Int32 => buf.To<int>(),
                TypeCode.UInt32 => buf.To<uint>(),
                TypeCode.Int64 => buf.To<long>(),
                TypeCode.UInt64 => buf.To<ulong>(),
                TypeCode.Single => buf.To<float>(),
                TypeCode.Double => buf.To<double>(),
                TypeCode.Decimal => buf.To<decimal>(),
                _ => throw new NotImplementedException()
            };

            return ret;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ReadOnlySpan<char> FromUnicodeBytesToUtf8
            (this in Span<byte> utf16bytes) => utf16bytes.Length == 0 ?
                ReadOnlySpan<char>.Empty :
                Encoding.Unicode.GetString(utf16bytes);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Span<byte> FromUnicodeBytesToUtf8(
            this in ReadOnlySpan<byte> utf16bytes) =>
            FromUnicodeBytesToUtf8(utf16bytes);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string ToUnicodeText(this Span<byte> bytes) =>
            Encoding.Unicode.GetString(bytes);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string ToUnicodeText(this ReadOnlySpan<byte> bytes) =>
            Encoding.Unicode.GetString(bytes);
    }
}