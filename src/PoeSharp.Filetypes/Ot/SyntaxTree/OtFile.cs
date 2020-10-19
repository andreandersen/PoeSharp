using System.Collections.Generic;

namespace PoeSharp.Filetypes.Ot.SyntaxTree
{
    public class OtFile
    {
        public OtFile(string version, string extends, 
            IReadOnlyList<OtObject> objects)
        {
            Version = version;
            Extends = extends;
            Objects = objects;
        }

        public string Extends { get; }
        public string Name { get; }
        public IReadOnlyList<OtObject> Objects { get; }
        public string Version { get; }
    }
}
