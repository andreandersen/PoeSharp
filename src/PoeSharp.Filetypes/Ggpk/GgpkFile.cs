using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

using PoeSharp.Filetypes.BuildingBlocks;
using PoeSharp.Filetypes.Ggpk.Records;

namespace PoeSharp.Filetypes.Ggpk
{
    [DebuggerDisplay("{Path}")]
    public sealed class GgpkFile : IFile
    {
        private readonly long _offset;

        public string Name { get; }
        public GgpkDirectory Parent { get; }
        public string Path { get; }
        public long Size { get; }

        IDirectory IFileSystemEntry.Parent => Parent;

        internal GgpkFile(FileRecord fileRecord, GgpkDirectory parent)
        {
            Parent = parent;
            Name = fileRecord.Name.ToString();
            Path = System.IO.Path.Combine(Parent.Path, Name);
            Size = fileRecord.DataLength;
            _offset = fileRecord.DataOffset;
        }

        public void CopyToStream(Stream stream)
        {
            var source = Parent.Root.Stream;
            source.CopyTo(stream, _offset, Size);
        }

        public Stream GetStream()
        {
            var source = Parent.Root.Stream;
            var dest = new MemoryStream();
            source.CopyTo(dest);
            source.Position = 0;
            return dest;
        }

        public void CopyToStream(Stream destinationStream, long start = 0, long length = 0)
        {

        }

        public Span<byte> AsSpan(long start = 0, long length = 0) => throw new NotImplementedException();
        
        public Task<Stream> GetStreamAsync() => throw new NotImplementedException();
    }
}
