namespace PoeSharp.Filetypes.Dat
{
    public enum DatType { Dat32, Dat64, DatL32, DatL64, Invalid }

    public class RawDatFile
    {
        private static readonly byte[] DataSeparator = {
            0xBB, 0xBB, 0xBB, 0xBB, 0xBB, 0xBB, 0xBB, 0xBB };

        private readonly object _lockOject = new();
        private bool _loaded = false;
        private byte[]? _data;
        private int _rowSize;
        private int _rowCount;
        private int _dataOffset;

        public IFile SourceFile { get; }
        public DatType DatType { get; }
        public bool Is64Bit => DatType is DatType.Dat64 or DatType.DatL64;

        public int RowSize { get { EnsureLoaded(); return _rowSize; } }
        public int RowCount { get { EnsureLoaded(); return _rowCount; } }
        public int DataOffset { get { EnsureLoaded(); return _dataOffset; } }

        public ReadOnlySpan<byte> RawBytes { get { EnsureLoaded(); return _data!.AsSpan(); } }
        public ReadOnlySpan<byte> RowData => RawBytes[0.._dataOffset];
        public ReadOnlySpan<byte> ContentData => RawBytes[_dataOffset..];

        public RawDatFile(IFile sourceFile, bool eagerLoad = false)
        {
            SourceFile = sourceFile;
            DatType = DetermineDatTypeFromExtension(sourceFile.Extension);

            if (DatType == DatType.Invalid)
                throw new InvalidOperationException("Invalid/Unknown filetype provided");

            if (eagerLoad)
                EnsureLoaded();
        }

        private void EnsureLoaded()
        {
            if (_loaded)
                return;

            lock (_lockOject)
            {
                if (_loaded)
                    return;

                var data = SourceFile.AsSpan();

                _rowCount = data[0..4].To<int>();
                _dataOffset = data.IndexOf(DataSeparator);
                _rowSize = _rowCount > 0 ? ((_dataOffset == -1 ? 0 : _dataOffset) - 4) / _rowCount : 0;
                _dataOffset = _dataOffset > 0 ? _dataOffset - 4 : -1;

                _data = data[4..].ToArray();
                _loaded = true;
            }
        }

        public static DatType DetermineDatTypeFromExtension(string extension) =>
            extension.ToLowerInvariant() switch
            {
                ".dat" => DatType.Dat32,
                ".datl" => DatType.DatL32,
                ".dat64" => DatType.Dat64,
                ".datl64" => DatType.DatL64,
                _ => DatType.Invalid
            };
    }
}