using System;
using System.IO;
using System.Threading.Tasks;

namespace PoeSharp.Filetypes.BuildingBlocks
{
    /// <summary>
    ///     File abstraction used to read and extract/export data.
    /// </summary>
    public interface IFile : IFileSystemEntry
    {
        /// <summary>
        ///     Size of file in bytes
        /// </summary>
        long Size { get; }

        /// <summary>
        ///     Copies the file's content to provided destination stream
        /// </summary>
        /// <param name="destinationStream">Destination stream</param>
        /// <param name="start">
        ///     Optional. Data offset to start copying from. Default is 0, which denotes
        ///     the beginning of the stream.
        /// </param>
        /// <param name="length">Optional. Length to copy. Default is end of stream.</param>
        void CopyToStream(Stream destinationStream, long start = default,
            long length = default);

        Span<byte> AsSpan(long start = default, long length = default);

        Stream GetStream();

        Task<Stream> GetStreamAsync();

        public string Extension
        {
            get
            {
                var length = Name.Length;
                for (var i = length; --i >= 0;)
                {
                    var ch = Name[i];
                    if (ch == '.')
                        return Name[i..length];
                    if (ch == System.IO.Path.DirectorySeparatorChar ||
                        ch == System.IO.Path.AltDirectorySeparatorChar ||
                        ch == System.IO.Path.VolumeSeparatorChar)
                    {
                        break;
                    }
                }
                return string.Empty;
            }
        }
    }
}
