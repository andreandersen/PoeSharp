using System.Collections.Generic;

namespace PoeSharp.Filetypes.Psg
{
    public class GraphNode
    {
        internal GraphNode(uint passiveSkill, uint radius, uint position, 
            IReadOnlyList<uint> connections)
        {
            PassiveSkill = passiveSkill;
            Radius = radius;
            Position = position;
            Connections = connections;
        }

        public uint PassiveSkill { get; }
        public uint Radius { get; }
        public uint Position { get; }
        public IReadOnlyList<uint> Connections { get; }
    }
}
