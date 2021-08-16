namespace PoeSharp.Filetypes
{
    public static class TypeExtensions
    {
        public static int GetByteSize(this Type type) =>
            Type.GetTypeCode(type).GetByteSize();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int GetByteSize(this TypeCode tc) => tc switch
        {
            TypeCode.Boolean => sizeof(bool),
            TypeCode.SByte => sizeof(sbyte),
            TypeCode.Byte => sizeof(byte),
            TypeCode.Int16 => sizeof(short),
            TypeCode.UInt16 => sizeof(ushort),
            TypeCode.Int32 => sizeof(int),
            TypeCode.UInt32 => sizeof(uint),
            TypeCode.Int64 => sizeof(long),
            TypeCode.UInt64 => sizeof(ulong),
            TypeCode.Single => sizeof(float),
            TypeCode.Double => sizeof(double),
            TypeCode.Decimal => sizeof(decimal),
            _ => ThrowHelper.NotSupported<int>()
        };
    }
}