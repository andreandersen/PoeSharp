using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

using PoeSharp.Filetypes.BuildingBlocks;
using PoeSharp.Filetypes.Bundle.Internal;

namespace PoeSharp.Filetypes.Bundle
{
    [DebuggerDisplay("{Path}")]
    public class BundledFile : IFile
    {
        private readonly BundleIndex _bundleIndex;
        private readonly BundleFileRecord _fileRecord;

        public ulong Hash => _fileRecord.Hash;

        public long Size => _fileRecord.FileSize;

        public IDirectory? Parent { get; }

        public string Name { get; }

        public string Path => System.IO.Path.Combine(Parent?.Path ?? "", Name);

        internal BundledFile(
            BundleIndex bundleIndex,
            BundleFileRecord fileRecord,
            string name,
            IDirectory? parent)
        {
            _fileRecord = fileRecord;
            _bundleIndex = bundleIndex;
            Name = name;
            Parent = parent;
        }

        public Span<byte> AsSpan(long start = 0, long length = 0) 
        {
            return _bundleIndex.GetContents(_fileRecord);
        }

        public void CopyToStream(Stream destinationStream, long start = 0, long length = 0) =>
            throw new NotImplementedException();

        public Stream GetStream() =>
            throw new NotImplementedException();

        public Task<Stream> GetStreamAsync() =>
            throw new NotImplementedException();
    }
}
