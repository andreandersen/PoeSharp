using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PoeSharp.Files.Ggpk.Records;
using PoeSharp.Shared.DataSources;

namespace PoeSharp.Files.Ggpk
{
    public sealed class GgpkDirectory : IDirectory
    {
        private readonly List<IDirectory> _directories;
        private readonly Lazy<Dictionary<string, IFileSystemEntry>> _indexDict;
        private readonly List<IFile> _files;
        private string _path;

        internal GgpkDirectory(DirectoryRecord record, IDirectory parent,
            GgpkFileSystem root)
        {
            Parent = parent;
            Root = root;

            Name = record.Name;
            var records = record.Entries.ToRecords(root.Records).ToArray();

            _directories = records.OfType<DirectoryRecord>()
                .Select(c => new GgpkDirectory(c, this, root)).ToList<IDirectory>();

            _files = records.OfType<FileRecord>().Select(c => new GgpkFile(c, this, root))
                .ToList<IFile>();

            _indexDict = new Lazy<Dictionary<string, IFileSystemEntry>>(() =>
            {
                return _directories.Union(_files.Cast<IFileSystemEntry>())
                    .ToDictionary(c => c.Name);
            });
        }

        internal GgpkFileSystem Root { get; }

        public IEnumerable<IDirectory> Directories => _directories;
        public IEnumerable<IFile> Files => _files;
        public string Name { get; }
        public IDirectory Parent { get; }

        public string Path
        {
            get
            {
                if (!string.IsNullOrEmpty(_path))
                {
                    return _path;
                }

                _path = $"{Parent.Path}{System.IO.Path.PathSeparator}{Name}";
                return _path;
            }
        }

        public IFileSystemEntry this[string index] => _indexDict.Value[index];

        public void CopyTo(IWritableDirectory destination)
        {
            var tasks = new List<Task>();

            _files.ForEach(c =>
            {
                var destFile = destination.GetOrCreateFile(c.Name);
                destFile.CopyFrom(c);
            });

            _directories.ForEach(c =>
            {
                var newDir = destination.GetOrCreateDirectory(c.Name);
                c.CopyTo(newDir);
            });
        }

        public override string ToString() => $"{Name} (GGPK Directory)";
    }
}