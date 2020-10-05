using System;
using System.Buffers;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

using Microsoft.Toolkit.HighPerformance.Extensions;

using PoeSharp.Filetypes.Dat.Specification;

namespace PoeSharp.Filetypes.Dat
{
    public class DatRow
    {
        private static readonly byte[] StringNullTerminator = { 0, 0, 0, 0 };

        public ImmutableDictionary<string, DatValue> Columns { get; }

        public DatRow(
            Span<byte> rowData, Span<byte> data,
            DatSpecification spec, DatFileIndex index)
        {
            var buf = rowData;
            foreach (var (name, field) in spec.Fields)
            {
                var dt = field.DatType;
                IDatValue val = (dt.IsList, dt.IsReference) switch
                {
                    (false, false)  when dt.TypeCode is not TypeCode.String => SimpleDatValue.Create(ref buf, dt.TypeCode),
                    (false, false)  when dt.TypeCode is TypeCode.String => new StringDatValue(ref buf),
                    (false, true)   => new ReferencedDatValue(ref buf, data, index, field),
                    (true, _)       => ListDatValue.Create(ref buf, data, index, field),
                    _ => throw new Exception()
                };
            }
        }

        public DatRow(
            Span<byte> rowData, Span<byte> data,
            DatSpecification specification, DatFileIndex index, bool old)
        {

            var dictBuilder = ImmutableDictionary.CreateBuilder<string, DatValue>();
            var d = new Dictionary<string, ValueType>();
            ValueType x = 4;

            var offset = 0;

            foreach (var (k, v) in specification.Fields)
            {
                if (offset >= rowData.Length)
                    break;

                object value;
                if (v.DatType.IsReference)
                {
                    int dataOffset;

                    if (v.DatType.IsList)
                    {
                        var elements = (int)rowData.Slice(offset, 4).To<uint>();
                        offset += 4;

                        if (offset >= rowData.Length)
                            break;

                        dataOffset = (int)rowData.Slice(offset, 4).To<uint>();
                        offset += 4;

                        if (offset.IsNullValue() || elements.IsNullValue())
                        {
                            continue;
                        }

                        value = ReadList(v.DatType.TypeCode, dataOffset,
                            elements, data);
                    }
                    else if (v.DatType.IsGenericReference)
                    {
                        var ptrData = rowData.Slice(offset, 4).To<uint>();
                        value = ptrData.IsNullValue() ? -1 : ptrData;
                        offset += 4;
                    }
                    else
                    {
                        dataOffset = (int)rowData.Slice(offset, 4).To<uint>();
                        offset += 4;

                        if (dataOffset.IsNullValue())
                        {
                            continue;
                        }

                        if (v.DatType.TypeCode == TypeCode.String)
                        {
                            if (dataOffset > data.Length)
                            {
                                continue;
                            }

                            var length = data[dataOffset..].IndexOf(StringNullTerminator);
                            if (length % 2 != 0)
                            {
                                length++;
                            }

                            value = data.Slice(dataOffset, length).ToUnicodeText();
                        }
                        else
                        {
                            value = data.Slice(dataOffset, v.DatType.TypeCode.GetByteSize())
                                .To(v.DatType.TypeCode);
                        }
                    }
                }
                else
                {
                    var size = v.DatType.TypeCode.GetByteSize();
                    value = rowData.Slice(offset, size).To(v.DatType.TypeCode);
                    offset += size;
                }

                var datValue = new DatValue(v, value, index);
                dictBuilder.Add(k, datValue);
            }

            Columns = dictBuilder.ToImmutable();
        }

        private static object[] ReadList(
            TypeCode typeCode, int start,
            int elements, Span<byte> data)
        {
            var ret = new object[elements];

            if (typeCode == TypeCode.String)
            {
                for (var i = 0; i < elements; i++)
                {
                    var length = data[start..].IndexOf(StringNullTerminator);
                    if (length % 2 != 0)
                    {
                        length++;
                    }

                    ret[i] = data.Slice(start, length).ToUnicodeText();
                    start += length;
                }
            }
            else
            {
                var size = typeCode.GetByteSize();

                for (var i = 0; i < elements; i++)
                {
                    ret[i] = data.Slice(start + (i * size), size).To(typeCode);
                }
            }
            return ret;
        }

        public T Value<T>(string key) =>
            (T)Columns[key].Value;

        public object Value(string key) =>
            Columns[key].Value;

    }
}
