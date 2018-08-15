using System;
using System.Collections.Generic;
using PoeSharp.Files.Dat.Specification;
using PoeSharp.Shared;
using PoeSharp.Util;

namespace PoeSharp.Files.Dat
{
    public class DatRow : ReadOnlyDictionaryBase<string, DatValue>
    {
        private static readonly byte[] StringNullTerminator = { 0, 0, 0, 0 };

    public DatRow(ref Span<byte> rowData, ref Span<byte> data,
            DatSpecification specification, DatFileIndex index)
        {
            Underlying = new Dictionary<string, DatValue>();
            var offset = 0;

            foreach (var field in specification.Fields)
            {
                var k = field.Key;
                var v = field.Value;

                object value;
                if (v.ClrType.IsReference)
                {
                    int dataOffset;

                    if (v.ClrType.IsList)
                    {
                        var elements = (int)rowData.Slice(offset, 4).To<uint>();
                        offset += 4;
                        dataOffset = (int)rowData.Slice(offset, 4).To<uint>();
                        offset += 4;

                        value = ReadList(v.ClrType.DefinedType, dataOffset,
                            elements, ref data);
                    }
                    else
                    {
                        dataOffset = (int)rowData.Slice(offset, 4).To<uint>();
                        offset += 4;

                        if (v.ClrType.DefinedType == typeof(string))
                        {
                            if (dataOffset > data.Length)
                            {
                                continue;
                            }

                            var length = data.Slice(dataOffset).IndexOf(StringNullTerminator);
                            if (length % 2 != 0)
                            {
                                length++;
                            }

                            value = data.Slice(dataOffset, length).ToUnicodeText();
                        }
                        else
                        {
                            value = data.Slice(dataOffset, SizeTable[v.ClrType.DefinedType])
                                .To(v.ClrType.DefinedType);
                        }
                    }
                }
                else
                {
                    var size = SizeTable[v.ClrType.DefinedType];
                    value = rowData.Slice(offset, size).To(v.ClrType.DefinedType);
                    offset += size;
                }

                var datValue = new DatValue(v, value, index);
                Underlying.Add(k, datValue);
            }
        }

        private static object[] ReadList(Type type, int start, int elements, ref Span<byte> data)
        {
            var ret = new object[elements];

            if (type == typeof(string))
            {
                for (int i = 0; i < elements; i++)
                {
                    var length = data.Slice(start).IndexOf(StringNullTerminator);
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
                var size = SizeTable[type];

                for (int i = 0; i < elements; i++)
                {
                    ret[i] = data.Slice(start + (i * size), size).To(type);
                }
            }
            return ret;
        }

        public T Value<T>(string key) =>
            (T)Underlying[key].Value;

        public object Value(string key) =>
            Underlying[key].Value;

        private static readonly Dictionary<Type, int> SizeTable =
            new Dictionary<Type, int>
            {
                { typeof(bool), sizeof(bool) },
                { typeof(byte), sizeof(byte) },
                { typeof(sbyte), sizeof(sbyte) },
                { typeof(short), sizeof(short) },
                { typeof(ushort), sizeof(ushort) },
                { typeof(int), sizeof(int) },
                { typeof(uint), sizeof(uint) },
                { typeof(long), sizeof(long) },
                { typeof(ulong), sizeof(ulong) },
                { typeof(float), sizeof(float) },
                { typeof(double), sizeof(double) },
                { typeof(decimal), sizeof(decimal) }
            };
    }
}
