using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;

using PoeSharp.Filetypes.BuildingBlocks;

namespace PoeSharp.Filetypes.Bundle
{
    [DebuggerDisplay("{Path}")]
    public sealed class BundledDirectory : IDirectory
    {
        private readonly Dictionary<string, IFile> _files = new();
        private readonly Dictionary<string, IDirectory> _directories = new();

        public IReadOnlyDictionary<string, IDirectory> Directories =>
            _directories;

        public IReadOnlyDictionary<string, IFile> Files =>
            _files;

        public BundledDirectory(string name, IDirectory? parent)
        {
            Name = name;
            Parent = parent;

            Path = System.IO.Path.Combine(Parent?.Path ?? "", Name);
        }

        internal void AddDirectory(BundledDirectory dir) =>
            _directories.Add(dir.Name, dir);

        internal void AddFile(BundledFile file) =>
            _files.Add(file.Name, file);

        public IFileSystemEntry? this[string index]
        {
            get
            {
                if (_files.TryGetValue(index, out var file))
                    return file;
                if (_directories.TryGetValue(index, out var dir))
                    return dir;

                return null;
            }
        }

        public IDirectory? Parent { get; }

        public string Name { get; }

        public string Path { get; }
    }
}
