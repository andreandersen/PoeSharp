using System;
using System.Runtime.InteropServices;

using Microsoft.Toolkit.HighPerformance;
using Microsoft.Toolkit.HighPerformance.Extensions;

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
            Span<byte> copy = stackalloc byte[compressedLength];
            compressedContent.CopyTo(copy);

            ref byte src = ref copy.DangerousGetReference();
            ref byte dst = ref decompressedContent.DangerousGetReference();

            return Ooz_Decompress(
                ref src, compressedLength, ref dst, decompressedSize, 0, 0, 0,
                null, 0, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, 0, 0);
        }
    }
}
