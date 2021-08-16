using System.Diagnostics;

namespace PoeSharp.Filetypes.Dat
{
    public readonly struct DatReference : IEquatable<DatReference>
    {
        public DatRow ReferencedFrom { get; }
        public ReferenceDefinition? ReferenceDefinition { get; }
        public int RowIndex { get; }
        public bool IsForeign { get; }

        public DatReference(
            int rowIdx, DatRow from,
            ReferenceDefinition? reference,
            bool isForeign = false)
        {
            RowIndex = rowIdx;
            ReferencedFrom = from;
            ReferenceDefinition = reference;
            IsForeign = isForeign;
        }

        public DatRow? GetReferencedRow()
        {
            var parent = ReferencedFrom.Parent;

            if (!IsForeign)
            {
                return parent.RowCount > RowIndex ? parent[RowIndex] : null;
            }

            var extensionToUse = parent.SourceFile.Extension;
            var tableFile = ReferenceDefinition!.Table + extensionToUse;

            if (parent.DatFileIndex.TryGetValue(tableFile, out var datFile) &&
                datFile.RowCount > RowIndex)
            {
                return datFile[RowIndex];
            }

            Debug.Fail("File not found");
            return null;
        }

        public override bool Equals(object? obj) => obj is DatReference reference && Equals(reference);
        public bool Equals(DatReference other) =>
            EqualityComparer<DatRow>.Default.Equals(ReferencedFrom, other.ReferencedFrom)
            && EqualityComparer<ReferenceDefinition?>.Default.Equals(ReferenceDefinition, other.ReferenceDefinition)
            && RowIndex == other.RowIndex && IsForeign == other.IsForeign;
        public override int GetHashCode() => HashCode.Combine(ReferencedFrom, ReferenceDefinition, RowIndex, IsForeign);

        public static bool operator ==(DatReference left, DatReference right) => left.Equals(right);
        public static bool operator !=(DatReference left, DatReference right) => !(left == right);
    }
}