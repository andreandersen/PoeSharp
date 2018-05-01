using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using PoeSharp.Shared.DataSources;
using PoeSharp.Util;

namespace PoeSharp.Files.Psg
{
    public class PsgFile
    {
        private readonly List<GraphGroup> _groups;
        private readonly List<GraphRoot> _roots;

        public PsgFile(IFile file)
        {
            using (Stream stream = file.GetStream())
            {
                stream.Position = 1;
                stream.Seek(stream.ReadByte(), SeekOrigin.Current);

                // rootLength should be 7, the number of classes in the game
                int rootLength = (int)stream.ReadUInt32();

                _roots = stream
                    .Read<uint>(rootLength)
                    .ToArray().ToList()
                    .Select(c => new GraphRoot(c)).ToList();

                _groups = ExtractGroupNodes(stream);
            }
        }

        public IReadOnlyList<GraphRoot> Roots => _roots;
        public IReadOnlyList<GraphGroup> Groups => _groups;

        private static List<GraphGroup> ExtractGroupNodes(Stream stream)
        {
            uint groupLength = stream.ReadUInt32();

            return Enumerable.Range(0, (int)groupLength)
                .Select(i =>
                {
                    GraphGroupStruct groupStruct = stream.Read<GraphGroupStruct>();

                    IEnumerable<GraphNode> nodes = Enumerable.Range(0, (int)groupStruct.PassiveLength)
                        .Select(__ =>
                        {
                            GraphGroupNodeStruct nodeStruct = stream.Read<GraphGroupNodeStruct>();
                            List<uint> connections = nodeStruct.ConnectionLength > 0
                                ? stream.Read<uint>((int)nodeStruct.ConnectionLength)
                                    .ToArray().ToList()
                                : new List<uint>();

                            return new GraphNode(nodeStruct.RowId, nodeStruct.Radius,
                                nodeStruct.Position, connections);
                        });

                    return new GraphGroup(i, groupStruct.X, groupStruct.Y, nodes);
                }).ToList();
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct GraphGroupStruct
        {
            public readonly float X;
            public readonly float Y;
            public readonly uint PassiveLength;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct GraphGroupNodeStruct
        {
            public readonly uint RowId;
            public readonly uint Radius;
            public readonly uint Position;
            public readonly uint ConnectionLength;
        }
    }
}
