using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using Microsoft.Toolkit.HighPerformance;
using Microsoft.Toolkit.HighPerformance.Extensions;

using PoeSharp.Filetypes.BuildingBlocks;
using PoeSharp.Filetypes.Bundle.Internal;

namespace PoeSharp.Filetypes.Bundle
{
    public class BundleIndex
    {
        private const string IndexFileName = "_.index.bin";
        private readonly IDirectory _sourceDir;
        private readonly BundleInfoRecord[] _bundles;

        public readonly BundledDirectory Root =
            new BundledDirectory("", null);

        public BundleIndex(IDirectory sourceDirectory)
        {
            if (sourceDirectory[IndexFileName] is not IFile indexFile)
                throw new ArgumentException("Directory does not contain a bundle index");

            _sourceDir = sourceDirectory;

            var src = indexFile.AsSpan();
            var buf = src.DecompressBundle();
            _bundles = ExtractBundles(ref buf);
            var files = ExtractFileInfo(ref buf);
            var paths = buf.ConsumeTo<BundlePathRecord>(buf.ConsumeTo<int>());
            var innerBundle = buf.DecompressBundle();

            foreach (var pathRecord in paths)
            {
                if (innerBundle.Length <= 8)
                    continue;

                var pathStrings = pathRecord.MapFilenames(innerBundle);

                if (pathStrings.Length == 0)
                    continue;

                var folderEnd = pathStrings[0].Path.LastIndexOf('/');
                var path = pathStrings[0].Path[0..folderEnd];

                var dir = GetDirectoryFromPath(path);

                foreach (var item in pathStrings)
                {
                    var fileName = Path.GetFileName(item.Path);
                    var fileRecord = files[item.Hash];
                    var bd = new BundledFile(this, fileRecord, fileName, dir);
                    dir.AddFile(bd);
                }
            }

        }

        internal Span<byte> GetContents(BundleFileRecord record)
        {
            var source = FindSourceFile(record);
            var compressed = source.AsSpan();
            var decompressed = BundleFile.Decompress(compressed, (int)record.FileOffset, (int)record.FileSize);
            //var decompressed = BundleFile.Decompress(compressed).Slice((int)record.FileOffset, (int)record.FileSize);
            return decompressed;
        }

        private IFile FindSourceFile(BundleFileRecord record)
        {
            const string bundleExt = ".bundle.bin";

            var bundle = _bundles[record.BundleIndex];
            string name = bundle.Name;
            var nameSpan = name.AsSpan();

            var currDir = _sourceDir;

            var folderEnd = name.LastIndexOf('/');
            if (folderEnd > 0)
            {
                var fileName = nameSpan[(folderEnd + 1)..];
                var path = name[0..folderEnd];
                
                var pathParts = path.Tokenize('/');

                foreach (var part in pathParts)
                {
                    currDir = currDir.Directories[part.ToString()];
                }

                return currDir.Files[fileName.ToString() + bundleExt];
            }

            return currDir.Files[name + bundleExt];
        }

        private BundledDirectory GetDirectoryFromPath(ReadOnlySpan<char> path)
        {
            var currLevel = Root;
            foreach (var part in path.Tokenize('/'))
            {
                var partString = part.ToString();
                if (currLevel[partString] is BundledDirectory bd)
                {
                    currLevel = bd;
                }
                else
                {
                    var newDir = new BundledDirectory(partString, currLevel);
                    currLevel.AddDirectory(newDir);
                    currLevel = newDir;
                }
            }

            return currLevel;
        }

        private static Dictionary<ulong, BundleFileRecord> ExtractFileInfo(
            ref Span<byte> buf)
        {
            var fileCount = buf.ConsumeTo<int>();
            var files = buf
                .ConsumeTo<BundleFileRecord>(fileCount)
                .ToArray()
                .ToDictionary(p => p.Hash);

            return files;
        }

        private static BundleInfoRecord[] ExtractBundles(ref Span<byte> buf)
        {
            var bundleCount = buf.ConsumeTo<int>();
            var bundleInfo = new BundleInfoRecord[bundleCount];

            for (var i = 0; i < bundleCount; i++)
            {
                var nameLen = buf.ConsumeTo<int>();
                var name = Encoding.UTF8.GetString(buf.Consume(nameLen));
                var uncompressedSize = buf.ConsumeTo<int>();

                bundleInfo[i] = new BundleInfoRecord(uncompressedSize, name);
            }

            return bundleInfo;
        }
    }

}
