using System.Collections.Generic;

namespace PoeSharp.Filetypes.BuildingBlocks
{
    /// <summary>
    ///     Abstraction of filesystem directory, which can be used to
    ///     create wrappers around containers, such as GGPK files etc.
    /// </summary>
    public interface IDirectory : IFileSystemEntry
    {
        /// <summary>
        ///     Child directories
        /// </summary>
        IEnumerable<IDirectory> Directories { get; }

        /// <summary>
        ///     Files in directory
        /// </summary>
        IEnumerable<IFile> Files { get; }

        /// <summary>
        ///     Gets a file system entry (this can be either a <see cref="IDirectory" /> or
        ///     <see cref="IFile" />. This shall not throw exceptions on no item found.
        /// </summary>
        /// <param name="index"></param>
        /// <returns>Returns the FileSystemEntry or null when not found.</returns>
        IFileSystemEntry? this[string index] { get; }

        void CopyTo(IWritableDirectory destination);
    }
}
