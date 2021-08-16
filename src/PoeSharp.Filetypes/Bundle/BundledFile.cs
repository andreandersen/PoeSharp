using System.Diagnostics;

namespace PoeSharp.Filetypes.Bundle
{
    [DebuggerDisplay("{Path}")]
    public class BundledFile : IFile
    {
        internal readonly BundleIndex _bundleIndex;
        internal readonly BundleFileRecord _fileRecord;

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

        public Span<byte> AsSpan(long start = 0, long length = 0) =>
            _bundleIndex.GetContents(_fileRecord);

        public void CopyToStream(Stream destinationStream, long start = 0, long length = 0) =>
            destinationStream.Write(AsSpan());

        /// <summary>
        /// Decompresses the content from the bundle and returns a MemoryStream.
        /// </summary>
        /// <remarks>
        /// Consider using AsSpan() or CopyToStream() instead.
        /// </remarks>
        /// <returns></returns>
        public Stream GetStream()
        {
            var memoryStream = new MemoryStream();
            memoryStream.Write(AsSpan());
            memoryStream.Position = 0;

            return memoryStream;
        }

        /// <summary>
        /// Decompresses the content from the bundle and returns a MemoryStream.
        /// </summary>
        /// <remarks>
        /// Consider using AsSpan() or CopyToStream() instead.
        /// </remarks>
        /// <returns></returns>
        public Task<Stream> GetStreamAsync() =>
            Task.FromResult(GetStream());
    }
}