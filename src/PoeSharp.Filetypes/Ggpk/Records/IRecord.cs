using System;
using System.Text.Utf8;

namespace PoeSharp.Filetypes.Ggpk.Records
{
    public interface IRecord
    {
        int Length { get; }

        /// <summary>
        /// UTF-8 representation in bytes
        /// </summary>
        ReadOnlyMemory<byte> Name { get; }

        long Offset { get; }
    }
}
