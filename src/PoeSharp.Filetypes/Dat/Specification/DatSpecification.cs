using System.Collections.Generic;

namespace PoeSharp.Filetypes.Dat.Specification
{
    public class DatSpecification
    {
        public Dictionary<string, DatField> Fields { get; set; }
        public Dictionary<string, VirtualFieldData> VirtualFields { get; set; }
    }
}
