using System;
using System.IO;
using System.Threading.Tasks;
using PoeSharp.Util;
using IoPath = System.IO.Path;

namespace PoeSharp.Shared.DataSources.Disk
{
    public class DiskFile : IWritableFile
    {
        private const int WriteStreamBuffer = 80 * 1024;

        public DiskFile(string name, DiskDirectory diskDirectory = null)
        {
            var p = EnsureNameAndPathSeparation(name, diskDirectory);

            Name = p.Name;
            Parent = p.Path;
        }

        public DiskFile(string name, DiskDirectory diskDirectory, Stream source,
            bool overwrite = true, long offset = default, long length = default)
        {
            Name = name;
            Parent = diskDirectory;
            if (!overwrite && File.Exists(Path))
                throw new Exception("File already exists");

            Write(source, offset, length);
        }

        public DiskFile(string name, Stream source, bool overwrite = true,
            long offset = default, long length = default)
        {
            var p = EnsureNameAndPathSeparation(name, null);

            Name = p.Name;
            Parent = p.Path;

            if (!overwrite && File.Exists(Path))
                throw new Exception("File already exists");

            Write(source, offset, length);
        }

        public IDirectory Parent { get; }
        public string Name { get; }
        public string Path => IoPath.Combine(Parent?.Path ?? string.Empty, Name);
        public long Size => new FileInfo(Path).Length;

        public void CopyToStream(Stream destinationStream, long start = default,
            long length = default)
        {
            using (var fs = File.OpenRead(Path))
            {
                fs.Position = start;
                if (length == default)
                    fs.CopyTo(destinationStream);
                else
                    fs.CopyTo(destinationStream, start, length);
            }
        }

        public void Write(Stream sourceStream, long startOffset = default,
            long length = default)
        {
            using (var fs = new FileStream(Path, FileMode.Create, FileAccess.Write,
                FileShare.None, WriteStreamBuffer, FileOptions.None))
            {
                sourceStream.CopyTo(fs, startOffset, length);
            }
        }

        public void Write(byte[] source) => File.WriteAllBytes(Path, source);

        public void CopyFrom(IFile file)
        {
            if (file is DiskFile diskFile)
                File.Copy(diskFile.Path, Path, true);
            else
                using (var fs = new FileStream(Path, FileMode.Create, FileAccess.Write,
                    FileShare.None, WriteStreamBuffer, FileOptions.None))
                {
                    file.CopyToStream(fs);
                }
        }

        public void Delete() => File.Delete(Path);

        public Stream GetStream() =>
            new FileStream(Path, FileMode.Create, FileAccess.ReadWrite, FileShare.None,
                WriteStreamBuffer);

        public Task<Stream> GetStreamAsync() => Task.FromResult(GetStream());

        private (string Name, DiskDirectory Path) EnsureNameAndPathSeparation(string name,
            DiskDirectory dir)
        {
            var fi = new FileInfo(IoPath.Combine(dir?.Path ?? "", name));
            if (dir == null || fi.Name != name)
            {
                return (fi.Name, new DiskDirectory(fi.DirectoryName));
            }

            return (name, dir);
        }
    }
}