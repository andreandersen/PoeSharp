using System;
using System.Collections.Generic;
using System.Diagnostics;

using PoeSharp.Filetypes.BuildingBlocks;
using PoeSharp.Filetypes.Ggpk.Records;

namespace PoeSharp.Filetypes.Ggpk
{
    [DebuggerDisplay("{Path}")]
    public sealed class GgpkDirectory : IDirectory
    {
        private readonly object _entriesLock = new object();
        private bool _hasEvaluatedEntries;
        private readonly ReadOnlyMemory<DirectoryEntry> _entries;
        private readonly Dictionary<string, GgpkDirectory> _directories;
        private readonly Dictionary<string, GgpkFile> _files;
        internal GgpkFileSystem Root { get; }

        public string Name { get; }
        public IDirectory? Parent { get; }
        public string Path { get; }

        public DirectoryRecord Record { get; }

        public IReadOnlyDictionary<string, GgpkDirectory> Directories
        {
            get
            {
                EnsureEntriesInitialized();
                return _directories;
            }
        }
        public IReadOnlyDictionary<string, GgpkFile> Files
        {
            get
            {
                EnsureEntriesInitialized();
                return _files;
            }
        }

        IEnumerable<IDirectory> IDirectory.Directories => throw new NotImplementedException();

        IEnumerable<IFile> IDirectory.Files => throw new NotImplementedException();

        public IFileSystemEntry this[string index] => throw new NotImplementedException();

        private GgpkDirectory(
                    DirectoryRecord dirRecord,
                    GgpkDirectory? parent = null,
                    GgpkFileSystem? root = null)
        {
            Record = dirRecord;
            Name = dirRecord.Name.ToString();
            Parent = parent;
            Root = root ?? parent?.Root ?? null;
            Path = Parent != null ?
                System.IO.Path.Combine(parent.Path, Name) :
                string.Empty;

            _entries = dirRecord.Entries;
            _files = new Dictionary<string, GgpkFile>();
            _directories = new Dictionary<string, GgpkDirectory>();
        }

        private void EnsureEntriesInitialized()
        {
            if (_hasEvaluatedEntries)
                return;

            lock (_entriesLock)
            {
                if (_hasEvaluatedEntries)
                    return;

                var stream = Root.Stream;
                foreach (var entry in _entries.Span)
                {
                    stream.Position = entry.Offset;
                    var record = stream.ReadRecord();
                    switch (record)
                    {
                        case FileRecord fr:
                            var file = new GgpkFile(fr, this);
                            _files.Add(file.Name, file);
                            break;
                        case DirectoryRecord dr:
                            var dir = new GgpkDirectory(dr, this);
                            _directories.Add(dir.Name, dir);
                            break;
                        default:
                            break;
                    }
                }

                _hasEvaluatedEntries = true;
            }
        }

        internal static GgpkDirectory CreateRootDirectory(
            DirectoryRecord rootDirRecord, GgpkFileSystem fileSystem)
        {
            return new GgpkDirectory(rootDirRecord, null, fileSystem);
        }

        public void CopyTo(IWritableDirectory destination) => throw new NotImplementedException();
    }
}
