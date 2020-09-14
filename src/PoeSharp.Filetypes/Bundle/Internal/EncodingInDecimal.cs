namespace PoeSharp.Filetypes.Bundle.Internal
{
    internal enum EncodingInDecimal : uint
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
