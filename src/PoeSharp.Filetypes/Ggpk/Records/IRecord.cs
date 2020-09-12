using System;

namespace PoeSharp.Filetypes.Ggpk.Records
{
    public interface IRecord
    {
        int Length { get; }

        /// <summary>
        /// UTF-8 representation in bytes
        /// </summary>
        ReadOnlyMemory<char> Name { get; }

        long Offset { get; }
    }
}
