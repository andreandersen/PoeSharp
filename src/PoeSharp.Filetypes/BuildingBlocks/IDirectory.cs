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
        IReadOnlyDictionary<string, IDirectory> Directories { get; }

        /// <summary>
        ///     Files in directory
        /// </summary>
        IReadOnlyDictionary<string, IFile> Files { get; }

        /// <summary>
        ///     Gets a file system entry (this can be either a <see cref="IDirectory" /> or
        ///     <see cref="IFile" />. This shall not throw exceptions on no item found.
        /// </summary>
        /// <param name="index"></param>
        /// <returns>Returns the FileSystemEntry or null when not found.</returns>
        IFileSystemEntry? this[string index] { get; }

        public void CopyTo(IWritableDirectory destination)
        {
            foreach (var file in Files.Values)
            {
                destination.GetOrCreateFile(file.Name).CopyFrom(file);
            }

            foreach (var dir in Directories.Values)
            {
                var subDir = destination.GetOrCreateDirectory(dir.Name);
                dir.CopyTo(subDir);
            }
        }
    }
}
