using System.Collections;

namespace PoeSharp.Filetypes.Dat
{
    public sealed class DatFile : RawDatFile, IEnumerator<DatRow>, IEnumerable<DatRow>
    {
        public TableDefinition Spec { get; }

        public DatFileIndex DatFileIndex { get; }

        public string Name => SourceFile.Name;

        public DatRow Current => this[_pos];

        object IEnumerator.Current => this[_pos];

        public DatRow this[int row] => new(this, row);

        public DatFile(IFile file, DatFileIndex datIndex, TableDefinition spec) : base(file)
        {
            DatFileIndex = datIndex;
            Spec = spec;
        }

        private int _pos = -1;
        public bool MoveNext() => ++_pos < RowCount;
        public void Reset() => _pos = 0;
        
        public void Dispose() { }

        public IEnumerator<DatRow> GetEnumerator() => this;
        IEnumerator IEnumerable.GetEnumerator() => this;
    }
}