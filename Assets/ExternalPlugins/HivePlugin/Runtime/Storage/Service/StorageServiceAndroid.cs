#if UNITY_ANDROID
using UnityEngine;


namespace Modules.Hive.Storage
{
    internal class StorageServiceAndroid : StorageServiceBase
    {
        private static readonly AndroidJavaClass StorageServiceClass = new AndroidJavaClass("org.hive.foundation.storage.StorageService");

        private string internalStoragePath = null;
        private string cacheStoragePath = null;


        public string InternalStoragePath 
            => internalStoragePath ?? (internalStoragePath = StorageServiceClass.CallStatic<string>("getInternalStoragePath"));


        public string CacheStoragePath
            => cacheStoragePath ?? (cacheStoragePath = StorageServiceClass.CallStatic<string>("getCacheStoragePath"));


        public override string GetScopeLocation(StorageScope scope)
        {
            switch (scope)
            {
                case StorageScope.Cache:
                    return CacheStoragePath;
                case StorageScope.Local:
                case StorageScope.Roaming:
                    return GetScopeDefaultLocation(InternalStoragePath, scope);
                default:
                    throw new System.NotImplementedException($"Unable to determine a root path for storage scope '${scope}'");
            }
        }
    }
}
#endif
