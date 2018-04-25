namespace PoeSharp.Metadata.Translations
{
    public class TranslationParameter
    {

        public TranslationParameter(string param, string arg)
        {
            Parameter = param;
            Argument = arg;
        }
        public string Argument { get; }
        public string Parameter { get; }
    }
}
