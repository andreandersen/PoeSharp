var dir = new GgpkFileSystem(@"D:\Games\Path of Exile\Content.ggpk")
    .Root.OpenBundleIndex()
    .Root.Directories["Data"];

var datFileIndex = new DatFileIndex(dir, DatSpecIndex.Default);

var dat = datFileIndex["Mods.dat"];

for (var i = 0; i < dat.RowCount; i++)
{
    var row = dat[i];
    var mod = new Mod(
        row["Id"], row["Hash"], row["Level"], row["Domain"],
        row["Name"], row["GenerationType"], row["CorrectGroup"],
        row["Stat1Min"], row["Stat1Max"]);

    if (i % 1000 == 0)
        Console.WriteLine(mod);
}

public record Mod(string Id, int Hash, int Level, int Domain, string Name,
    int GenerationType, string CorrectGroup, int Stat1Min, int Stat1Max);