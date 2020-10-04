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

        public DiskDirectory(string name, IDirectory parent = null, bool createIfNotExists = false)
        {
            _dirInfo = new DirectoryInfo(IoPath.Combine(parent?.Path ?? string.Empty, name));

            if (_dirInfo.Parent != null)
                Parent = new DiskDirectory(_dirInfo.Parent.FullName);

            Name = _dirInfo.Name;

            if (!createIfNotExists || _dirInfo.Exists) return;

            Directory.CreateDirectory(_dirInfo.FullName);
            _dirInfo.Refresh();
        }

        public IDirectory Parent { get; }
        public string Name { get; }

        public IEnumerable<IDirectory> Directories
        {
            get
            {
                if (!_dirInfo.Exists)
                    return new List<IDirectory>();
                return _dirInfo.GetDirectories().Select(c => new DiskDirectory(c.Name, this));
            }
        }

        public IEnumerable<IFile> Files
        {
            get
            {
                if (!_dirInfo.Exists)
                    return new List<IFile>();
                return _dirInfo.GetFiles().Select(c => new DiskFile(c.Name, this));
            }
        }

        public string Path => _dirInfo.FullName;

        public IFileSystemEntry this[string index]
        {
            get
            {
                var dirMatch = Directories.FirstOrDefault(c => c.Name == index);
                return dirMatch == null
                    ? Files.FirstOrDefault(c => c.Name == index)
                    : (IFileSystemEntry)dirMatch;
            }
        }

        public void CopyTo(IWritableDirectory destination)
        {
            foreach (var file in Files)
            {
                destination.GetOrCreateFile(file.Name).CopyFrom(file);
            }

            foreach (var dir in Directories)
            {
                var subDir = destination.GetOrCreateDirectory(dir.Name);
                dir.CopyTo(subDir);
            }
        }

        public IWritableFile GetOrCreateFile(string name) => new DiskFile(name, this);

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
