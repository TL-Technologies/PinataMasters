using System;


namespace Modules.General.Abstraction.GooglePlayGameServices
{
    public interface ISavedGamesGpgs
    {
        event Action<bool> OnProgressLoad;
        event Action<bool> OnProgressSaved; 
        
        bool IsAuthenticated { get; }
        bool IsLoaded { get; }

        void Initialize();
        
        void SetBool(string key, bool value, bool shouldSaveImmediately = false);
        void SetFloat(string key, float value, bool shouldSaveImmediately = false);
        void SetDouble(string key, double value, bool shouldSaveImmediately = false);
        void SetInt(string key, int value, bool shouldSaveImmediately = false);
        void SetDateTime(string key, DateTime value, bool shouldSaveImmediately = false);
        void SetString(string key, string value, bool shouldSaveImmediately = false);

        bool GetBool(string key, bool defaultValue = default);
        float GetFloat(string key, float defaultValue = default);
        double GetDouble(string key, double defaultValue = default);
        int GetInt(string key, int defaultValue = default);
        DateTime GetDateTime(string key, DateTime defaultValue);
        string GetString(string key, string defaultValue);

        bool HasKey(string key);
        void DeleteKey(string key);
        void DeleteAll();
        void Save();
        void LoadGameProgress();
        void DeleteGameProgress();
    }
}
