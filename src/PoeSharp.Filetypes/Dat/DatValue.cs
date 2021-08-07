using System;
using System.Runtime.CompilerServices;

using PoeSharp.Filetypes.Dat.Specification;

namespace PoeSharp.Filetypes.Dat
{
    internal struct DirectDatValue
    {
        internal static object Create(ref Span<byte> buf, TypeCode tc)
        {
            var v = tc switch
            {
                TypeCode.Boolean => buf.To<bool>(),
                TypeCode.SByte =>   buf.To<sbyte>(),
                TypeCode.Byte =>    buf.To<byte>(),
                TypeCode.Int16 =>   buf.To<short>(),
                TypeCode.UInt16 =>  buf.To<ushort>(),
                TypeCode.Int32 =>   buf.To<int>(),
                TypeCode.UInt32 =>  buf.To<uint>(),
                TypeCode.Int64 =>   buf.To<long>(),
                TypeCode.UInt64 =>  buf.To<ulong>(),
                TypeCode.Single =>  buf.To<float>(),
                TypeCode.Double =>  buf.To<double>(),
                _ => ThrowHelper.NotSupported<object>()
            };

            return v;
        }

        internal static T Create<T>(ref Span<byte> buf)
            where T : unmanaged => buf.To<T>();

        internal static readonly byte[] StringNullTerminator
            = { 0, 0, 0, 0 };

        internal static string CreateString(ref Span<byte> buf, Span<byte> data)
        {
            var offset = buf.To<int>();
            if (offset.IsNullValue() || offset > data.Length)
            {
                return string.Empty;
            }

            var dataBuf = data[offset..];
            var dataLen = dataBuf.IndexOf(StringNullTerminator);

            if (dataLen == -1)
                return string.Empty;

            if (dataLen % 2 != 0)
                dataLen++;

            return dataBuf.Slice(0, dataLen).ToUnicodeText();
        }
    }

    public readonly struct ReferencedDatValue
    {
        public int Value { get; }

        internal ReferencedDatValue(
            ref Span<byte> buf, ReadOnlySpan<byte> data,
            DatFileIndex index, DatField field)
        {
            var offset = buf.ConsumeTo<int>();
            Value = offset;
            //if (offset.IsNullValue())
            //{
            //    Value = -1;
            //    return;
            //}

            //if (field.DatType.IsGenericReference)
            //{
            //    Value = offset;
            //    return;
            //} else
            //{
            //    var sz = Unsafe.SizeOf<T
            //    data.Slice(offset).
            //}
        }
    }

    internal readonly struct ListDatValue
    {
        public static object[] Create(
            ref Span<byte> buf, Span<byte> data,
            DatFileIndex idx, DatField field)
        {
            var tc = field.TypeCode;

            return tc switch
            {
                TypeCode.Boolean => CreateSimple<bool>(ref buf, data),
                TypeCode.SByte =>   CreateSimple<sbyte>(ref buf, data),
                TypeCode.Byte =>    CreateSimple<byte>(ref buf, data),
                TypeCode.Int16 =>   CreateSimple<short>(ref buf, data),
                TypeCode.UInt16 =>  CreateSimple<ushort>(ref buf, data),
                TypeCode.Int32 =>   CreateSimple<int>(ref buf, data),
                TypeCode.UInt32 =>  CreateSimple<uint>(ref buf, data),
                TypeCode.Int64 =>   CreateSimple<long>(ref buf, data),
                TypeCode.UInt64 =>  CreateSimple<ulong>(ref buf, data),
                TypeCode.Single =>  CreateSimple<float>(ref buf, data),
                TypeCode.Double =>  CreateSimple<double>(ref buf, data),
                TypeCode.String =>  CreateString(ref buf, data),
                _ => ThrowHelper.NotSupported<object[]>()
            };
        }

        private static object[] CreateSimple<T>(
            ref Span<byte> buf, Span<byte> data) where T : struct
        {
            var elements = buf.ConsumeTo<int>();
            var offset = buf.To<int>();

            if (elements.IsNullValue() || offset.IsNullValue())
            {
                return Array.Empty<object>();
            }

            var vals = new object[elements];
            var df = data;
            var sz = Unsafe.SizeOf<T>();
            for (var i = 0; i < elements; i++)
            {
                var elBuf = df.Slice(0, sz);
                vals[i] = elBuf.To<T>();
                df = df.Slice(sz);
            }

            return vals;
        }

        private static object[] CreateString(
            ref Span<byte> buf, Span<byte> data)
        {
            var elements = buf.ConsumeTo<int>();
            var offset = buf.To<int>();

            if (elements.IsNullValue() || offset.IsNullValue())
            {
                return Array.Empty<object>();
            }

            var strings = new object[elements];

            var df = data;

            for (var i = 0; i < elements; i++)
            {
                var strLen = df.IndexOf(
                    DirectDatValue.StringNullTerminator);

                if (strLen % 2 != 0)
                    strLen++;

                strings[i] = data
                    .Slice(0, strLen)
                    .ToUnicodeText();

            }

            return strings;
        }
    }

}
