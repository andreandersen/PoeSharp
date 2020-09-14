using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using PoeSharp.Filetypes.Bundle.Internal;

namespace PoeSharp.Filetypes.Bundle
{
    public static class EncodedIndexBundle
    {
        public static IEnumerable<ReadOnlyMemory<byte>> EnumerateChunks(Stream stream)
        {
            ValidateStream(stream);

            var hdr = stream.Read<IndexBinHead>();
            var count = hdr.EntryCount;
            var sizes = stream.Read<uint>((int)count).ToArray();
            var buffer = new byte[(int)sizes.Max()];

            var idx = 0;
            while (idx < count)
            {
                var length = sizes[idx];
                var read = stream.Read(buffer, 0, (int)length);

                idx++;
                yield return buffer.AsMemory(0, read);
            }

        }

        private static void ValidateStream(Stream stream)
        {
            _ = stream ?? throw new ArgumentNullException(
                nameof(stream), "Stream is null");

            if (!stream.CanRead)
            {
                throw new ArgumentException(
                    "Stream cannot be read", nameof(stream));
            }
        }
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
