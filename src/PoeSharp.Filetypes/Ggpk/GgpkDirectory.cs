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
        private readonly object _entriesLock = new();
        private bool _hasEvaluatedEntries;

        private readonly DirectoryEntry[] _entries;
        private readonly Dictionary<string, IDirectory> _directories = new();
        private readonly Dictionary<string, IFile> _files = new();

        internal GgpkFileSystem Root { get; }

        public string Name { get; }
        public IDirectory? Parent { get; }
        public string Path { get; }

        public DirectoryRecord Record { get; }

        public IReadOnlyDictionary<string, IDirectory> Directories
        {
            get
            {
                EnsureEntriesInitialized();
                return _directories;
            }
        }
        public IReadOnlyDictionary<string, IFile> Files
        {
            get
            {
                EnsureEntriesInitialized();
                return _files;
            }
        }

        public IFileSystemEntry this[string index]
        {
            get
            {
                if (Files.TryGetValue(index, out var f))
                    return f;

                if (Directories.TryGetValue(index, out var d))
                    return d;

                throw new ArgumentException("Entry not found");
            }
        }

        private GgpkDirectory(DirectoryRecord dirRecord, GgpkFileSystem root)
        {
            Record = dirRecord;
            Name = dirRecord.Name.ToString();
            Parent = null;
            Root = root;
            Path = string.Empty;

            _entries = dirRecord.Entries;
            _files = new Dictionary<string, IFile>();
            _directories = new Dictionary<string, IDirectory>();
        }

        private GgpkDirectory(DirectoryRecord dirRecord, GgpkDirectory parent)
        {
            Record = dirRecord;
            Name = dirRecord.Name.ToString();
            Parent = parent;
            Root = parent.Root;
            Path = System.IO.Path.Combine(parent.Path, Name);
            _entries = dirRecord.Entries;
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
                foreach (var entry in _entries)
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
            DirectoryRecord rootDirRecord, GgpkFileSystem fileSystem) =>
            new(rootDirRecord, fileSystem);

    }
}
