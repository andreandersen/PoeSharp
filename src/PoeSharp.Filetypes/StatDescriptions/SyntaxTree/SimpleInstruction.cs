namespace PoeSharp.Filetypes.StatDescriptions.SyntaxTree
{
    public class SimpleInstruction : IInstruction
    {
        public SimpleInstruction(string instruction)
        {
            Instruction = instruction;
        }

        public string Instruction { get; }
    }
}
