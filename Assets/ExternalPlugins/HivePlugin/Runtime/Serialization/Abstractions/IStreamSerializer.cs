using System;
using System.IO;


namespace Modules.Hive.Serialization
{
    public interface IStreamSerializer : ISerializer
    {
        void Serialize(Stream stream, object value);
        object Deserialize(Stream stream, Type type);
    }
}
