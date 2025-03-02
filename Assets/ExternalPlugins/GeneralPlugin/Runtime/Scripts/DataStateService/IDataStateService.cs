using System;


namespace Modules.General
{
    public interface IDataStateService
    {
        T Get<T>(string path, T defaultValue = default);
        bool TryGet<T>(string path, out T value);
        void Set<T>(string path, T value, bool writeToPrefs = true);
        void Remove(string path);
        void AddListener(string valueName, Action<object, object> callback);
        void RemoveListener(string valueName);
    }
}