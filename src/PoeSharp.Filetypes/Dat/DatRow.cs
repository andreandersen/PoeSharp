using System;
using System.Collections.Generic;
using System.Linq;

using PoeSharp.Filetypes.BuildingBlocks;
using PoeSharp.Filetypes.Dat.Specification;

namespace PoeSharp.Filetypes.Dat
{
    public class DatRow : ReadOnlyDictionaryBase<string, DatValue>
    {
        private static readonly byte[] StringNullTerminator = { 0, 0, 0, 0 };

        public DatRow(
            Span<byte> rowData, Span<byte> data,
            DatSpecification specification, DatFileIndex index)
        {
            Underlying = new Dictionary<string, DatValue>();
            var offset = 0;

            var totalLength = specification.Fields.Sum(p => p.Value.DatType.TypeCode.GetByteSize());

            foreach (var field in specification.Fields)
            {
                var k = field.Key;
                var v = field.Value;

                object value;
                if (v.DatType.IsReference)
                {
                    int dataOffset;

                    if (v.DatType.IsList)
                    {
                        var elements = (int)rowData.Slice(offset, 4).To<uint>();
                        offset += 4;
                        dataOffset = (int)rowData.Slice(offset, 4).To<uint>();
                        offset += 4;

                        if (offset.IsNullValue() || elements.IsNullValue())
                        {
                            value = Array.Empty<object>();
                            continue;
                        }

                        value = ReadList(v.DatType.TypeCode, dataOffset,
                            elements, data);
                    }
                    else
                    {
                        dataOffset = (int)rowData.Slice(offset, 4).To<uint>();
                        offset += 4;

                        if (dataOffset.IsNullValue())
                        {
                            value = null;
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
                Underlying.Add(k, datValue);
            }
        }

        private static object[] ReadList(
            TypeCode typeCode, int start,
            int elements,Span<byte> data)
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
            (T)Underlying[key].Value;

        public object Value(string key) =>
            Underlying[key].Value;

    }
}
