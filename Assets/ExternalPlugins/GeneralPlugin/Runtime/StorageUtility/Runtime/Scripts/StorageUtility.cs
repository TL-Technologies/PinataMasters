namespace Modules.General.Utilities.StorageUtility
{
    public class StorageUtility
    {
        #region Fields

        private static StorageUtility instance;
        private readonly IStorage concreteStorage;
        
        #endregion



        #region Properties

        public static StorageUtility Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new StorageUtility();
                }

                return instance;
            }
        }

        #endregion



        #region Class lifecycle
        
        private StorageUtility()
        {
            concreteStorage =
                #if UNITY_EDITOR
                    new EditorStorage();
                #elif UNITY_ANDROID
                    new AndroidPlayerPrefsStorage();
                #elif UNITY_IOS
                    new IosKeystoreStorage();
                #else
                    new EditorStorage();
                #endif
            
            concreteStorage.Initialize();
        }

        #endregion



        #region Methods

        public string Load(string key)
        {
            return concreteStorage.Load(key);
        }
    
        
        public void Save(string key, string value)
        {
            concreteStorage.Save(key, value);
        }
        
        
        public void Remove(string key)
        {
            concreteStorage.Remove(key);
        }

        #endregion
    }
}
