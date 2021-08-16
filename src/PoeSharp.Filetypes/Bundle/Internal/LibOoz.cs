namespace PoeSharp.Filetypes.Bundle.Internal
{
    internal class LibOoz
    {
        [DllImport(@"Bundle\lib\libooz.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int Ooz_Decompress(
            ref byte compressedContent, int compressedLength,
            ref byte decompressedContent, int decompressedSize,
            int fuzz, int crc, int verbose, byte[]? dst_base, int e,
            IntPtr cb, IntPtr cb_ctx, IntPtr scratch, int scratch_size, int threadPhase);

        public static int Ooz_Decompress(
            Span<byte> compressedContent, int compressedLength,
            Span<byte> decompressedContent, int decompressedSize)
        {
            ref var src = ref compressedContent.DangerousGetReference();
            ref var dst = ref decompressedContent.DangerousGetReference();

            var retVal = Ooz_Decompress(
                ref src, compressedLength, ref dst, decompressedSize, 0, 0, 0,
                null, 0, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, 0, 3);

            return retVal;
        }
    }
}