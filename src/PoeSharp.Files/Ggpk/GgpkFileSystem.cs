using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using PoeSharp.Files.Ggpk.Records;
using PoeSharp.Shared.DataSources;

namespace PoeSharp.Files.Ggpk
{
    public sealed class GgpkFileSystem : IDirectory, IDisposable
    {

        internal readonly Dictionary<long, IRecord> Records;
        internal readonly Stream SourceStream;
        private readonly List<GgpkDirectory> _directories;
        private readonly List<GgpkFile> _files;
        private readonly Lazy<Dictionary<string, IFileSystemEntry>> _indexDict;

        public GgpkFileSystem(string filePath)
        {
            Name = new FileInfo(filePath).Name;
            SourceStream = File.OpenRead(filePath);

            var recordsList = new List<IRecord>();
            var headerBytes = new byte[sizeof(int) * 2];
            var streamLength = SourceStream.Length;

            while (SourceStream.Position < streamLength)
            {
                SourceStream.Read(headerBytes, 0, headerBytes.Length);
                RecordHeader header;
                unsafe
                {
                    fixed (byte* bt = &headerBytes[0])
                    {
                        header = *(RecordHeader*)bt;
                    }
                }

                switch (header.Type)
                {
                    case RecordType.Ggpk:
                        recordsList.Add(new GgpkRecord(SourceStream, header.Length));
                        break;
                    case RecordType.Directory:
                        recordsList.Add(new DirectoryRecord(SourceStream, header.Length));
                        break;
                    case RecordType.File:
                        recordsList.Add(new FileRecord(SourceStream, header.Length));
                        break;
                    case RecordType.Free:
                        recordsList.Add(new FreeRecord(SourceStream, header.Length));
                        break;
                    default:
                        throw new InvalidOperationException("Invalid record");
                }
            }

            Records = recordsList.ToDictionary(c => c.Offset);

            var ggpkRecord = (GgpkRecord)Records.First().Value;

            var rootOffset =
                ggpkRecord.RecordOffsets.First(c => Records[c] is DirectoryRecord);

            var root = (DirectoryRecord)Records[rootOffset];

            var rootDirectory = new GgpkDirectory(root, Parent, this);

            _directories = rootDirectory.Directories.OfType<GgpkDirectory>().ToList();
            _files = rootDirectory.Files.OfType<GgpkFile>().ToList();
            _indexDict = new Lazy<Dictionary<string, IFileSystemEntry>>(() =>
            {
                return _directories.Union(_files.Cast<IFileSystemEntry>())
                    .ToDictionary(c => c.Name);
            });
        }

        public IEnumerable<IDirectory> Directories => _directories;
        public IEnumerable<IFile> Files => _files;

        public string Name { get; }
        public IDirectory Parent => null;
        public string Path => string.Empty;
        public IFileSystemEntry this[string index] => _indexDict.Value[index];

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void CopyTo(IWritableDirectory destination)
        {
            for (var i = 0; i < _files.Count; i++)
            {
                var c = _files[i];
                var destFile = destination.GetOrCreateFile(c.Name);
                using (var destStream = destFile.GetStream())
                {
                    c.CopyToStream(destStream);
                }
            }

            for (var i = 0; i < _directories.Count; i++)
            {
                var c = _directories[i];
                var newDir = destination.GetOrCreateDirectory(c.Name);
                c.CopyTo(newDir);
            }
        }

        public void Dispose()
        {
            _files?.Clear();
            _directories?.Clear();
            SourceStream?.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
