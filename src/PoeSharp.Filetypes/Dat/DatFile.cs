using System;
using System.Collections.Generic;
using System.Collections.Immutable;

using PoeSharp.Filetypes.BuildingBlocks;
using PoeSharp.Filetypes.Dat.Specification;

namespace PoeSharp.Filetypes.Dat
{
    public class DatFile
    {
        private static readonly byte[] DataSeparator =
            BitConverter.GetBytes(0xbbbbbbbbbbbbbbbb);

        private readonly DatFileIndex _idx;

        private readonly object _loadLock = new object();
        private readonly IFile _source;

        private readonly DatSpecification _spec;
        private bool _isLoaded;
        private DatRow[] _rows;

        internal DatFile(IFile source, DatSpecification specification, DatFileIndex index,
            bool lazyLoad = true)
        {
            _source = source;
            _spec = specification;
            _idx = index;

            if (!lazyLoad) Load();
        }

        public IReadOnlyList<DatRow> Rows
        {
            get
            {
                if (_isLoaded) return _rows;

                lock (_loadLock)
                {
                    if (!_isLoaded) Load();
                }

                return _rows;
            }
        }

        public DatRow this[int index] => Rows[index];

        public int Count => Rows.Count;


        private void Load()
        {
            Span<byte> data = _source.AsSpan();

            var rowsCount = (int) data.Slice(0, 4).To<uint>();
            var dataStart = data.IndexOf(DataSeparator);
            var rowSize = rowsCount > 0 ? (dataStart - 4) / rowsCount : 0;

            var dataSection = data[dataStart..];
            var rows = new DatRow[rowsCount];

            for (var r = 0; r < rowsCount; r++)
            {
                var rowBytes = data.Slice(4 + r * rowSize, rowSize);
                var datRow = new DatRow(rowBytes, dataSection, _spec, _idx);
                rows[r] = datRow;
            }

            _rows = rows;
            _isLoaded = true;
        }
    }
}