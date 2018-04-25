using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace PoeSharp.Util
{
    public static class TypeExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe int SizeOf<T>(this T _) where T : struct
        {
            var typeCode = Type.GetTypeCode(typeof(T));
            switch (typeCode)
            {
                case TypeCode.Boolean:
                    return sizeof(bool);
                case TypeCode.Char:
                    return sizeof(char);
                case TypeCode.SByte:
                    return sizeof(sbyte);
                case TypeCode.Byte:
                    return sizeof(byte);
                case TypeCode.Int16:
                    return sizeof(short);
                case TypeCode.UInt16:
                    return sizeof(ushort);
                case TypeCode.Int32:
                    return sizeof(int);
                case TypeCode.UInt32:
                    return sizeof(uint);
                case TypeCode.Int64:
                    return sizeof(long);
                case TypeCode.UInt64:
                    return sizeof(ulong);
                case TypeCode.Single:
                    return sizeof(float);
                case TypeCode.Double:
                    return sizeof(double);
                case TypeCode.Decimal:
                    return sizeof(decimal);
                case TypeCode.DateTime:
                    return sizeof(DateTime);
                default:
                    var tArray = new T[2];
                    var tArrayPinned = GCHandle.Alloc(tArray, GCHandleType.Pinned);
                    try
                    {
                        var tRef0 = __makeref(tArray[0]);
                        var tRef1 = __makeref(tArray[1]);
                        var ptrToT0 = *((IntPtr*)&tRef0);
                        var ptrToT1 = *((IntPtr*)&tRef1);
                        return (int)((byte*)ptrToT1 - (byte*)ptrToT0);
                    }
                    finally
                    {
                        tArrayPinned.Free();
                    }
            }
        }
    }
}
