using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace PoeSharp.Shared.DataDeserializers
{
    public class JsonDeserializer<TOut> : IDataDeserializer<string, TOut>
    {
        private readonly JsonSerializerSettings _jsonSettings;

        public JsonDeserializer(JsonSerializerSettings jsonSettings)
        {
            _jsonSettings = jsonSettings;
        }

        public JsonDeserializer(NamingConvention namingConvention = NamingConvention.CamelHump)
        {
            _jsonSettings = new JsonSerializerSettings();

            NamingStrategy selectedNamingStrategy;

            switch (namingConvention)
            {
                case NamingConvention.CamelHump:
                    selectedNamingStrategy = new CamelCaseNamingStrategy(true, false);
                    break;
                case NamingConvention.SnakeCase:
                    selectedNamingStrategy = new SnakeCaseNamingStrategy(true, false);
                    break;
                default:
                    selectedNamingStrategy = new DefaultNamingStrategy();
                    break;
            }

            _jsonSettings.ContractResolver = new PrivateSetterContractResolver
            {
                NamingStrategy = selectedNamingStrategy
            };
        }

        public TOut Deserialize(string serialized) =>
            JsonConvert.DeserializeObject<TOut>(serialized, _jsonSettings);

        public enum NamingConvention
        {
            PascalType,
            CamelHump,
            SnakeCase
        }
    }

    public class PrivateSetterContractResolver : DefaultContractResolver
    {
        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            var jProperty = base.CreateProperty(member, memberSerialization);
            if (jProperty.Writable)
                return jProperty;

            jProperty.Writable = member.IsPropertyWithSetter();

            return jProperty;
        }
    }
}