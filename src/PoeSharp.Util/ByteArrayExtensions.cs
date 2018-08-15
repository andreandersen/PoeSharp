using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;

namespace PoeSharp.Util
{
    public static class ConversionExtensions
    {
        private static readonly Encoding UnicodeEncoding = Encoding.Unicode;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T To<T>(this byte[] buf) => ToImpl<T>(buf);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T To<T>(this Span<byte> buf) => ToImplSpan<T>(buf);

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
        private static unsafe T ToImplSpan<T>(Span<byte> buf)
        {
            fixed (byte* b = &buf[0])
                return Unsafe.ReadUnaligned<T>(b);
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
        public static string ToUnicodeText(this Span<byte> bytes) =>
            UnicodeEncoding.GetString(bytes.ToArray());
    }
}
