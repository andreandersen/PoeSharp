namespace PoeSharp.Filetypes.StatDescriptions.SyntaxTree
{
    public class Include : IInstruction
    {
        public Include(string file)
        {
            File = file;
        }

        public string File { get; }
    }
}
