namespace PoeSharp.Filetypes.Dat
{
    public static class NullCheckExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsNullValue(this int val) =>
            val is -0x1010102;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsNullValue(this uint val) =>
            val is 0xFEFEFEFE or 0xFFFFFFFF;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsNullValue(this long val) =>
            val is -0x101010101010102 or -0x1010102;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsNullValue(this ulong val) =>
            val is 0xFEFEFEFEFEFEFEFE or 0xFEFEFEFE or 0xFFFFFFFF;
    }
}