using System;
using System.Buffers;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;

using PoeSharp.Filetypes.Dat.Specification;

namespace PoeSharp.Filetypes.Dat
{
    public interface IDatValue
    {
        public const bool INTERN_STRINGS = true;

    }

    internal struct SimpleDatValue
    {
        internal static IDatValue Create(ref Span<byte> buf, TypeCode tc)
        {
            return null;
        }
    }

    public readonly struct SimpleDatValue<T> : IDatValue where T : unmanaged
    {
        public T? Value { get; }
        public TypeCode TypeCode { get; }

        internal SimpleDatValue(ref Span<byte> buf)
        {
            Value = buf.ConsumeTo<T>();
            TypeCode = Type.GetTypeCode(typeof(T));

            if (Value.IsNullValue())
                Value = null;
        }

        public static implicit operator T?(SimpleDatValue<T> dv) => dv.Value;
        public static implicit operator string(SimpleDatValue<T> dv) =>
            dv.Value.HasValue ? dv.Value.ToString() : string.Empty;

    }

    public readonly struct StringDatValue : IDatValue
    {
        public string Value { get; }

        internal StringDatValue(ref Span<byte> buf)
        {
            Value = string.Empty;
        }

    }

    public readonly struct ReferencedDatValue : IDatValue
    {
        public DatRow? Value { get; }

        internal ReferencedDatValue(
            ref Span<byte> buf, ReadOnlySpan<byte> data,
            DatFileIndex index, DatField field)
        {
            Value = null;
        }
    }

    internal readonly struct ListDatValue
    {
        public static IDatValue Create(ref Span<byte> buf, ReadOnlySpan<byte> data, DatFileIndex idx, DatField field)
        {
            return null;
        }
    }

    public readonly struct ListDatValue<T> : IDatValue
    {
        public T[] Value { get; }

        internal ListDatValue(ref Span<byte> buf, ReadOnlySpan<byte> data, DatFileIndex idx, DatField field)
        {
            var elements = buf.ConsumeTo<int>();

            if (elements.IsNullValue())
            {
                Value = Array.Empty<T>();
                return;
            }

            //if (tc == TypeCode.String)
            //{
            //    Value = Array.Empty<T>();
            //    return;
            //}

            Value = Array.Empty<T>();
        }

        public static implicit operator T[](ListDatValue<T> dv) => dv.Value;
    }
}
