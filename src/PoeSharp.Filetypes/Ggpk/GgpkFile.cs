using System.Diagnostics;
using System.IO;
using PoeSharp.Filetypes.Ggpk.Records;

namespace PoeSharp.Filetypes.Ggpk
{
    [DebuggerDisplay("{Path}")]
    public sealed class GgpkFile
    {
        private readonly long _offset;

        public string Name { get; }
        public GgpkDirectory Parent { get; }
        public string Path { get; }
        public long Size { get; }

        internal GgpkFile(in FileRecord fileRecord, in GgpkDirectory parent)
        {
            Parent = parent;
            Name = fileRecord.Name.ToString();
            Path = System.IO.Path.Combine(Parent.Path, Name);
            Size = fileRecord.DataLength;
            _offset = fileRecord.DataOffset;
        }

        public void CopyToStream(in Stream stream)
        {
            var source = Parent.Root.Stream;
            source.CopyTo(stream, _offset, Size);
        }

        public MemoryStream GetStream()
        {
            var source = Parent.Root.Stream;
            var dest = new MemoryStream();
            source.CopyTo(dest);
            source.Position = 0;
            return dest;
        }

        public void Extract(string path)
        {
            var file = new FileInfo(path);

            if (!file.Directory.Exists)
                file.Directory.Create();

            using (var fs = file.Exists ? file.OpenWrite() : file.Create())
            {
                CopyToStream(fs);
            }
        }
    }
}
