using UnityEngine;


namespace Modules.Hive.Storage
{
    public class PlayerPrefsStorage : IKeyValueStorage
    {
        public string this[string key]
        {
            get { return PlayerPrefs.GetString(key); }
            set { PlayerPrefs.SetString(key, value); }
        }
        
        
        public bool ContainsKey(string key)
        {
            return PlayerPrefs.HasKey(key);
        }
        

        public bool TryGetValue(string key, out string value)
        {
            bool b = PlayerPrefs.HasKey(key);
            value = b ? PlayerPrefs.GetString(key) : null;
            return b;
        }
        

        public void Save()
        {
            PlayerPrefs.Save();
        }
        

        public bool Remove(string key)
        {
            bool hasKey = PlayerPrefs.HasKey(key);
            if (hasKey)
            {
                PlayerPrefs.DeleteKey(key);
            }

            return hasKey;
        }
    }
}
