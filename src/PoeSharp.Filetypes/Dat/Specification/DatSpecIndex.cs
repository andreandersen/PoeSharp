using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

using PoeSharp.Filetypes.BuildingBlocks;

namespace PoeSharp.Filetypes.Dat.Specification
{
    public class DatSpecIndex : Dictionary<string, DatSchema>
    {
        public static DatSpecIndex Default
        {
            get
            {
                var programDirectory = AppContext.BaseDirectory;
                var specFile = Path.Combine(programDirectory, "spec.json");

                var options = new JsonSerializerOptions
                {
                    PropertyNamingPolicy = new JsonSnakeCaseNamingPolicy()
                };

                options.Converters.Add(new BooleanConverter());

                var specIndex = JsonSerializer.Deserialize<DatSpecIndex>(
                    File.ReadAllText(specFile), options);

                return specIndex ??
                    throw new InvalidOperationException(
                        "Could not deserialize Dat specifications");
            }
        }
    }
}
