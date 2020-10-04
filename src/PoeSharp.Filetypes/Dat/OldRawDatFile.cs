using System;
using System.IO;

using PoeSharp.Filetypes.Ggpk;

namespace PoeSharp.Filetypes.Dat
{
    public record Point(int X, int Y);

    public class OldRawDatFile
    {
        private static readonly byte[] s_dataSeparator =
            BitConverter.GetBytes(0xbbbbbbbbbbbbbbbb);

        public ReadOnlyMemory<ReadOnlyMemory<byte>> Rows { get; }
        public ReadOnlyMemory<byte> Data { get; }
        public string Name { get; }

        private OldRawDatFile(
            string name,
            ReadOnlyMemory<ReadOnlyMemory<byte>> rows,
            ReadOnlyMemory<byte> data)
        {
            Name = name;
            Rows = rows;
            Data = data;
        }

        public static OldRawDatFile CreateFromStream(string name, Stream stream, int length)
        {
            Span<byte> bytes = new byte[length];
            stream.Read(bytes);

            var dataStart = bytes.IndexOf(s_dataSeparator);
            var rowsCount = (int)bytes.Slice(0, 4).To<uint>();
            var rowSize = rowsCount > 0 ? ((dataStart == -1 ? 0 : dataStart) - 4) / rowsCount : 0;

            var rows = new Memory<ReadOnlyMemory<byte>>(
                new ReadOnlyMemory<byte>[rowsCount]);

            var rowsSpan = rows.Span;
            for (int i = 0; i < rowsCount; i++)
            {
                rowsSpan[i] = bytes
                    .Slice(4 + i * rowSize, rowSize)
                    .ToArray().AsMemory();
            }

            byte[] data = dataStart == -1 ?
                Array.Empty<byte>() :
                bytes[dataStart..].ToArray();

            return new OldRawDatFile(name, rows, data);
        }

        public static OldRawDatFile CreateFromGgpkFile(GgpkFile ggpkFile)
        {
            using var ms = new MemoryStream();
            ggpkFile.CopyToStream(ms);
            ms.Position = 0;

            return CreateFromStream(ggpkFile.Name, ms, (int)ms.Length);
        }
    }
}
