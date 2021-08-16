using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

using PoeSharp.Filetypes.BuildingBlocks;

namespace PoeSharp.Filetypes.Dat.Specification
{
    public class DatSpecIndex 
        : ReadOnlyDictionaryBase<string, TableDefinition>
    {
        public int Version { get; set; }
        public DateTimeOffset CreatedAt { get; set; }

        public List<TableDefinition> Tables { get; set; }

        public DatSpecIndex(
            int version, DateTimeOffset createdAt,
            List<TableDefinition> tables)
        {
            Version = version;
            CreatedAt = createdAt;
            Tables = tables;
            Underlying = Tables.ToDictionary(p => p.Name);
        }

        public static DatSpecIndex Default => Create(@"spec.json");

        public static DatSpecIndex Create(string filepath)
        {
            string json;
            if (File.Exists(filepath))
                json = File.ReadAllText(filepath);
            else
                json = File.ReadAllText(Path.Combine(AppContext.BaseDirectory, filepath));

            return JsonSerializer.Deserialize<DatSpecIndex>(
                json, GetJsonSerializerOptions())!;
        }

        public static ValueTask<DatSpecIndex> Create(Stream utf8Stream) =>
            JsonSerializer.DeserializeAsync<DatSpecIndex>(
                utf8Stream, GetJsonSerializerOptions())!;

        public static DatSpecIndex Create(byte[] utf8Bytes) =>
            JsonSerializer.Deserialize<DatSpecIndex>(
                utf8Bytes, GetJsonSerializerOptions())!;

        public static DatSpecIndex Create(Span<byte> utf8Bytes) =>
            JsonSerializer.Deserialize<DatSpecIndex>(
                utf8Bytes, GetJsonSerializerOptions())!;

        public static JsonSerializerOptions GetJsonSerializerOptions()
        {
            var opts = new JsonSerializerOptions();
            opts.Converters.Add(new DatSpecJsonConverter());
            return opts;
        }
    }

    public class TableDefinition 
        : ReadOnlyDictionaryBase<string, ColumnDefinition>
    {
        public string Name { get; }

        public List<ColumnDefinition> Columns { get; }

        public TableDefinition(string name, List<ColumnDefinition> columns)
        {
            Name = name;
            Columns = columns;
            Underlying = Columns.ToDictionary(p => p.Name!);
        }
    }


    public record ColumnDefinition(string Name, string Description,
        bool Array, ColumnType Type, bool Unique, bool Localized,
        ReferenceDefinition? Reference, string? Until, int Offset32, int Size32,
        int Offset64, int Size64);

    public record ReferenceDefinition(string Table, string? Column);

    public enum ColumnType
    {
        Array, Bool, F32, ForeignRow, I16, I32,
        I64, I8, Row, String, U32, U64, Unknown
    };

    public static class ColumnTypeExtensions
    {
        public static int GetSize64(this ColumnType type) =>
            type switch
            {
                ColumnType.ForeignRow => 16,
                ColumnType.Array => 16,
                ColumnType.Row => 8,
                ColumnType.I64 => 8,
                ColumnType.U64 => 8,
                ColumnType.String => 8,
                ColumnType.F32 => 4,
                ColumnType.I32 => 4,
                ColumnType.U32 => 4,
                ColumnType.I16 => 2,
                ColumnType.I8 => 1,
                ColumnType.Bool => 1,
                _ => throw new NotImplementedException()
            };

        public static int GetSize32(this ColumnType type) =>
            type switch
            {
                ColumnType.ForeignRow => 8,
                ColumnType.Array => 8,
                ColumnType.Row => 4,
                ColumnType.I64 => 8,
                ColumnType.U64 => 8,
                ColumnType.String => 4,
                ColumnType.F32 => 4,
                ColumnType.I32 => 4,
                ColumnType.U32 => 4,
                ColumnType.I16 => 2,
                ColumnType.I8 => 1,
                ColumnType.Bool => 1,
                _ => throw new NotImplementedException()
            };
    }
}
