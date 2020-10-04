using System;
using System.IO;
using System.Linq;

using Microsoft.Toolkit.HighPerformance.Extensions;

using PoeSharp.Filetypes.Bundle.Internal;

namespace PoeSharp.Filetypes.Bundle
{
    public static class BundleIndex
    {
        public static void C(Stream src)
        {
            var bundleCount = (int)src.Read<uint>();
            var bundleInfo = new BundleInfo[bundleCount];

            for (var i = 0; i < bundleCount; i++)
            {
                var nameLen = src.Read<int>();
                var name = src.ReadUtf8String(nameLen);
                var uncompressedSize = src.Read<int>();

                bundleInfo[i] = new BundleInfo(
                    uncompressedSize, name.ToString());
            }

            var fileCount = (int)src.Read<uint>();
            using var filesOwner = src.Read<BundleFileInfo>(fileCount);
            var files = filesOwner.Span;

            var pathInfoCount = (int)src.Read<uint>();
            var pathInfoOwner = src.Read<BundlePathInfo>(pathInfoCount);
            var pathInfo = pathInfoOwner.Span;

            //var fileHashMap = files
            //    .ToArray()
            //    .ToDictionary(p => p.Hash, p => p);

            var pathSorted = pathInfo
                .ToArray();

            var pathHashMap = pathSorted
                .ToDictionary(p => p.Hash, p => p);

            var length = src.Length - src.Position;
            var innerBundle = src.DecompressBundle();

            foreach (var path in pathInfo)
            {
                var yo = path.MapFilenames(innerBundle);
            }

        }
    }
}
