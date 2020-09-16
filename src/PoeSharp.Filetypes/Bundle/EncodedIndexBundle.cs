using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

namespace PoeSharp.Filetypes.Bundle
{
    public static class EncodedIndexBundle
    {
        private const int MaxChunkSize = 256 * 1024;

        public static void PrepareIndexForDecompression2(Stream src, Stream dst)
        {
            ValidateSourceStream(src);
            var hdr = src.Read<IndexBinHead>();
            var count = hdr.EntryCount;
            var lastEntry = count - 1;
            var sizes = src.Read<uint>((int)count).ToArray();

            for (var i = 0; i < sizes.Length; i++)
            {
                var sz = (int)sizes[i];
                var buf = new byte[sz];

                var decSize = i == lastEntry ? hdr.UncompressedSize - (MaxChunkSize * lastEntry) : MaxChunkSize;
                var rc = src.Read(buf, 0, sz);

                var dstArr = new byte[decSize + 64];
                _ = LibOoz.Ooz_Decompress(buf, buf.Length, dstArr, (int)decSize);
                dst.Write(dstArr, 0, (int)decSize);
            }
        }

        private static void ValidateSourceStream(Stream source)
        {
            _ = source ?? throw new ArgumentNullException(
                nameof(source), "Stream is null");

            if (!source.CanRead)
            {
                throw new ArgumentException(
                    "Stream cannot be read", nameof(source));
            }
        }

        private static void ValidateDestinationStream(Stream destination)
        {
            _ = destination ?? throw new ArgumentNullException(
                nameof(destination), "Stream is null");

            if (!destination.CanWrite)
                throw new ArgumentException("Stream cannot be written to", nameof(destination));
        }
    }

    [StructLayout(LayoutKind.Explicit, Size = 60)]
    public readonly struct IndexBinHead
    {
        [FieldOffset(00)] public readonly uint UncompressedSize;
        [FieldOffset(04)] public readonly uint TotalPayloadSize;
        [FieldOffset(08)] public readonly uint HeadPayloadSize;
        [FieldOffset(12)] public readonly EncodingInDecimal FirstFileEncode;
        [FieldOffset(16)] public readonly uint Unk0;
        [FieldOffset(20)] public readonly ulong UncompressedSize2;
        [FieldOffset(28)] public readonly ulong TotalPayloadSize2;
        [FieldOffset(36)] public readonly uint EntryCount;
        [FieldOffset(40)] public readonly uint Unk1;
        [FieldOffset(44)] public readonly uint Unk2;
        [FieldOffset(48)] public readonly uint Unk3;
        [FieldOffset(52)] public readonly uint Unk4;
        [FieldOffset(56)] public readonly uint Unk5;
    }

    public enum EncodingInDecimal : uint
    {
        Kraken6 = 8,
        MermaidA = 9,
        LeviathanC = 13
    }
}

/*
 * Thanks Whitefang et al for figuring this out.
 * 
    uint32 uncompressed_size;
    uint32 total_payload_size;
    uint32 head_payload_size;
    struct head_payload_t {
        enum <uint32> {Kraken_6 = 8, Mermaid_A = 9, Leviathan_C = 13 } first_file_encode;
        uint32 unk03;
        uint64 uncompressed_size2;
        uint64 total_payload_size2;
        uint32 entry_count;
        uint32 unk28[5];
        uint32 entry_sizes[entry_count];
    } head;

    local int i <hidden=true>;
    for (i = 0; i < head.entry_count; ++i) {
        struct entry_t {
            byte data[head.entry_sizes[i]];
        } entry;
    }
*/
