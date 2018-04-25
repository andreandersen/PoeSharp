namespace PoeSharp.Shared.DataSources
{
    public interface IFileSystemEntry
    {
        /// <summary>
        ///     Parent directory. Must return null if root.
        /// </summary>
        IDirectory Parent { get; }

        /// <summary>
        /// Name of the filesystem entry.
        /// </summary>
        string Name { get; }

        /// <summary>
        ///     Get absolute path of the directory
        /// </summary>
        string Path { get; }
    }
}