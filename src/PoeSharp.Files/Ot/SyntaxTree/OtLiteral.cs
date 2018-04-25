namespace PoeSharp.Files.Ot.SyntaxTree
{
    public class OtLiteral
    {
        public OtLiteral(object value, LiteralType literalType)
        {
            Value = value;
            LiteralType = literalType;
        }

        public LiteralType LiteralType { get; }
        public object Value { get; }
    }
}
