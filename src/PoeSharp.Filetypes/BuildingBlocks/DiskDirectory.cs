using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using IoPath = System.IO.Path;
namespace PoeSharp.Filetypes.BuildingBlocks
{
    public class DiskDirectory : IWritableDirectory
    {
        private readonly DirectoryInfo _dirInfo;

        public DiskDirectory(
            string name, IDirectory? parent = null,
            bool createIfNotExists = false)
        {
            _dirInfo = new DirectoryInfo(
                IoPath.Combine(parent?.Path ?? string.Empty, name));

            if (_dirInfo.Parent != null)
                Parent = new DiskDirectory(_dirInfo.Parent.FullName);

            Name = _dirInfo.Name;

            if (!createIfNotExists || _dirInfo.Exists) return;

            System.IO.Directory.CreateDirectory(_dirInfo.FullName);
            _dirInfo.Refresh();
        }

        public IDirectory? Parent { get; }
        public string Name { get; }

        public IReadOnlyDictionary<string, IDirectory> Directories
        {
            get
            {
                if (!_dirInfo.Exists)
                    return new Dictionary<string, IDirectory>();

                return
                    _dirInfo.GetDirectories().Select(
                    c => (IDirectory)new DiskDirectory(c.Name, this))
                    .ToDictionary(p => p.Name);
            }
        }

        public IReadOnlyDictionary<string, IFile> Files
        {
            get
            {
                if (!_dirInfo.Exists)
                    return new Dictionary<string, IFile>();

                return
                    _dirInfo.GetFiles().Select(
                    c => (IFile)new DiskFile(c.Name, this))
                    .ToDictionary(p => p.Name);
            }
        }

        public string Path => _dirInfo.FullName;

        public IFileSystemEntry? this[string index]
        {
            get
            {
                if (Directories.TryGetValue(index, out var dir))
                    return dir;

                if (Files.TryGetValue(index, out var file))
                    return file;

                return null;
            }
        }

        public IWritableFile GetOrCreateFile(string name) =>
            new DiskFile(name, this);

        public IWritableDirectory GetOrCreateDirectory(string name)
        {
            if (!_dirInfo.Exists)
                _dirInfo.Create();

            var existing = this[name];
            return existing switch
            {
                null => new DiskDirectory(name, this, true),
                IWritableDirectory dir => dir,
                IFile _ => throw new InvalidOperationException(
            "The directory already contains a file with this name"),
                _ => throw new Exception("Unknown exception"),
            };
        }

        public void Delete() => _dirInfo.Delete(true);

        public static implicit operator DiskDirectory(string directory) =>
            new DiskDirectory(directory);
    }
}
