using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

using PoeSharp.Filetypes.BuildingBlocks;

namespace PoeSharp.Filetypes.Ggpk.Exporter
{
    internal class ExportStatus
    {
        private int _filesWrittenCount;
        private long _filesWrittenSize;
        private int _totalFiles;
        private long _totalSize;


        public ExportStatus()
        {
            DirQueue = new ConcurrentQueue<IDirectory>();
            FileQueue = new ConcurrentQueue<IFile>();
        }

        public ConcurrentQueue<IDirectory> DirQueue { get; }
        public ConcurrentQueue<IFile> FileQueue { get; }

        public bool IsEnumerationDone { get; internal set; }
        public int FilesWrittenCount  => _filesWrittenCount;
        public long FilesWrittenSize  => _filesWrittenSize;

        public long TotalFileSize => _totalSize;
        public int TotalFileCount => _totalFiles;

        public void ReportFileWritten(long bytes)
        {
            Interlocked.Increment(ref _filesWrittenCount);
            Interlocked.Add(ref _filesWrittenSize, bytes);
        }

        public void ReportFilesDiscovered(IEnumerable<IFile> files)
        {
            var fileCount = files.Count();
            var fileSize = files.Sum(c => c.Size);

            Interlocked.Add(ref _totalSize, fileSize);
            Interlocked.Add(ref _totalFiles, fileCount);
        }
    }
}
