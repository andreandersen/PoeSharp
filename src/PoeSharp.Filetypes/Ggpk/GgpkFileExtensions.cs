namespace PoeSharp.Filetypes.Ggpk
{
    public static class GgpkFileExtensions
    {
        public static void Extract(this IFile file, string path)
        {
            var destFile = new FileInfo(path);

            if (!destFile.Directory!.Exists)
                destFile.Directory.Create();

            using var fs = destFile.Exists ? destFile.OpenWrite() : destFile.Create();
            file.CopyToStream(fs);
        }
    }
}