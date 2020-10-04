using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace PoeSharp.Filetypes.Bundle.Internal
{
    internal unsafe class LibOoz
    {
        [DllImport("Bundle\\lib\\libooz.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int Ooz_Decompress(void* compressedContent, int compressedLength, void* decompressedContent, int decompressedSize,
            int fuzz, int crc, int verbose, byte[] dst_base, int e, IntPtr cb, IntPtr cb_ctx, IntPtr scratch, int scratch_size, int threadPhase);

        public static int Ooz_Decompress(Span<byte> compressedContent, int compressedLength, Span<byte> decompressedContent, int decompressedSize)
        {
            void* src = Unsafe.AsPointer(ref compressedContent[0]);
            void* dst = Unsafe.AsPointer(ref decompressedContent[0]);
            return Ooz_Decompress(src, compressedLength, dst, decompressedSize, 0, 0, 0, null, 0, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, 0, 0);
        }
    }
}
