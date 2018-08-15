namespace PoeSharp.Files.Ggpk.Records
{
    /// <summary>
    /// GGPK header field: Type, 4 bytes converted to int
    /// for performance reasons.
    /// </summary>
    internal enum RecordType
    {
        File = 1162627398, // ASCI 'FILE'
        Free = 1162170950, // ASCI 'FREE'
        Directory = 1380533328, // ASCII 'PDIR'
        Ggpk = 1263552327 // ASCI 'GGPK'
    }
}
