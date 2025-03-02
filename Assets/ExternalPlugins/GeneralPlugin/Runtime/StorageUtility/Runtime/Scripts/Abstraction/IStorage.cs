namespace Modules.General.Utilities.StorageUtility
{
    public interface IStorage
    {
        void Initialize();
        string Load(string key);
        void Save(string key, string value);
        void Remove(string key);
    }
}
