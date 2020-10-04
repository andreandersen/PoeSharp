namespace PoeSharp.Filetypes.BuildingBlocks
{
    public interface IWritableDirectory : IDirectory
    {
        IWritableFile GetOrCreateFile(string name);
        IWritableDirectory GetOrCreateDirectory(string name);

        void Delete();
    }
}
