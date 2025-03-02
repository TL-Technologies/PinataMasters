using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using UnityEngine;
using UnityEngine.Scripting;


namespace Modules.Hive.Serialization.NewtonsoftJson
{
    /// <summary>
    /// Json Converter for Vector2, Vector3 and Vector4. Only serializes x, y, (z) and (w) properties.
    /// </summary>
    [Preserve]
    public class VectorConverter : JsonConverter
    {
        private static readonly Type V2 = typeof(Vector2);
        private static readonly Type V3 = typeof(Vector3);
        private static readonly Type V4 = typeof(Vector4);
    
        
        public bool EnableVector2 { get; set; }
        public bool EnableVector3 { get; set; }
        public bool EnableVector4 { get; set; }
    
        
        /// <summary>
        /// Default Constructor - All Vector types enabled by default
        /// </summary>
        public VectorConverter()
        {
            EnableVector2 = true;
            EnableVector3 = true;
            EnableVector4 = true;
        }
    
        
        /// <summary>Selectively enable Vector types</summary>
        /// <param name="enableVector2">Use for Vector2 objects</param>
        /// <param name="enableVector3">Use for Vector3 objects</param>
        /// <param name="enableVector4">Use for Vector4 objects</param>
        public VectorConverter(bool enableVector2, bool enableVector3, bool enableVector4) : this()
        {
            EnableVector2 = enableVector2;
            EnableVector3 = enableVector3;
            EnableVector4 = enableVector4;
        }
    

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value == null)
            {
                writer.WriteNull();
            }
            else
            {
                Type type = value.GetType();
                if (type == V2)
                {
                    Vector2 vector2 = (Vector2) value;
                    WriteVector(writer, vector2.x, vector2.y, null, null);
                }
                else if (type == V3)
                {
                    Vector3 vector3 = (Vector3) value;
                    WriteVector(writer, vector3.x, vector3.y, vector3.z, null);
                }
                else if (type == V4)
                {
                    Vector4 vector4 = (Vector4) value;
                    WriteVector(writer, vector4.x, vector4.y, vector4.z, vector4.w);
                }
                else
                {
                    writer.WriteNull();
                }
            }
        }
        
    
        private static void WriteVector(JsonWriter writer, float x, float y, float? z, float? w)
        {
            writer.WriteStartObject();
            writer.WritePropertyName(nameof(x));
            writer.WriteValue(x);
            writer.WritePropertyName(nameof(y));
            writer.WriteValue(y);
            if (z.HasValue)
            {
                writer.WritePropertyName(nameof (z));
                writer.WriteValue(z.Value);
                if (w.HasValue)
                {
                    writer.WritePropertyName(nameof (w));
                    writer.WriteValue(w.Value);
                }
            }
            writer.WriteEndObject();
        }
    

        public override object ReadJson(
          JsonReader reader,
          Type objectType,
          object existingValue,
          JsonSerializer serializer)
        {
            if (objectType == V2)
            {
                return PopulateVector2(reader);
            }

            return objectType == V3 ? (object)PopulateVector3(reader) : PopulateVector4(reader);
        }
    

        public override bool CanConvert(Type objectType)
        {
            if (EnableVector2 && objectType == V2 || EnableVector3 && objectType == V3)
            {
                return true;
            }

            return EnableVector4 && objectType == V4;
        }
    
        
        private static Vector2 PopulateVector2(JsonReader reader)
        {
            Vector2 vector2 = new Vector2();
            if (reader.TokenType != JsonToken.Null)
            {
                JObject jObject = JObject.Load(reader);
                vector2.x = jObject["x"].Value<float>();
                vector2.y = jObject["y"].Value<float>();
            }
            return vector2;
        }
    
        
        private static Vector3 PopulateVector3(JsonReader reader)
        {
            Vector3 vector3 = new Vector3();
            if (reader.TokenType != JsonToken.Null)
            {
                JObject jObject = JObject.Load(reader);
                vector3.x = jObject["x"].Value<float>();
                vector3.y = jObject["y"].Value<float>();
                vector3.z = jObject["z"].Value<float>();
            }
            return vector3;
        }
        
    
        private static Vector4 PopulateVector4(JsonReader reader)
        {
            Vector4 vector4 = new Vector4();
            if (reader.TokenType != JsonToken.Null)
            {
                JObject jObject = JObject.Load(reader);
                vector4.x = jObject["x"].Value<float>();
                vector4.y = jObject["y"].Value<float>();
                vector4.z = jObject["z"].Value<float>();
                vector4.w = jObject["w"].Value<float>();
            }
            return vector4;
        }
    }
}
