using System.IO;
using System.Threading.Tasks;

namespace PoeSharp.Shared.DataSources
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

        Stream GetStream();

        Task<Stream> GetStreamAsync();
    }
}