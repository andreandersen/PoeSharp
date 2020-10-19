using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security.Cryptography;

using Microsoft.Toolkit.HighPerformance.Extensions;

namespace PoeSharp.Filetypes.Dat
{
    public partial class DatFile
    {
        public readonly struct DatRow
        {
            private readonly DatFile _parent;
            private readonly int _rowIndex;

            public DatRow(DatFile parent, int rowIndex)
            {
                _parent = parent;
                _rowIndex = rowIndex;
            }

            /// <summary>
            /// Use this for simple values, such as short/ushort, int/uint etc
            /// </summary>
            /// <typeparam name="T">Type of your column</typeparam>
            /// <param name="col">Column name</param>
            /// <returns>The data with your type T</returns>
            public T GetPrimitive<T>(string col) where T : unmanaged
            {
                var fld = _parent.Spec.Fields[col];
                return _parent.Bytes
                    .AsSpan()
                    .Slice(fld.Offset + (_rowIndex * _parent.RowSize), fld.Size)
                    .To<T>();
            }

            /// <summary>
            /// Use this for your ref|short, ref|int etc
            /// </summary>
            /// <typeparam name="T">Type of your column, e.g. short, int</typeparam>
            /// <param name="col">Column name</param>
            /// <returns>The array of data with your type T</returns>
            public T[] GetPrimitiveList<T>(string col) where T : unmanaged
            {
                var fld = _parent.Spec.Fields[col];
                var bytes = _parent.Bytes.AsSpan();
                var buf = bytes.Slice(fld.Offset + (_rowIndex * _parent.RowSize), fld.Size);
                var elements = buf.ConsumeTo<int>();
                var offset = buf.To<int>();

                var data = bytes.Slice(_parent.DataOffset + offset, elements * Unsafe.SizeOf<T>());
                var ret = data.Cast<byte, T>();
                return ret.ToArray();
            }

            /// <summary>
            /// Used to find the end of strings in the data chunk
            /// </summary>
            internal static readonly byte[] StringNullTerminator
                = { 0, 0, 0, 0 };

            /// <summary>
            /// Use this for your single strings
            /// </summary>
            /// <param name="col">Column name</param>
            /// <returns>String</returns>
            public string GetString(string col)
            {
                var fld = _parent.Spec.Fields[col];
                var bytes = _parent.Bytes.AsSpan();
                var buf = bytes.Slice(fld.Offset + (_rowIndex * _parent.RowSize), fld.Size);
                var offset = buf.To<int>();
                var data = bytes.Slice(_parent.DataOffset);
                
                // TODO: Should we perhaps throw on offset > data.Length?
                if (offset.IsNullValue() || offset > data.Length)
                    return string.Empty;

                var dataBuf = data.Slice(offset);
                var dataLen = dataBuf.IndexOf(StringNullTerminator);

                if (dataLen == -1)
                    return string.Empty;

                if (dataLen % 2 != 0)
                    dataLen++;

                return dataBuf.Slice(0, dataLen).ToUnicodeText();
            }

            /// <summary>
            /// Use this for your string arrays
            /// </summary>
            /// <param name="col">Column name</param>
            /// <returns>Array of Strings</returns>
            public string[] GetStringList(string col)
            {
                var fld = _parent.Spec.Fields[col];
                var bytes = _parent.Bytes.AsSpan();
                var buf = bytes.Slice(fld.Offset + (_rowIndex * _parent.RowSize), fld.Size);

                var elements = buf.ConsumeTo<int>();
                var offset = buf.To<int>();

                if (elements.IsNullValue() || offset.IsNullValue())
                    return Array.Empty<string>();
                
                var data = bytes.Slice(_parent.DataOffset);

                var dataOffsets = data.Slice(offset, 4 * elements);
                var offsets = dataOffsets.Cast<byte, int>();

                var strings = new string[elements];

                for (int i = 0; i < elements; i++)
                {
                    var df = data.Slice(offsets[i]);
                    var strLen = df.IndexOf(DirectDatValue.StringNullTerminator);
                    if (strLen % 2 != 0)
                        strLen++;

                    strings[i] = df.Slice(0, strLen).ToUnicodeText();
                }

                return strings;
            }

            /// <summary>
            /// Use this for your e.g ref|int or ref|generic
            /// </summary>
            /// <param name="col">Column name</param>
            /// <returns>True if successful</returns>
            public bool TryGetReferencedDatValue(string col, out DatRow row)
            {
                var fld = _parent.Spec.Fields[col];

                // Key is required if not generic
                if (string.IsNullOrEmpty(fld.Key) && !fld.IsGenericReference)
                {
                    Debug.Fail("Field is not a reference");
                    row = default;
                    return false;
                }

                // Should use the GetReferencesListValues instead
                if (fld.IsList)
                {
                    Debug.Fail("Field is a list.");
                    row = default;
                    return false;
                }

                if (fld.TypeCode == TypeCode.String)
                {
                    Debug.Fail(
                        "Field is a string, which is not supported by this method. " +
                        "Use GetString and reference manually");
                    row = default;
                    return false;
                }

                var rowIdx = fld.TypeCode switch
                {
                    TypeCode.Int32 or TypeCode.UInt32 => GetPrimitive<int>(col),
                    TypeCode.Int64 or TypeCode.UInt64 => GetPrimitive<long>(col),
                    _ => -1
                };

                if (rowIdx < 0 || rowIdx.IsNullValue())
                {
                    Debug.Fail("Wrong kind of type used, or null value found");
                    row = default;
                    return false;
                }

                DatFile refDat;

                if (fld.IsGenericReference)
                { 
                    refDat = _parent;
                }
                else if (!_parent.Index.TryGetValue(fld.Key, out refDat))
                {
                    Debug.Fail("Dat file not found");
                    row = default;
                    return false;
                }

                if (refDat.Count < rowIdx)
                {
                    Debug.Fail("Dat file's count is lower than referenced index");
                    row = default;
                    return false;
                }

                row = refDat[(int)rowIdx];
                return true;
            }

            /// <summary>
            /// Use this for your ref|list|int where Key is present 
            /// or for your ref|list|generic
            /// </summary>
            /// <param name="col">Column name</param>
            /// <returns>True if successful</returns>
            public bool TryGetReferencedDatValueList(string col, out DatRow[] rows)
            {
                var fld = _parent.Spec.Fields[col];

                // Key is required if not generic
                if (string.IsNullOrEmpty(fld.Key) && !fld.IsGenericReference)
                {
                    Debug.Fail("Field is not a reference");
                    rows = Array.Empty<DatRow>();
                    return false;
                }

                // Should use the GetReferencesListValues instead
                if (!fld.IsList)
                {
                    Debug.Fail("Field is not a list.");
                    rows = Array.Empty<DatRow>();
                    return false;
                }

                if (fld.TypeCode == TypeCode.String)
                {
                    Debug.Fail(
                        "Field is a string, which is not supported by this method. " +
                        "Use GetString and reference manually");
                    rows = Array.Empty<DatRow>();
                    return false;
                }

                DatFile refDat;

                if (fld.IsGenericReference)
                {
                    refDat = _parent;
                }
                else if (!_parent.Index.TryGetValue(fld.Key, out refDat))
                {
                    Debug.Fail("Dat file not found");
                    rows = Array.Empty<DatRow>();
                    return false;
                }

                var ret = fld.TypeCode switch
                {
                    TypeCode.Int64 or TypeCode.UInt64 => GetRowsL(GetPrimitiveList<long>(col)),
                    TypeCode.Int32 or TypeCode.UInt32 => GetRowsI(GetPrimitiveList<int>(col)),
                    _ => null
                };

                rows = ret ?? Array.Empty<DatRow>();
                return ret != null;

                DatRow[] GetRowsI(int[] indexes)
                {
                    var c = indexes.Length;
                    var rowsRet = new DatRow[c];
                    for (int i = 0; i < c; i++)
                    {
                        rowsRet[i] = refDat[indexes[i]];
                    }
                    return rowsRet;
                }

                DatRow[] GetRowsL(long[] indexes)
                {
                    var c = indexes.Length;
                    var rowsRet = new DatRow[c];
                    for (int i = 0; i < c; i++)
                    {
                        rowsRet[i] = refDat[(int)indexes[i]];
                    }
                    return rowsRet;
                }

            }

            /// <summary>
            /// WARNING: This currently does not behave the same way as the Get/TryGet methods!
            /// This is your simple boxed version of getting a column. 
            /// It will allocate and you will pay the box/unboxing penalty. 
            /// But hey, it's simple, and all that.
            /// </summary>
            /// <param name="col">Column name as per the Dat Specification</param>
            /// <returns>A boxed value your what you asked for</returns>
            public object this[string col]
            {
                get
                {
                    var fld = _parent.Spec.Fields[col];
                    var bytes = _parent.Bytes.AsSpan();
                    var buf = bytes[(fld.Offset + (_rowIndex * _parent.RowSize))..];
                    var data = bytes[_parent.DataOffset..];

                    var val = (fld.IsList, fld.IsReference) switch
                    {
                        (false, false) when fld.TypeCode is not TypeCode.String => DirectDatValue.Create(ref buf, fld.TypeCode),
                        (false, true) when fld.TypeCode is TypeCode.String => DirectDatValue.CreateString(ref buf, data),
                        (false, true) => new ReferencedDatValue(ref buf, data, _parent.Index, fld),
                        (true, _) => ListDatValue.Create(ref buf, data, _parent.Index, fld),
                        _ => ThrowHelper.NotSupported<object>()
                    };

                    return val;
                }
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            private Specification.DatField GetFieldForColumn(string col) => _parent.Spec.Fields[col];
        }
    }
}
