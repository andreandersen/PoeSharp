using System.IO;

namespace PoeSharp.Filetypes.BuildingBlocks
{
    /// <inheritdoc />
    /// <summary>
    ///     File abstraction used to write to files.
    /// </summary>
    public interface IWritableFile : IFile
    {
        /// <summary>
        ///     Writes to file using a source stream
        /// </summary>
        /// <param name="sourceStream">Source stream that will be used</param>
        /// <param name="startOffset">Where in the stream to start copying from</param>
        /// <param name="length">Length of content to copy from source stream</param>
        void Write(Stream sourceStream, long startOffset = default,
            long length = default);

        /// <summary>
        ///     Writes to file using a byte array
        /// </summary>
        /// <param name="source">Bytes to write</param>
        void Write(byte[] source);

        /// <summary>
        ///     Copies content from another file
        /// </summary>
        /// <param name="file">Source file</param>
        void CopyFrom(IFile file);

        /// <summary>
        ///     Deletes file the file from file system
        /// </summary>
        void Delete();
    }
}
