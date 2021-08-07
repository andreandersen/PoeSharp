
using PoeSharp.Filetypes.BuildingBlocks;

namespace PoeSharp.Filetypes.Bundle
{
    public static class BundlesExtensions
    {
        public static BundleIndex OpenBundleIndex(this IDirectory bundlesDir) => new(bundlesDir);
    }
}