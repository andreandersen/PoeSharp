namespace PoeSharp.Filetypes.Ggpk
{
    using System.Diagnostics;

    [DebuggerDisplay("{Path}")]
    public sealed class GgpkFile : IFile
    {
        private readonly long _offset;

        public string Name { get; }
        public GgpkDirectory Parent { get; }
        public string Path { get; }
        public long Size { get; }

        IDirectory IFileSystemEntry.Parent => Parent;

        internal GgpkFile(
            FileRecord fileRecord,
            GgpkDirectory parent)
        {
            Parent = parent;
            Name = fileRecord.Name.ToString();
            Path = System.IO.Path.Combine(Parent.Path, Name);
            Size = fileRecord.DataLength;
            _offset = fileRecord.DataOffset;
        }

        public Stream GetStream()
        {
            var dest = new MemoryStream();
            CopyToStream(dest);
            dest.Position = 0;
            return dest;
        }

        public void CopyToStream(
            Stream destinationStream,
            long start = 0, long length = 0)
        {
            if (length == 0)
                length = Size;

            var source = Parent.Root.Stream;
            source.CopyTo(destinationStream, _offset + start, length);
        }

        public Span<byte> AsSpan(long start = 0, long length = 0)
        {
            if (length == 0)
                length = Size;

            var source = Parent.Root.Stream;
            source.Position = _offset + start;
            var buff = new Span<byte>(new byte[length]);
            source.Read(buff);
            return buff;
        }

        public Task<Stream> GetStreamAsync() =>
            Task.FromResult(GetStream());
    }
}