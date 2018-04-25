using System.Collections.Generic;

namespace PoeSharp.Files.Dat.Specification
{
    public class DatSpecification
    {
        public Dictionary<string, FieldSpecification> Fields { get; set; }
        public Dictionary<string, VirtualFieldData> VirtualFields { get; set; }
    }
}
