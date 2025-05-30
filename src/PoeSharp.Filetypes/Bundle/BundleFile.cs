﻿namespace PoeSharp.Filetypes.Bundle
{
    public static class BundleFile
    {
        // A compressed bundle is chunked into pieces of 256 kB.
        private const int MaxChunkSize = 256 * 1024;

        // Safe space is used by the decompressor.
        // Just let this number be.
        private const int SafeSpace = 64;

        public static Span<byte> Decompress(
            Span<byte> buffer, int uncompressedOffset, int uncompressedSizeTake)
        {
            var hdr = buffer.ConsumeTo<EncodedBundleHeader>();
            var count = (int)hdr.EntryCount;
            var lastEntry = count - 1;
            var blockSizes = buffer.ConsumeTo<int>(count);

            var blockStart = Convert.ToInt32(Math.Floor(uncompressedOffset / (double)MaxChunkSize));
            var blockCount = Convert.ToInt32(Math.Ceiling(uncompressedSizeTake / (double)MaxChunkSize)) + 1;
            blockCount = Math.Min(blockSizes.Length - blockStart, blockCount);

            var slice = 0;
            if (blockStart > 0)
                slice = blockSizes.Slice(0, blockStart).ToArray().Sum();
            if (slice > 0)
                buffer = buffer[slice..];

            var destination = new Span<byte>(
                new byte[(blockCount * MaxChunkSize) + SafeSpace]);

            var pos = 0;
            for (var i = 0; i < blockCount; i++)
            {
                var ii = blockStart + i;
                var compressedSize = blockSizes[ii];

                var uncompressedSize = (int)(ii == lastEntry ?
                    hdr.UncompressedSize - (MaxChunkSize * lastEntry)
                    : MaxChunkSize);

                var compressed = buffer.Consume(compressedSize);

                var subDestionation = destination.Slice(
                    pos, uncompressedSize);

                _ = LibOoz.Ooz_Decompress(
                    compressed, compressedSize,
                    subDestionation, uncompressedSize);

                pos += uncompressedSize;
            }

            var sliceOffset = uncompressedOffset - (blockStart * MaxChunkSize);

            return destination.Slice(sliceOffset, uncompressedSizeTake);
        }

        public static Span<byte> Decompress(Span<byte> buffer)
        {
            var hdr = buffer.ConsumeTo<EncodedBundleHeader>();
            var count = (int)hdr.EntryCount;
            var lastEntry = count - 1;
            var blockSizes = buffer.ConsumeTo<int>(count);
            var blockCount = blockSizes.Length;

            var totalUncompressedSize = (int)hdr.UncompressedSize;

            var destination = new Span<byte>(
                new byte[totalUncompressedSize + SafeSpace]);

            var pos = 0;
            for (var i = 0; i < blockCount; i++)
            {
                var compressedSize = blockSizes[i];

                var uncompressedSize = (int)(i == lastEntry ?
                    hdr.UncompressedSize - (MaxChunkSize * lastEntry)
                    : MaxChunkSize);

                var compressed = buffer.Consume(compressedSize);

                var subDestionation = destination.Slice(
                    pos, uncompressedSize);

                _ = LibOoz.Ooz_Decompress(
                    compressed, compressedSize,
                    subDestionation, uncompressedSize);

                pos += uncompressedSize;
            }

            return destination.Slice(0, totalUncompressedSize);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Span<byte> DecompressBundle(this Span<byte> src) =>
            Decompress(src);

    }
}