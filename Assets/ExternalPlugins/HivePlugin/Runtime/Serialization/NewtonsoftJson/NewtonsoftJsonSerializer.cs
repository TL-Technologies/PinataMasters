using Newtonsoft.Json;
using System;
using System.IO;

namespace Modules.Hive.Serialization
{
    public class NewtonsoftJsonSerializer : NewtonsoftJsonSerializerBase, IStreamSerializer, IStringSerializer
    {
        public NewtonsoftJsonSerializer() { }
        public NewtonsoftJsonSerializer(JsonSerializerSettings settings) : base(settings) { }

        
        public void Serialize(Stream stream, object value)
        {
            var serializer = CreateDefaultSerializer();

            using (var streamWriter = new StreamWriter(stream))
            using (var jsonTextWriter = new JsonTextWriter(streamWriter))
            {
                serializer.Serialize(jsonTextWriter, value);
            }
        }
        

        public object Deserialize(Stream stream, Type type)
        {
            var serializer = CreateDefaultSerializer();

            using (var streamReader = new StreamReader(stream))
            using (var jsonTextReader = new JsonTextReader(streamReader))
            {
                return serializer.Deserialize(jsonTextReader, type);
            }
        }
        

        public string Serialize(object value)
        {
            return JsonConvert.SerializeObject(value, serializerSettings);
        }
        

        public object Deserialize(string value, Type type)
        {
            return JsonConvert.DeserializeObject(value, type, serializerSettings);
        }
    }
}
