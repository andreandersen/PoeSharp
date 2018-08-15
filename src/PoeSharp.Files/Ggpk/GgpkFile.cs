using System;
using System.IO;
using System.Threading.Tasks;
using PoeSharp.Files.Ggpk.Records;
using PoeSharp.Shared.DataSources;
using PoeSharp.Util;

namespace PoeSharp.Files.Ggpk
{
    internal sealed class GgpkFile : IFile
    {
        private readonly long _offset;
        private readonly GgpkFileSystem _root;

        public GgpkFile(FileRecord fileRecord, GgpkDirectory parent, GgpkFileSystem root)
        {
            Parent = parent;
            Name = fileRecord.Name;
            Size = fileRecord.DataLength;

            _offset = fileRecord.DataOffset;
            _root = root;
        }
        public string Name { get; }

        public IDirectory Parent { get; }
        public string Path => System.IO.Path.Combine(Parent.Path, Name);
        public long Size { get; }

        public void CopyToStream(Stream destinationStream, long start = default,
            long length = default)
        {
            var bytes = length == default ? Size : Math.Min(Size, length);
            _root.SourceStream.CopyTo(destinationStream, _offset + start, bytes);

        }

        /// <summary>
        /// Gets the file as a MemoryStream. Be advised: This will copy the file content into a memory stream.
        /// </summary>
        /// <returns>MemoryStream with the file contents</returns>
        public Stream GetStream()
        {
            var memStream = new MemoryStream((int)Size);
            _root.SourceStream.CopyTo(memStream, _offset, Size);
            memStream.Position = 0;
            return memStream;
        }

        /// <summary>
        /// Gets the file as a MemoryStream. Be advised: This will copy the file content into a memory stream.
        /// </summary>
        /// <returns>MemoryStream with the file contents</returns>
        public Task<Stream> GetStreamAsync() => Task.FromResult(GetStream());
    }
}
