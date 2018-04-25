using System;
using System.Collections.Generic;
using System.Linq;
using PoeSharp.Files.Dat.Specification;

namespace PoeSharp.Files.Dat
{
    public sealed class DatValue
    {
        private readonly DatFileIndex _idx;

        private static readonly object[] NullValues =
             { -0x1010102, 0xFEFEFEFE, -0x101010101010102, 0xFEFEFEFEFEFEFEFE, 0xFFFFFFFF };

        public DatValue(FieldSpecification fieldData, object fieldValue,
            DatFileIndex datIndex)
        {
            Specification = fieldData;
            _idx = datIndex;

            if (!string.IsNullOrEmpty(fieldData.Key))
            {
                if (fieldValue is List<object> l)
                {
                    _fieldValue = new Lazy<object>(() => l
                        .Select(p =>
                            NullValues.Contains(p) ?
                            null : _idx[fieldData.Key][Convert.ToInt32(p)])
                        .ToList());
                }
                else if (fieldValue is ulong || fieldValue is int)
                {
                    _fieldValue = NullValues.Contains(fieldValue) ?
                        new Lazy<object>(() => null) :
                        new Lazy<object>(() => _idx[fieldData.Key][Convert.ToInt32(fieldValue)]);
                }
                else
                {
                    if (NullValues.Contains(fieldValue))
                    {
                        _fieldValue = new Lazy<object>(() => null);
                    } else
                    {
                        throw new Exception("Unexpected data type in conjunction with a Key field");
                    }
                }
            }
            else
            {
                _fieldValue = NullValues.Contains(fieldValue) ?
                    new Lazy<object>(() => null) :
                    new Lazy<object>(() => fieldValue);
            }
        }

        private readonly Lazy<object> _fieldValue;

        public FieldSpecification Specification { get; }
        public object Value => _fieldValue.Value;

        public override string ToString() => Value?.ToString();
    }
}