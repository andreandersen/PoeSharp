using System.Collections.Generic;
using System.IO;
using System.Reflection;
using PoeSharp.Shared.DataDeserializers;

namespace PoeSharp.Files.Dat.Specification
{
    public class DetSpecificationIndex : Dictionary<string, DatSpecification>
    {
        public static DetSpecificationIndex Default
        {
            get
            {
                var programDirectory = new FileInfo(Assembly.GetExecutingAssembly().Location).Directory.FullName;
                var specFile = Path.Combine(programDirectory, "spec.json");
                var deserializer = new JsonDeserializer<DetSpecificationIndex>(JsonDeserializer<DetSpecificationIndex>.NamingConvention.SnakeCase);
                var specIndex = deserializer.Deserialize(File.ReadAllText(specFile));
                return specIndex;
            }
        }
    }
}
