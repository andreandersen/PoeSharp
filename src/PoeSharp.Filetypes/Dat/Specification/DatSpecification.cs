using System.Collections.Generic;

namespace PoeSharp.Filetypes.Dat.Specification
{
    public class DatSpecification
    {
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public Dictionary<string, DatField> Fields { get; set; }
        public Dictionary<string, VirtualFieldData> VirtualFields { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    }
}
