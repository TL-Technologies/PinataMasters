using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using UnityEngine;


namespace Modules.Hive.Serialization.NewtonsoftJson
{
    public class ColorConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType) => objectType == typeof(Color) || objectType == typeof(Color32);
        

        public override object ReadJson(
            JsonReader reader,
            Type objectType,
            object existingValue,
            JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
            {
                return new Color();
            }

            JObject jObject = JObject.Load(reader);
            // ISSUE: type reference
            return objectType == typeof(Color32) ?
                (object) new Color32((byte)jObject["r"], (byte)jObject["g"], (byte)jObject["b"], (byte)jObject["a"]) :
                new Color((float)jObject["r"], (float)jObject["g"], (float)jObject["b"], (float)jObject["a"]);
        }

        
        public override void WriteJson(
            JsonWriter writer,
            object value,
            JsonSerializer serializer)
        {
            if (value == null)
            {
                writer.WriteNull();
            }
            else
            {
                Color color = value is Color castedColor ? castedColor : (Color)(Color32)value;
                writer.WriteStartObject();
                writer.WritePropertyName("a");
                writer.WriteValue(color.a);
                writer.WritePropertyName("r");
                writer.WriteValue(color.r);
                writer.WritePropertyName("g");
                writer.WriteValue(color.g);
                writer.WritePropertyName("b");
                writer.WriteValue(color.b);
                writer.WriteEndObject();
            }
        }
    }
}
