using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

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
            DirQueue = new ConcurrentQueue<GgpkDirectory>();
            FileQueue = new ConcurrentQueue<GgpkFile>();
        }

        public ConcurrentQueue<GgpkDirectory> DirQueue { get; }
        public ConcurrentQueue<GgpkFile> FileQueue { get; }

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

        public void ReportFilesDiscovered(IEnumerable<GgpkFile> files)
        {
            var fileCount = files.Count();
            var fileSize = files.Sum(c => c.Size);

            Interlocked.Add(ref _totalSize, fileSize);
            Interlocked.Add(ref _totalFiles, fileCount);
        }
    }
}
