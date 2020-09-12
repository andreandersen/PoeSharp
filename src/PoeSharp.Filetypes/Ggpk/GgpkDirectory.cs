using System;
using System.Buffers.Text;
using System.Collections.Generic;
using System.Diagnostics;
using PoeSharp.Filetypes.Ggpk.Records;

namespace PoeSharp.Filetypes.Ggpk
{
    [DebuggerDisplay("{Path}")]
    public sealed class GgpkDirectory
    {
        private object _entriesLock = new object();
        private bool _hasEvaluatedEntries;
        private ReadOnlyMemory<DirectoryEntry> _entries;
        private Dictionary<string, GgpkDirectory> _directories;
        private Dictionary<string, GgpkFile> _files;
        internal GgpkFileSystem Root { get; }

        public string Name { get; }
        public GgpkDirectory Parent { get; }
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

        private GgpkDirectory(
                    in DirectoryRecord dirRecord,
                    GgpkDirectory parent,
                    GgpkFileSystem root = null)
        {
            Record = dirRecord;
            Name = dirRecord.Name.ToString();
            Parent = parent;
            Root = root ?? parent.Root;
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
            in DirectoryRecord rootDirRecord, GgpkFileSystem fileSystem)
        {
            return new GgpkDirectory(rootDirRecord, null, fileSystem);
        }
    }
}
