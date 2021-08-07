using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

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
            buf = buf.Slice(size);
            return ret;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Span<T> Consume<T>(this ref Span<T> buf, int length) where T : unmanaged
        {
            var ret = buf.Slice(0, length);
            buf = buf.Slice(length);
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
            buf = buf.Slice(elements * size);
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






        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        //public static object To(this Span<byte> buf, TypeCode type)
        //{
        //    var ret = type switch
        //    {
        //        TypeCode.Boolean => buf.To<bool>(),
        //        TypeCode.Byte => buf.To<byte>(),
        //        TypeCode.SByte => buf.To<sbyte>(),
        //        TypeCode.Int16 => buf.To<short>(),
        //        TypeCode.UInt16 => buf.To<ushort>(),
        //        TypeCode.Int32 => buf.To<int>(),
        //        TypeCode.UInt32 => buf.To<uint>(),
        //        TypeCode.Int64 => buf.To<long>(),
        //        TypeCode.UInt64 => buf.To<ulong>(),
        //        TypeCode.Single => buf.To<float>(),
        //        TypeCode.Double => buf.To<double>(),
        //        TypeCode.Decimal => buf.To<decimal>(),
        //        _ => ThrowHelper.NotSupported<object>()
        //    };

        //    return ret;
        //}
    }
}
