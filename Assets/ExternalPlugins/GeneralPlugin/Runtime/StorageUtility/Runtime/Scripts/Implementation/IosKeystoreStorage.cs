#if UNITY_IOS
using System.Runtime.InteropServices;


namespace Modules.General.Utilities.StorageUtility
{
    public class IosKeystoreStorage : IStorage
    {
        #region Fields

        public const string keychainAccessGroupId = "com.playgendary.sharedItems";


        [DllImport ("__Internal")]
        private static extern void KeychainUtilsInitialize(string accessGroup);

        [DllImport ("__Internal")]
        private static extern string KeychainUtilsLoadByKey(string key);
   
        [DllImport ("__Internal")]
        private static extern void KeychainUtilsSaveForKey(string key, string value);

        [DllImport ("__Internal")]
        private static extern void KeychainUtilsRemoveForKey(string key);

        #endregion



        #region Methods

        public void Initialize()
        {
            KeychainUtilsInitialize(keychainAccessGroupId);
        }

        
        public string Load(string key)
        {
            return KeychainUtilsLoadByKey(key);
        }
    
        
        public void Save(string key, string value)
        {
            KeychainUtilsSaveForKey(key, value);
        }


        public void Remove(string key)
        {
            KeychainUtilsRemoveForKey(key);
        }

        #endregion
    }
}
#endif
