namespace PoeSharp.Filetypes.Dat
{
    public static class ObjectExtensions
    {
        public static bool IsNullValue(this object obj) =>
            obj switch
            {
                -0x1010102 or // signed equivalent of 0xFEFEFEFE
                0xFEFEFEFE or
                0xFFFFFFFF or
                -0x101010101010102 or // signed equivalent
                0xFEFEFEFEFEFEFEFE => true,
                _ => false,
            };
    }
}
