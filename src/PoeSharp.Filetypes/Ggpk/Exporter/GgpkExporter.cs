﻿namespace PoeSharp.Filetypes.Ggpk.Exporter
{
    public class GgpkExporter
    {
        public event EventHandler<ExportedFileEventArgs>? FileExported;

        public Task Export(GgpkFileSystem ggpk, ExportConfig config) =>
            Export(ggpk.Directories.Values, config);

        public Task Export(IDirectory ggpkDir, ExportConfig config) =>
            Export(new[] { ggpkDir }, config);

        public async Task Export(IEnumerable<IDirectory> ggpkDirs, ExportConfig config)
        {
            var status = new ExportStatus();
            if (!config.Shallow)
            {
                FillQueue(status.DirQueue, ggpkDirs);
            }
            else
            {
                foreach (var dir in ggpkDirs)
                {
                    FillQueue(
                        status.FileQueue,
                        dir.Files.Values);
                }
            }

            await Task.WhenAll(Enumerable.Union(
                    StartEnumerationTasks(config, status),
                    StartExportTasks(config, status))).ConfigureAwait(false);

            await Task.Delay(50);
        }

        private List<Task> StartExportTasks(
            ExportConfig config, ExportStatus status)
        {
            return Enumerable
                .Range(0, config.ExportTasks)
                .Select(_ => Task.Factory.StartNew(() =>
                {
                    while (
                        !status.IsEnumerationDone ||
                        !status.FileQueue.IsEmpty)
                    {
                        if (!status.FileQueue.TryDequeue(out var file))
                            continue;

                        file.Extract(Path.Combine(config.ExportPath, file.Path));
                        status.ReportFileWritten(file.Size);
                        FileExported?.Invoke(
                            this,
                            new ExportedFileEventArgs(
                                status, file.Path, file.Size));
                    }
                })).ToList();
        }

        private static List<Task> StartEnumerationTasks(
            ExportConfig config, ExportStatus status)
        {
            return Enumerable
                .Range(0, config.EnumerationTasks)
                .Select(_ => Task.Factory.StartNew(() =>
                {
                    while (status.DirQueue.TryDequeue(out var dir))
                    {
                        if (!config.Shallow)
                        {
                            FillQueue(
                                status.DirQueue,
                                dir.Directories.Values);
                        }

                        status.ReportFilesDiscovered(dir.Files.Values);
                        FillQueue(status.FileQueue, dir.Files.Values);
                    }
                    status.IsEnumerationDone = true;
                })).ToList();
        }

        private static void FillQueue(
            ConcurrentQueue<IDirectory> q,
            IEnumerable<IDirectory> dirs)
        {
            foreach (var dir in dirs)
                q.Enqueue(dir);
        }

        private static void FillQueue(
            ConcurrentQueue<IFile> q,
            IEnumerable<IFile> files)
        {
            foreach (var file in files)
                q.Enqueue(file);
        }
    }
}