using Newtonsoft.Json;
using Newtonsoft.Json.Bson;
using System;
using System.IO;


namespace Modules.Hive.Serialization
{
    public class NewtonsoftBsonSerializer : NewtonsoftJsonSerializerBase, IStreamSerializer, IStringSerializer
    {
        public NewtonsoftBsonSerializer() { }
        public NewtonsoftBsonSerializer(JsonSerializerSettings settings) : base(settings) { }
        

        public void Serialize(Stream stream, object value)
        {
            var serializer = CreateDefaultSerializer();

            using (var binaryWriter = new BinaryWriter(stream))
            using (var bsonWriter = new BsonWriter(binaryWriter))
            {
                serializer.Serialize(bsonWriter, value);
            }
        }
        

        public object Deserialize(Stream stream, Type type)
        {
            var serializer = CreateDefaultSerializer();

            using (var binaryReader = new BinaryReader(stream))
            using (var bsonReader = new BsonReader(binaryReader))
            {
                return serializer.Deserialize(bsonReader, type);
            }
        }
        

        public string Serialize(object value)
        {
            byte[] buffer;
            using (MemoryStream stream = new MemoryStream())
            {
                Serialize(stream, value);
                buffer = stream.ToArray();
            }

            return Convert.ToBase64String(buffer);
        }
        

        public object Deserialize(string value, Type type)
        {
            byte[] buffer = Convert.FromBase64String(value);
            using (MemoryStream stream = new MemoryStream(buffer))
            {
                return Deserialize(stream, type);
            }
        }
    }
}
