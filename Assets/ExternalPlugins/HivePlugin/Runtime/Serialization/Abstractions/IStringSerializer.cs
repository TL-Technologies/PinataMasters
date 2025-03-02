using System;


namespace Modules.Hive.Serialization
{
    public interface IStringSerializer : ISerializer
    {
        string Serialize(object value);
        object Deserialize(string value, Type type);
    }
}
