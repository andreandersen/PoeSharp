namespace PoeSharp.Files.StatDescriptions.SyntaxTree
{
    public class NoDescription : IInstruction
    {
        public NoDescription(string stat)
        {
            Stat = stat;
        }

        public string Stat { get; }
    }
}
