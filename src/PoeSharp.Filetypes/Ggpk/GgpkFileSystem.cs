namespace PoeSharp.Filetypes.Ggpk
{
    public sealed class GgpkFileSystem
    {
        private readonly ThreadLocal<FileStream> _threadStream;

        internal FileStream Stream => _threadStream.Value!;

        public string Path { get; }

        public IReadOnlyDictionary<string, IDirectory> Directories =>
            Root.Directories;

        public IReadOnlyDictionary<string, IFile> Files => Root.Files;

        public GgpkDirectory Root { get; }

        public GgpkFileSystem(string path)
        {
            _threadStream = new ThreadLocal<FileStream>(() =>
                File.OpenRead(path));

            Path = path;
            Root = GetRootDirectory();
        }

        private GgpkDirectory GetRootDirectory()
        {
            RecordHeader ggpkHeader;
            do
            {
                ggpkHeader = Stream.ReadRecordHeader();
            } while (ggpkHeader.Type != RecordType.Ggpk);

            var ggpk = new GgpkRecord(Stream, ggpkHeader.Length);
            DirectoryRecord? dirRecord = default;
            foreach (var offset in ggpk.RecordOffsets)
            {
                Stream.Position = offset;
                RecordHeader header = Stream.ReadRecordHeader();
                if (header.Type == RecordType.Directory)
                {
                    dirRecord = new DirectoryRecord(Stream, header.Length);
                    break;
                }
            }

            return dirRecord switch
            {
                not null => GgpkDirectory.CreateRootDirectory(dirRecord, this),
                _ => throw ParseException.GgpkParseFailure,
            };
        }

        public static implicit operator GgpkFileSystem(string filePath) => new(filePath);
    }
}