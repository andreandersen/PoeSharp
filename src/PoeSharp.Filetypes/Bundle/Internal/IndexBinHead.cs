using System.Runtime.InteropServices;

namespace PoeSharp.Filetypes.Bundle.Internal
{
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