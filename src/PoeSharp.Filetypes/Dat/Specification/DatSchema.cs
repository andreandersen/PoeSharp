using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace PoeSharp.Filetypes.Dat.Specification
{
    /// <summary>
    /// Schema for dat file. Used to read dat files' 
    /// columns in the right format
    /// </summary>
    public class DatSchema
    {
        public Dictionary<string, DatField> Fields { get; }

        [JsonConstructor]
        public DatSchema(Dictionary<string, DatField> fields)
        {
            var colIndex = 0;
            var offset = 0;
            foreach (var (name, field) in fields)
            {
                field.ColumnIndex = colIndex++;
                field.Offset = offset;

                // Determine the size of the column to be read
                // which is determined by the type
                field.Size = (field.IsList, field.IsGenericReference, field.IsReference) switch
                {
                    (false, false, _) when field.TypeCode is TypeCode.String => 4,
                    (false, false, false) => field.TypeCode.GetByteSize(),
                    (false, false, true) => 8,
                    (false, true, true) => 4,
                    (true, _, _) => 8,
                    _ => ThrowHelper.NotSupported<int>()
                };

                offset += field.Size;
            }

            Fields = fields;
        }
    }
}
