using System.Runtime.CompilerServices;

namespace PoeSharp.Filetypes.Dat
{
    public static class NullCheckExtensions
    {
        //public static bool IsNullValue(this object obj) =>
        //    obj switch
        //    {
        //        -0x1010102 or // signed equivalent of 0xFEFEFEFE
        //        0xFEFEFEFE or
        //        0xFFFFFFFF or
        //        -0x101010101010102 or // signed equivalent
        //        0xFEFEFEFEFEFEFEFE => true,
        //        _ => false,
        //    };

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsNullValue(this int val) =>
            val is -0x1010102;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsNullValue(this uint val) =>
            val is 0xFEFEFEFE or 0xFFFFFFFF;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsNullValue(this long val) =>
            val is -0x101010101010102;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsNullValue(this ulong val) =>
            val is 0xFEFEFEFEFEFEFEFE;

    }
}
