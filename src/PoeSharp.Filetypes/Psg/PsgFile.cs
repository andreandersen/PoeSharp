using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

using Microsoft.Toolkit.HighPerformance;

using PoeSharp.Filetypes.BuildingBlocks;

namespace PoeSharp.Filetypes.Psg
{
    public class PsgFile
    {
        private readonly List<GraphGroup> _groups;
        private readonly List<GraphRoot> _roots;

        public PsgFile(IFile file)
        {
            using (var stream = file.GetStream())
            {
                stream.Position = 1;
                stream.Seek(stream.ReadByte(), SeekOrigin.Current);

                // rootLength should be 7, the number of classes in the game
                //var rootLength = (int)stream.ReadUInt32();
                var rootLength = stream.Read<int>();

                _roots = stream
                    .Read<uint>(rootLength).Span
                    .ToArray().ToList()
                    .Select(c => new GraphRoot(c)).ToList();

                _groups = ExtractGroupNodes(stream);
            }
        }

        public IReadOnlyList<GraphRoot> Roots => _roots;
        public IReadOnlyList<GraphGroup> Groups => _groups;

        private static List<GraphGroup> ExtractGroupNodes(Stream stream)
        {
            var groupLength = stream.Read<uint>();

            return Enumerable.Range(0, (int)groupLength)
                .Select(i =>
                {
                    var groupStruct = stream.Read<GraphGroupStruct>();

                    var nodes = Enumerable.Range(0, (int)groupStruct.PassiveLength)
                        .Select(__ =>
                        {
                            var nodeStruct = stream.Read<GraphGroupNodeStruct>();
                            var connections = nodeStruct.ConnectionLength > 0
                                ? stream.Read<uint>((int)nodeStruct.ConnectionLength)
                                    .Span.ToArray().ToList()
                                : new List<uint>();

                            return new GraphNode(nodeStruct.RowId, nodeStruct.Radius,
                                nodeStruct.Position, connections);
                        });

                    return new GraphGroup(i, groupStruct.X, groupStruct.Y, nodes);
                }).ToList();
        }

        [StructLayout(LayoutKind.Sequential)]
        private readonly struct GraphGroupStruct
        {
            public readonly float X;
            public readonly float Y;
            public readonly uint PassiveLength;
        }

        [StructLayout(LayoutKind.Sequential)]
        private readonly struct GraphGroupNodeStruct
        {
            public readonly uint RowId;
            public readonly uint Radius;
            public readonly uint Position;
            public readonly uint ConnectionLength;
        }
    }
}
