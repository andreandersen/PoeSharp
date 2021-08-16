namespace PoeSharp.Filetypes.Ggpk.Exporter
{
    public class ExportedFileEventArgs : EventArgs
    {
        internal ExportedFileEventArgs(ExportStatus status, string name, long size)
        {
            IsEnumerationDone = status.IsEnumerationDone;
            TotalFilesWrittenCount = status.FilesWrittenCount;
            TotalFilesWrittenSize = status.FilesWrittenSize;
            FileSize = size;
            FileName = name;
            TotalFileCount = status.TotalFileCount;
            TotalFileSize = status.TotalFileSize;

        }

        public bool IsEnumerationDone { get; set; }
        public int TotalFilesWrittenCount { get; set; }
        public long TotalFilesWrittenSize { get; set; }

        public int TotalFileCount { get; set; }
        public long TotalFileSize { get; set; }

        public long FileSize { get; set; }
        public string FileName { get; set; }
    }
}