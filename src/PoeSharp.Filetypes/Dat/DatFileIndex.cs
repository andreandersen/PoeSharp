namespace PoeSharp.Filetypes.Dat
{
    public sealed class DatFileIndex : ReadOnlyDictionaryBase<string, DatFile>
    {
        public DatFileIndex(IDirectory directory, DatSpecIndex specIndex, bool lazyLoad = true)
        {
            var files = directory.Files.Values.Where(c => c.Extension.StartsWith(".dat")).ToArray();
            if (files.Length == 0)
            {
                var dataDirectory = directory.Directories.Values.FirstOrDefault(c => c.Name == "Data");
                if (dataDirectory != null)
                {
                    files = dataDirectory.Files.Values.Where(c => c.Extension.StartsWith(".dat")).ToArray();
                }
            }

            foreach (var file in files)
            {
                var datName = file.Name[0..^(file.Extension.Length)];

                if (Underlying.ContainsKey(file.Name) ||
                    !specIndex.ContainsKey(datName))
                {
                    continue;
                }

                var dat = new DatFile(file, this, specIndex[datName]);
                Underlying.Add(file.Name, dat);
            }
        }
    }
}