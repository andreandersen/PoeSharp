using System;
using System.Linq;
using System.Text.Json.Serialization;

namespace PoeSharp.Filetypes.Dat.Specification
{
    public class DatField
    {
        /// <summary>
        /// String representation of the type for this column
        /// </summary>
        public string Type { get; }
        
        /// <summary>
        /// Reference to key in dat file
        /// </summary>
        public string Key { get; }
        
        /// <summary>
        /// Not sure what this is
        /// </summary>
        /// 

        public string KeyId { get; }
        
        /// <summary>
        /// If row representation has an offset (e.g. 1)
        /// </summary>
        public int KeyOffset { get; }

        /// <summary>
        /// Enum representation, such as MOD_DOMAIN, RARITY
        /// </summary>
        public string Enum { get; }

        /// <summary>
        /// Indicates if this column has a unique value for this file
        /// </summary>
        public bool Unique { get; }

        /// <summary>
        /// Indicates if this field is a file path
        /// </summary>
        public bool FilePath { get; }

        /// <summary>
        /// File extension to append in case file path is true
        /// </summary>
        public string FileExt { get; }

        /// <summary>
        /// Additional display for column
        /// </summary>
        public string Display { get; }

        /// <summary>
        /// Display format, normally {0}. 
        /// Python format.
        /// </summary>
        public string DisplayType { get; }

        /// <summary>
        /// Additional description for this field. 
        /// In many cases null.
        /// </summary>
        public string Description { get; }

        /// <summary>
        /// Byte count offset for this column in row
        /// </summary>
        public int Offset { get; internal set; }

        /// <summary>
        /// Byte size for this column
        /// </summary>
        public int Size { get; internal set; }

        /// <summary>
        /// Column index
        /// </summary>
        public int ColumnIndex { get; internal set; }

        /// <summary>
        /// TypeCode for this field, i.e. Int32
        /// </summary>
        public TypeCode TypeCode { get; }

        /// <summary>
        /// Indicates if this field is reference
        /// </summary>
        public bool IsReference { get; }

        /// <summary>
        /// Indicates if this field is a list
        /// </summary>
        public bool IsList { get; }

        /// <summary>
        /// Indicates if this is a generic reference
        /// </summary>
        public bool IsGenericReference { get; }

        /// <summary>
        /// Indicates if this is an external reference
        /// </summary>
        public bool IsExternalReference { get; }

        [JsonConstructor]
        public DatField(string type, string key, string keyId,
            int keyOffset, string @enum, bool unique,
            bool filePath, string fileExt, string display,
            string displayType, string description)
        {
            Type = type;
            Key = key;
            KeyId = keyId;
            KeyOffset = keyOffset;
            Enum = @enum;
            Unique = unique;
            FilePath = filePath;
            FileExt = fileExt;
            Display = display;
            DisplayType = displayType;
            Description = description;

            // Columns can be references to the data section
            // or in the case of "generic", a row number in
            // the same file.

            // Examples types:
            // ulong
            // ref|list|ulong <-- list of ulongs
            // ref|string <-- string (variable length) in data section
            // ref|generic <-- references row in same file

            if (!type.StartsWith("ref|"))
            {
                TypeCode = ParseTypeCode(type);
                return;
            }

            IsReference = true;
            var refTypeInfo = type.Split('|')[1..];

            TypeCode = ParseTypeCode(refTypeInfo.Last());
            IsList = refTypeInfo[0] == "list";
            IsGenericReference = refTypeInfo.Last() == "generic";

            IsExternalReference =
                !string.IsNullOrEmpty(key)
                && !IsGenericReference;
        }

        private static TypeCode ParseTypeCode(string type) =>
            type switch
            {
                "bool" => TypeCode.Boolean,
                "byte" => TypeCode.SByte,
                "ubyte" => TypeCode.Byte,
                "short" => TypeCode.Int16,
                "ushort" => TypeCode.UInt16,
                "int" => TypeCode.Int32,
                "uint" => TypeCode.UInt32,
                "long" => TypeCode.Int64,
                "ulong" => TypeCode.UInt64,
                "float" => TypeCode.Single,
                "double" => TypeCode.Double,
                "string" => TypeCode.String,
                "generic" => TypeCode.Int32,
                _ => throw new InvalidCastException(),
            };
    }
}