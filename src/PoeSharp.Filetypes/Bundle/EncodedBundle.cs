using System;
using System.Buffers;
using System.IO;

using Microsoft.Toolkit.HighPerformance.Extensions;

using PoeSharp.Filetypes.Bundle.Internal;

namespace PoeSharp.Filetypes.Bundle
{
    public static class EncodedBundle
    {
        // A compressed bundle is chunked into pieces of 256 kB.
        private const int MaxChunkSize = 256 * 1024;

        // Safe space is used by the decompressor.
        // Just let this number be.
        private const int SafeSpace = 64;

        public static void DecompressToStream(Stream src, Stream dst)
        {
            src.ValidateSourceStream();
            dst.ValidateDestinationStream();

            // Read encoded bundle header
            var hdr = src.Read<EncodedBundleHeader>();

            var count = (int)hdr.EntryCount;
            var lastEntry = count - 1;

            // After the header comes a list of uints
            // representing the chunks' uncompressed sizes
            using var sizesOwner = src.Read<uint>(count);
            var sizes = sizesOwner.Span;

            // Rent some space
            using var bufRent = MemoryPool<byte>.Shared.Rent(MaxChunkSize + SafeSpace);
            using var dstRent = MemoryPool<byte>.Shared.Rent(MaxChunkSize + SafeSpace);

            // Decompress each chunk separately
            for (var i = 0; i < sizes.Length; i++)
            {
                var compressedSize = (int)sizes[i];

                // If last chunk, it might be smaller than 256K
                var uncompressedSize = (int)(i == lastEntry ?
                    hdr.UncompressedSize - (MaxChunkSize * lastEntry) : MaxChunkSize);

                // Slicing off necessary amount for the current required buffer
                var srcBuf = bufRent.Memory.Span.Slice(0, compressedSize);

                // Fill buffer from source stream
                src.Read(srcBuf);

                // Slicing off necessary amount for the current required buffer
                var dstBuf = dstRent.Memory.Span.Slice(0, uncompressedSize + SafeSpace);

                // Decompress using LibOoz. Thanks zao!
                _ = LibOoz.Ooz_Decompress(srcBuf, compressedSize, dstBuf, uncompressedSize);

                // Write decompressed result to destination stream
                dst.Write(dstBuf.Slice(0, uncompressedSize));
            }
        }

        public static Span<byte> DecompressBundle(this Stream src)
        {
            using var stream = src.DecompressBundleToStream();
            return stream.ToArray().AsSpan();
        }

        public static MemoryStream DecompressBundleToStream(this Stream stream)
        {
            var dst = new MemoryStream();
            DecompressToStream(stream, dst);
            dst.Position = 0;
            return dst;
        }
    }
}