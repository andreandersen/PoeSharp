using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using PoeSharp.Files.Dat.Specification;
using PoeSharp.Shared.DataSources;
using PoeSharp.Util;

namespace PoeSharp.Files.Dat
{
    public class DatFile : IReadOnlyList<DatRow>
    {
        private static readonly byte[] DataSeparator =
            BitConverter.GetBytes(0xbbbbbbbbbbbbbbbb);

        private readonly DatFileIndex _idx;

        private readonly object _loadLock = new object();
        private readonly IFile _source;

        private readonly DatSpecification _spec;
        private bool _isLoaded;
        private List<DatRow> _rows;

        internal DatFile(IFile source, DatSpecification specification, DatFileIndex index,
            bool lazyLoad = true)
        {
            _source = source;
            _spec = specification;
            _idx = index;

            if (!lazyLoad) Load();
        }

        private IReadOnlyList<DatRow> Rows
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

        public IEnumerator<DatRow> GetEnumerator() => Rows.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => Rows.GetEnumerator();

        private void Load()
        {
            Span<byte> data;
            using (var ms = new MemoryStream())
            {
                _source.CopyToStream(ms);
                ms.Position = 0;
                data = new Span<byte>(ms.ToArray());
            }

            var rowsCount = (int) data.Slice(0, 4).To<uint>();
            var dataStart = data.IndexOf(DataSeparator);
            var rowSize = rowsCount > 0 ? (dataStart - 4) / rowsCount : 0;

            var dataSection = data.Slice(dataStart);

            var rows = new List<DatRow>();
            for (var r = 0; r < rowsCount; r++)
            {
                var rowBytes = data.Slice(4 + r * rowSize, rowSize);
                var datRow = new DatRow(ref rowBytes, ref dataSection, _spec, _idx);
                rows.Add(datRow);
            }

            _rows = rows;
            _isLoaded = true;
        }
    }
}