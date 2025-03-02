using Modules.Hive.Serialization.NewtonsoftJson;
using Newtonsoft.Json;

namespace Modules.Hive.Serialization
{
    public abstract class NewtonsoftJsonSerializerBase
    {
        public readonly JsonSerializerSettings serializerSettings;

        
        public NewtonsoftJsonSerializerBase()
        {
            serializerSettings = CreateDefaultSettings();
        }

        
        public NewtonsoftJsonSerializerBase(JsonSerializerSettings settings)
        {
            serializerSettings = settings ?? CreateDefaultSettings();
        }

        
        public static JsonSerializerSettings CreateDefaultSettings()
        {
            var settings = new JsonSerializerSettings();

            // Fix to avoid item duplications in pre-initialized collections (https://stackoverflow.com/a/34819288)
            settings.ObjectCreationHandling = ObjectCreationHandling.Replace;

            // Add converters
            settings.Converters.Add(new ColorConverter());

            return settings;
        }
        

        protected virtual JsonSerializer CreateDefaultSerializer()
        {
            var serializer = JsonSerializer.CreateDefault(serializerSettings);

            return serializer;
        }
    }
}
