using System;
using System.Collections.Generic;
using System.Linq;

namespace PoeSharp.Filetypes.Ot.SyntaxTree
{
    public class OtFile
    {
        public OtFile(string version, string extends, 
            IReadOnlyList<OtObject?> objects)
        {
            Name = string.Empty;
            Version = version;
            Extends = extends;
            Objects = objects == null ? 
                Array.Empty<OtObject>().ToList() : 
                objects.Where(p => p is not null).ToList()!;
        }

        public string Extends { get; }
        public string Name { get; }
        public IReadOnlyList<OtObject> Objects { get; }
        public string Version { get; }
    }
}
