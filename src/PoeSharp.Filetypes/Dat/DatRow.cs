namespace PoeSharp.Filetypes.Dat
{
    public class DatRow
    {
        public readonly DatFile Parent;
        private readonly int _rowIndex;

        public DatRow(DatFile parent, int rowIndex)
        {
            Parent = parent;
            _rowIndex = rowIndex;
        }

        public ReadOnlySpan<byte> GetBytes() =>
            Parent.RowData.Slice(Parent.RowSize * _rowIndex, Parent.RowSize);

        public DatValue this[string columnName]
        {
            get
            {
                var spec = Parent.Spec[columnName];
                var offset = Parent.Is64Bit ? spec.Offset64 : spec.Offset32;
                var size = Parent.Is64Bit ? spec.Size64 : spec.Size32;
                return new DatValue(this, GetBytes().Slice(offset, size), spec);
            }
        }
    }
}