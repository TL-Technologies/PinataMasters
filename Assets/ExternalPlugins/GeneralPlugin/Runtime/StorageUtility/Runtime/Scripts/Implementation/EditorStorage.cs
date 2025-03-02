using Modules.General.HelperClasses;


namespace Modules.General.Utilities.StorageUtility
{
    public class EditorStorage : IStorage
    {
        #region Methods

        public void Initialize() { }

        
        public string Load(string key)
        {
            return CustomPlayerPrefs.GetString(key);
        }

        
        public void Save(string key, string value)
        {
            CustomPlayerPrefs.SetString(key, value);
        }

        
        public void Remove(string key)
        {
            CustomPlayerPrefs.DeleteKey(key);
        }

        #endregion
    }
}
