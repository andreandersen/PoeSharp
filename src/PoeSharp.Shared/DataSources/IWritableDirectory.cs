namespace PoeSharp.Shared.DataSources
{
    public interface IWritableDirectory : IDirectory
    {
        IWritableFile GetOrCreateFile(string name);
        IWritableDirectory GetOrCreateDirectory(string name);

        void Delete();
    }
}