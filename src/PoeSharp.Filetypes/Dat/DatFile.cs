using System;

using PoeSharp.Filetypes.BuildingBlocks;
using PoeSharp.Filetypes.Dat.Specification;

namespace PoeSharp.Filetypes.Dat
{
    public partial class DatFile
    {
        private static readonly byte[] DataSeparator =
            BitConverter.GetBytes(0xbbbbbbbbbbbbbbbb);

        private readonly IFile _sourceFile;
        private bool _isLoaded;
        private readonly object _lockObject = new();

        protected byte[] Bytes;
        protected int RowSize { get; private set; }
        protected int RowCount;
        protected int DataOffset;


        public int Count
        {
            get
            {
                if (!_isLoaded)
                    EnsureLoaded();

                return RowCount;
            }
        }

        public DatSchema Specification => Spec;

        protected readonly DatFileIndex Index;
        protected readonly DatSchema Spec;

        public string Name => _sourceFile.Name;

        public DatRow this[int row]
        {
            get
            {
                if (!_isLoaded)
                    EnsureLoaded();

                return new DatRow(this, row);
            }
        }

        internal DatFile(
            IFile file,
            DatFileIndex datIndex,
            DatSchema spec)
        {
            _sourceFile = file;
            Index = datIndex;
            Spec = spec;
            Bytes = Array.Empty<byte>();
        }

        private void EnsureLoaded()
        {
            if (_isLoaded)
                return;

            lock (_lockObject)
            {
                if (_isLoaded)
                    return;

                var bytes = _sourceFile.AsSpan();

                RowCount = (int)bytes.Slice(0, 4).To<uint>();
                DataOffset = bytes.IndexOf(DataSeparator);
                RowSize = RowCount > 0 ? ((DataOffset == -1 ? 0 : DataOffset) - 4) / RowCount : 0;
                DataOffset = DataOffset > 0 ? DataOffset - 4 : -1;
                Bytes = bytes[4..].ToArray();
                _isLoaded = true;
            }    
        }
    }

}
