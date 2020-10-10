using System;
using System.Linq;

namespace PoeSharp.Filetypes.Dat.Specification
{
    public class DatField
    {
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        private DatType _clrType;
        public string Type { get; set; }
        public string Key { get; set; }
        public string KeyId { get; set; }
        public int KeyOffset { get; set; }
        public string Enum { get; set; }
        public bool Unique { get; set; }
        public bool FilePath { get; set; }
        public string FileExt { get; set; }
        public string Display { get; set; }
        public string DisplayType { get; set; }
        public string Description { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

        public DatType DatType => _clrType ??= new DatType(Type);
    }

    public class DatType
    {
        public DatType(string type)
        {
            if (!type.StartsWith("ref|"))
            {
                TypeCode = GetPrimitiveType(type);
                return;
            }

            IsReference = true;
            var refTypeInfo = type.Split('|')[1..];

            TypeCode = GetPrimitiveType(refTypeInfo.Last());
            IsList = refTypeInfo[0] == "list";
            IsGenericReference = refTypeInfo.Last() == "generic";
        }

        public TypeCode TypeCode { get; }
        public bool IsReference { get; }
        public bool IsList { get; }
        public bool IsGenericReference { get; }

        private static TypeCode GetPrimitiveType(string type) =>
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