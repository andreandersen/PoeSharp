using System.Linq;

using PoeSharp.Filetypes.BuildingBlocks;
using PoeSharp.Filetypes.Dat.Specification;

namespace PoeSharp.Filetypes.Dat
{
    public sealed class DatFileIndex : ReadOnlyDictionaryBase<string, DatFile>
    {
        public DatFileIndex(IDirectory directory, DatSpecIndex specIndex, bool lazyLoad = true)
        {
            var files = directory.Files.Where(c => c.Name.EndsWith(".dat")).ToArray();
            if (files.Length == 0)
            {
                var dataDirectory = directory.Directories.FirstOrDefault(c => c.Name == "Data");
                if (dataDirectory != null)
                {
                    files = dataDirectory.Files.Where(c => c.Name.EndsWith(".dat")).ToArray();
                }
            }

            foreach (var file in files)
            {
                if (Underlying.ContainsKey(file.Name) ||
                    !specIndex.ContainsKey(file.Name))
                {
                    continue;
                }

                var dat = new DatFile(file, this, specIndex[file.Name]);
                Underlying.Add(file.Name, dat);
            }
        }
    }
}
