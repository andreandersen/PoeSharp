using System.Diagnostics;

namespace PoeSharp.Filetypes.Bundle
{
    /// <summary>
    /// Wrapper class to access files inside of the Bundles 
    /// normally located in the Bundles2 folder.
    /// </summary>
    public class BundleIndex
    {
        internal const string IndexFileName = "_.index.bin";
        internal const string BundlesFolder = "Bundles2";

        private readonly IDirectory _sourceDir;
        private readonly BundleInfoRecord[] _bundles;
        private readonly SimpleCache<IFile, byte[]> _compressedCache = new(16);
        private readonly SimpleCache<uint, IFile> _bundleIndexCache = new(256);

        /// <summary>
        /// Virtual Root Directory for Bundled directories (and files)
        /// </summary>
        public BundledDirectory Root { get; } = new("", null);

        /// <summary>
        /// Creates a Bundle Index wrapper to access inner files
        /// </summary>
        /// <param name="sourceDirectory">Directory of the Bundles2 directory</param>
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

        /// <summary>
        /// Clears the memory cache used when accessing/decompressing files
        /// </summary>
        public void ClearMemoryCache()
        {
            _bundleIndexCache.Clear();
            _compressedCache.Clear();
            GC.Collect();
        }

        internal Span<byte> GetContents(BundleFileRecord record)
        {
            var source = _bundleIndexCache.GetOrAdd(record.BundleIndex, () => FindSourceFile(record));
            var compressed = _compressedCache.GetOrAdd(source, () => source.AsSpan().ToArray());
            var decompressed = BundleFile.Decompress(
                compressed, (int)record.FileOffset, (int)record.FileSize);
            return decompressed;
        }

        private IFile FindSourceFile(BundleFileRecord record)
        {
            const string bundleExt = ".bundle.bin";
            var bundle = _bundles[record.BundleIndex];
            var name = bundle.Name;
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