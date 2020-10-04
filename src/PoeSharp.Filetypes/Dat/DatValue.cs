using System;
using System.Linq;

using PoeSharp.Filetypes.Dat.Specification;

namespace PoeSharp.Filetypes.Dat
{
    public sealed class DatValue
    {
        public DatValue(DatField fieldData, object fieldValue,
            DatFileIndex datIndex)
        {
            Specification = fieldData;

            if (!string.IsNullOrEmpty(fieldData.Key))
            {
                switch (fieldValue)
                {
                    case object[] l:
                        {
                            _fieldValue = new Lazy<object>(() => l
                                .Select(p =>
                                    p.IsNullValue() ?
                                    null : datIndex[fieldData.Key][Convert.ToInt32(p)])
                                .ToList());
                            break;
                        }

                    case ulong or int or uint:
                        _fieldValue = fieldValue.IsNullValue() ?
                            new Lazy<object>(() => null) :
                            new Lazy<object>(() => datIndex[fieldData.Key][Convert.ToInt32(fieldValue)]);
                        break;

                    default:
                        _fieldValue = fieldValue.IsNullValue() ?
                            new Lazy<object>(() => null)
                            : throw new Exception(
                                "Unexpected data type in conjunction with a Key field");
                        break;
                }
            }
            else
            {
                _fieldValue = fieldValue.IsNullValue() ?
                    new Lazy<object>(() => null) :
                    new Lazy<object>(() => fieldValue);
            }
        }

        private readonly Lazy<object> _fieldValue;

        public DatField Specification { get; }

        public object Value => _fieldValue.Value;

        public override string ToString() => Value?.ToString();
    }
}
