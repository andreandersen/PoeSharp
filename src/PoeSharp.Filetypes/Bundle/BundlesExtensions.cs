namespace PoeSharp.Filetypes.Bundle
{
    public static class BundlesExtensions
    {
        /// <summary>
        /// Creates a Bundle Index from either the root directory or Bundles2 folder
        /// </summary>
        /// <param name="bundlesDir"></param>
        /// <returns></returns>
        public static BundleIndex OpenBundleIndex(this IDirectory bundlesDir)
        {
            if (bundlesDir.Files.ContainsKey(BundleIndex.IndexFileName))
                return new(bundlesDir);

            if (bundlesDir.Directories.TryGetValue(BundleIndex.BundlesFolder, out var dir))
                return new(dir);

            throw new ArgumentException("Cant' find bundle index with supplied directory");
        }
    }
}