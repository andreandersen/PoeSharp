using System.Collections.Generic;

namespace PoeSharp.Filetypes.Ot.SyntaxTree
{
    public class OtObject
    {
        public OtObject(string name, List<KeyValuePair<string, string>> values)
        {
            Name = name;
            Values = values;
        }

        public string Name { get; }
        public IReadOnlyList<KeyValuePair<string, string>> Values { get; }
    }
}
