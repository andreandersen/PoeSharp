using System;

namespace PoeSharp.Filetypes.Dat.Specification
{
    public class VirtualFieldData
    {
        public string[] Fields { get; } = Array.Empty<string>();
        public bool Zip { get; }
    }
}
