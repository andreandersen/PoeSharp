using System;
using System.Linq;

namespace PoeSharp.Files.Dat.Specification
{
    public class FieldSpecification
    {
        private TypeData _clrType;

        public string Type { get; set; }
        public string Key { get; set; }
        public string KeyId { get; set; }
        public string KeyOffset { get; set; }
        public string Enum { get; set; }
        public bool Unique { get; set; }
        public string FilePath { get; set; }
        public string FileExt { get; set; }
        public string Display { get; set; }
        public string DisplayType { get; set; }
        public string Description { get; set; }
        public TypeData ClrType => _clrType ?? (_clrType = new TypeData(Type));

        public class TypeData
        {
            public TypeData (string type)
            {
                if (!type.StartsWith("ref|"))
                {
                    DefinedType = GetPrimitiveType(type);
                    return;
                }
                
                IsReference = true;
                var refTypeInfo = type.Split('|').Skip(1).ToArray();

                DefinedType = GetPrimitiveType(refTypeInfo.Last());
                IsList = refTypeInfo[0] == "list";
            }
            
            public Type DefinedType { get; set; }
            public bool IsReference { get; set; }
            public bool IsList { get; set; }

            private static Type GetPrimitiveType(string type)
            {
                switch (type)
                {
                    case "bool": return typeof(bool);
                    case "byte": return typeof(sbyte);
                    case "ubyte": return typeof(byte);
                    case "short": return typeof(short);
                    case "ushort": return typeof(ushort);
                    case "int": return typeof(int);
                    case "uint": return typeof(uint);
                    case "long": return typeof(long);
                    case "ulong": return typeof(ulong);
                    case "float": return typeof(float);
                    case "double": return typeof(double);
                    case "string": return typeof(string);
                }

                throw new InvalidCastException();
            }
        }
    }
}