namespace Modules.Hive.Storage
{
    public interface IKeyValueStorage
    {
        //int Count { get; }
        bool ContainsKey(string key);
        bool Remove(string key);
        bool TryGetValue(string key, out string value);
        void Save();

        string this[string key] { get; set; }
    }
}
