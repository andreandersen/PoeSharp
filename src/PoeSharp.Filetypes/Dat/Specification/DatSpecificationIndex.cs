using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text.Json;

using PoeSharp.Filetypes.BuildingBlocks;

namespace PoeSharp.Filetypes.Dat.Specification
{
    public class DetSpecificationIndex : Dictionary<string, DatSpecification>
    {
        public static DetSpecificationIndex Default
        {
            get
            {
                var programDirectory = new FileInfo(
                    Assembly.GetExecutingAssembly().Location).Directory!.FullName;

                var specFile = Path.Combine(programDirectory, "spec.json");

                var options = new JsonSerializerOptions
                {
                    PropertyNamingPolicy = new JsonSnakeCaseNamingPolicy()
                };

                options.Converters.Add(new BooleanConverter());

                var specIndex = JsonSerializer.Deserialize<DetSpecificationIndex>(
                    File.ReadAllText(specFile), options);

                return specIndex;
            }
        }
    }
}
