namespace PoeSharp.Filetypes.Dat
{
    public class DatFile : RawDatFile
    {
        public TableDefinition Spec { get; }

        public DatFileIndex DatFileIndex { get; }

        public string Name => SourceFile.Name;

        public DatRow this[int row] => new(this, row);

        public DatFile(IFile file, DatFileIndex datIndex, TableDefinition spec) : base(file)
        {
            DatFileIndex = datIndex;
            Spec = spec;
        }
    }
}