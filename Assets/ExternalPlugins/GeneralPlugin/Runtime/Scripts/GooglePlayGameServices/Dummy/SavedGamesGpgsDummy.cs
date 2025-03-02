using Modules.General.Abstraction.GooglePlayGameServices;
using System;


namespace Modules.General.GooglePlayGameServices
{
    internal class SavedGamesGpgsDummy : ISavedGamesGpgs
    {
        public event Action<bool> OnProgressLoad;
        public event Action<bool> OnProgressSaved;


        public bool IsAuthenticated => false;
        public bool IsLoaded => false;


        public void Initialize() { }


        public void SetBool(string key, bool value, bool shouldSaveImmediately = false) { }
        public void SetFloat(string key, float value, bool shouldSaveImmediately = false) { }
        public void SetDouble(string key, double value, bool shouldSaveImmediately = false) { }
        public void SetInt(string key, int value, bool shouldSaveImmediately = false) { }
        public void SetDateTime(string key, DateTime value, bool shouldSaveImmediately = false) { }
        public void SetString(string key, string value, bool shouldSaveImmediately = false) { }


        public bool GetBool(string key, bool defaultValue = default) => default;
        public float GetFloat(string key, float defaultValue = default) => default;
        public double GetDouble(string key, double defaultValue = default) => default;
        public int GetInt(string key, int defaultValue = default) => default;
        public DateTime GetDateTime(string key, DateTime defaultValue) => default;
        public string GetString(string key, string defaultValue) => default;


        public bool HasKey(string key) => false;
        public void DeleteKey(string key) { }
        public void DeleteAll() { }
        public void Save() { }
        public void LoadGameProgress() { }
        public void DeleteGameProgress() { }
    }
}
