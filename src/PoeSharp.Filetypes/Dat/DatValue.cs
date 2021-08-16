namespace PoeSharp.Filetypes.Dat
{
    public readonly ref struct DatValue
    {
        internal static readonly byte[] StringNullTerminator
            = { 0, 0, 0, 0 };

        private readonly DatRow _row;
        private readonly ColumnDefinition _columnDefinition;
        public ReadOnlySpan<byte> Data { get; }

        public DatValue(in DatRow row, ReadOnlySpan<byte> data, ColumnDefinition colDef)
        {
            _row = row;
            _columnDefinition = colDef;
            Data = data;
        }

        public T GetPrimitive<T>() where T : unmanaged => Data.To<T>();

        public T[] GetPrimitiveArray<T>() where T : unmanaged
        {
            var elementOffset = GetArrayDetails();
            if (!elementOffset.HasValue)
                return Array.Empty<T>();

            (var elements, var offset) = elementOffset.Value;

            return _row.Parent.ContentData
                .Slice(offset, Unsafe.SizeOf<T>() * elements)
                .Cast<byte, T>().ToArray();
        }

        public string GetString()
        {
            var offset = _row.Parent.Is64Bit ?
                Data.Slice(0, 8).To<long>() : Data.To<int>();

            if (offset.IsNullValue())
                return string.Empty;

            var dataBuf = _row.Parent.ContentData[(int)offset..];

            return ExtractStringFromBytes(dataBuf);
        }

        public string[] GetStringArray()
        {
            var elementOffset = GetArrayDetails();
            if (!elementOffset.HasValue)
                return Array.Empty<string>();

            (var elements, var offset) = elementOffset.Value;

            var result = new string[elements];

            var buf = _row.Parent.ContentData;

            if (_row.Parent.Is64Bit)
            {
                var stringOffsets = buf
                    .Slice(offset, elements * 8)
                    .As<long>(elements);

                for (int i = 0; i < elements; i++)
                {
                    result[i] = ExtractStringFromBytes(
                        buf.Slice((int)stringOffsets[i]));
                }
            }
            else
            {
                var stringOffsets = buf
                    .Slice(offset, elements * 4)
                    .As<int>(elements);

                for (int i = 0; i < elements; i++)
                {
                    result[i] = ExtractStringFromBytes(
                        buf.Slice(stringOffsets[i]));
                }
            }

            return result;
        }

        public DatReference? GetReference()
        {
            long val = this;
            if (val.IsNullValue())
                return null;

            var foreign = _columnDefinition.Type == ColumnType.ForeignRow;
            return new DatReference((int)val,
                _row,
                _columnDefinition.Reference,
                foreign);
        }

        public DatReference[] GetReferenceArray()
        {
            var elementOffset = GetArrayDetails();
            if (!elementOffset.HasValue)
                return Array.Empty<DatReference>();

            (var elements, var offset) = elementOffset.Value;

            var foreign = _columnDefinition.Type == ColumnType.ForeignRow;
            var values = new DatReference[elements];

            var data = _row.Parent.ContentData[offset..];
            var is64 = _row.Parent.Is64Bit;

            for (var i = 0; i < elements; i++)
            {
                var rowIndex = is64 ?
                    data.Slice(i * 16, 16).Slice(0, 8).To<long>() :
                    data.Slice(i * 8, 8).To<long>();

                values[i] = new DatReference(
                    (int)rowIndex, _row,
                    _columnDefinition.Reference, foreign);
            }

            return values;
        }

        private (int Elements, int Offset)? GetArrayDetails()
        {
            int elements;
            int offset;

            if (_row.Parent.Is64Bit)
            {
                var lngElements = Data.Slice(0, 8).To<long>();
                var lngOffset = Data.Slice(8, 8).To<long>();
                if (lngElements.IsNullValue()
                    || lngOffset.IsNullValue()
                    || lngElements > int.MaxValue
                    || lngOffset > int.MaxValue
                    || lngElements < int.MinValue
                    || lngOffset < int.MinValue
                    || lngElements == 0)
                {
                    return null;
                }

                elements = (int)lngElements;
                offset = (int)lngOffset;
            }
            else
            {
                elements = Data.Slice(0, 4).To<int>();
                offset = Data.Slice(4, 4).To<int>();
                if (elements.IsNullValue()
                    || offset.IsNullValue()
                    || elements == 0)
                    return null;
            }

            return (elements, offset);
        }

        private string ExtractStringFromBytes(
            ReadOnlySpan<byte> dataBuf)
        {
            var strLen = dataBuf.IndexOf(StringNullTerminator);
            if (_row.Parent.DatType is DatType.Dat32 or DatType.Dat64)
            {
                if (strLen % 2 != 0)
                    strLen++;

                return Encoding.Unicode.GetString(dataBuf[..strLen]);
            }
            else
            {
                if (strLen % 4 != 0)
                    strLen += 3;

                return Encoding.UTF32.GetString(dataBuf[..strLen]);
            }
        }

        public static implicit operator string(DatValue datVal) => datVal.GetString();
        public static implicit operator ReadOnlySpan<char>(DatValue datVal) => datVal.GetString();
        public static implicit operator bool(DatValue datVal) => datVal.GetPrimitive<bool>();
        public static implicit operator short(DatValue datVal) => datVal.GetPrimitive<short>();
        public static implicit operator ushort(DatValue datVal) => datVal.GetPrimitive<ushort>();
        public static implicit operator int(DatValue datVal) => datVal.GetPrimitive<int>();
        public static implicit operator uint(DatValue datVal) => datVal.GetPrimitive<uint>();
        public static implicit operator long(DatValue datVal) => datVal.GetPrimitive<long>();
        public static implicit operator ulong(DatValue datVal) => datVal.GetPrimitive<ulong>();
        public static implicit operator float(DatValue datVal) => datVal.GetPrimitive<float>();
        public static implicit operator double(DatValue datVal) => datVal.GetPrimitive<double>();

        public static implicit operator bool[](DatValue datVal) => datVal.GetPrimitiveArray<bool>();
        public static implicit operator short[](DatValue datVal) => datVal.GetPrimitiveArray<short>();
        public static implicit operator ushort[](DatValue datVal) => datVal.GetPrimitiveArray<ushort>();
        public static implicit operator int[](DatValue datVal) => datVal.GetPrimitiveArray<int>();
        public static implicit operator uint[](DatValue datVal) => datVal.GetPrimitiveArray<uint>();
        public static implicit operator long[](DatValue datVal) => datVal.GetPrimitiveArray<long>();
        public static implicit operator ulong[](DatValue datVal) => datVal.GetPrimitiveArray<ulong>();
        public static implicit operator float[](DatValue datVal) => datVal.GetPrimitiveArray<float>();
        public static implicit operator double[](DatValue datVal) => datVal.GetPrimitiveArray<double>();
        public static implicit operator string[](DatValue datVal) => datVal.GetStringArray();

        public static implicit operator DatReference?(DatValue datVal) => datVal.GetReference();
        public static implicit operator DatReference[](DatValue datVal) => datVal.GetReferenceArray();

        public static implicit operator DatRow?(DatValue datVal) => datVal.GetReference()?.GetReferencedRow();
        public static implicit operator DatRow[](DatValue datVal)
        {
            var references = datVal.GetReferenceArray();
            var datRows = new DatRow[references.Length];
            for (int i = 0; i < references.Length; i++)
                datRows[i] = references[i].GetReferencedRow()!;

            return datRows;
        }
    }
}