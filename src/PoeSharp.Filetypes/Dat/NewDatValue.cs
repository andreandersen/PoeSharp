using System;
using System.Runtime.CompilerServices;

using PoeSharp.Filetypes.Dat.Specification;

namespace PoeSharp.Filetypes.Dat
{
    public interface IDatValue
    {
        public const bool INTERN_STRINGS = true;
    }

    internal class SimpleDatValue
    {
        internal static IDatValue Create(ref Span<byte> buf, TypeCode tc)
        {
            var v = tc switch
            {
                TypeCode.Boolean => new SimpleDatValue<bool>(ref buf),
                TypeCode.SByte => new SimpleDatValue<sbyte>(ref buf),
                TypeCode.Byte => new SimpleDatValue<byte>(ref buf),
                TypeCode.Int16 => new SimpleDatValue<short>(ref buf),
                TypeCode.UInt16 => new SimpleDatValue<ushort>(ref buf),
                TypeCode.Int32 => new SimpleDatValue<int>(ref buf),
                TypeCode.UInt32 => new SimpleDatValue<uint>(ref buf),
                TypeCode.Int64 => new SimpleDatValue<long>(ref buf),
                TypeCode.UInt64 => new SimpleDatValue<ulong>(ref buf),
                TypeCode.Single => new SimpleDatValue<float>(ref buf),
                TypeCode.Double => new SimpleDatValue<double>(ref buf),
                _ => ThrowHelper.NotSupported<IDatValue>()
            };

            return v;
        }
    }

    public readonly struct SimpleDatValue<T> : IDatValue where T : unmanaged
    {
        public T? Value { get; }
        public TypeCode TypeCode { get; }

        public SimpleDatValue(ref Span<byte> buf)
        {
            Value = buf.ConsumeTo<T>();
            TypeCode = Type.GetTypeCode(typeof(T));

            if (Value.IsNullValue())
                Value = null;
        }

        public static implicit operator T?(SimpleDatValue<T> dv) => dv.Value;
        public static implicit operator string(SimpleDatValue<T> dv)
        {
            var str = dv.Value.HasValue ?
                dv.Value.Value.ToString() :
                string.Empty;

            return str!;
        }
    }


    public readonly struct StringDatValue : IDatValue
    {
        internal static readonly byte[] StringNullTerminator
            = { 0, 0, 0, 0 };

        public string Value { get; }

        internal StringDatValue(ref Span<byte> buf, Span<byte> data)
        {
            var offset = buf.ConsumeTo<int>();
            if (offset.IsNullValue() || offset > data.Length)
            {
                Value = string.Empty;
                return;
            }

            var dataBuf = data.Slice(offset);
            var dataLen = dataBuf.IndexOf(StringNullTerminator);

            if (dataLen == -1)
            {
                Value = dataBuf.ToUnicodeText();
                return;
            }

            Value = dataBuf.Slice(0, dataLen).ToUnicodeText();
        }

        public static implicit operator string(StringDatValue dv) => dv.Value;
        public override string ToString() => Value.ToString();
    }

    public readonly struct ReferencedDatValue : IDatValue
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
        public static IDatValue Create(
            ref Span<byte> buf, ReadOnlySpan<byte> data,
            DatFileIndex idx, DatField field)
        {
            var tc = field.DatType.TypeCode;
            var v = tc switch
            {
                TypeCode.Boolean => new ListDatValue<bool>(ref buf, data, idx, field),
                TypeCode.SByte => new ListDatValue<sbyte>(ref buf, data, idx, field),
                TypeCode.Byte => new ListDatValue<byte>(ref buf, data, idx, field),
                TypeCode.Int16 => new ListDatValue<short>(ref buf, data, idx, field),
                TypeCode.UInt16 => new ListDatValue<ushort>(ref buf, data, idx, field),
                TypeCode.Int32 => new ListDatValue<int>(ref buf, data, idx, field),
                TypeCode.UInt32 => new ListDatValue<uint>(ref buf, data, idx, field),
                TypeCode.Int64 => new ListDatValue<long>(ref buf, data, idx, field),
                TypeCode.UInt64 => new ListDatValue<ulong>(ref buf, data, idx, field),
                TypeCode.Single => new ListDatValue<float>(ref buf, data, idx, field),
                TypeCode.Double => new ListDatValue<double>(ref buf, data, idx, field),
                TypeCode.String => new StringListDatValue(ref buf, data),
                _ => ThrowHelper.NotSupported<IDatValue>()
            };

            return v;
        }
    }

    public readonly struct StringListDatValue : IDatValue
    {
        public string[] Value { get; }

        public StringListDatValue(ref Span<byte> buf, ReadOnlySpan<byte> data)
        {
            var elements = buf.ConsumeTo<int>();
            var offset = buf.ConsumeTo<int>();

            if (elements.IsNullValue() || offset.IsNullValue())
            {
                Value = Array.Empty<string>();
                return;
            }

            Value = new string[elements];

            var df = data;

            for (var i = 0; i < elements; i++)
            {
                var strLen = df.IndexOf(StringDatValue.StringNullTerminator);

                if (strLen % 2 != 0)
                    strLen++;

                Value[i] = data.Slice(0, strLen).ToUnicodeText();
            }
        }
    }

    public readonly struct ListDatValue<T> : IDatValue
    {
        public T[] Value { get; }

        internal ListDatValue(
            ref Span<byte> buf, ReadOnlySpan<byte> data,
            DatFileIndex idx, DatField field)
        {
            var elements = buf.ConsumeTo<int>();
            var offset = buf.ConsumeTo<int>();

            if (elements.IsNullValue() || offset.IsNullValue())
            {
                Value = Array.Empty<T>();
                return;
            }

            Value = new T[elements];

            var df = data;
            var sz = Unsafe.SizeOf<T>();
            for (var i = 0; i < elements; i++)
            {
                var elBuf = df.Slice(0, sz);
                Value[i] = df.To<T>();
                df = df.Slice(sz);
            }
        }

        public static implicit operator T[](ListDatValue<T> dv) => dv.Value;
    }
}
