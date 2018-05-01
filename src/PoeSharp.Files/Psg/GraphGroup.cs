using System.Collections.Generic;
using System.Linq;
using PoeSharp.Shared;

namespace PoeSharp.Files.Psg
{
    public class GraphGroup : ReadOnlyListBase<GraphNode>
    {
        internal GraphGroup(int id, float x, float y, IEnumerable<GraphNode> nodes)
        {
            Id = id;
            X = x;
            Y = y;
            Underlying = nodes.ToList();
        }

        public int Id { get; }
        public float X { get; }
        public float Y { get; }
    }
}
