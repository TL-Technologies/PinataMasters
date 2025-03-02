using UnityEngine;


namespace Modules.Hive.Storage
{
    internal class StorageServiceGeneric : StorageServiceBase
    {
        public override string GetScopeLocation(StorageScope scope)
        {
            return scope == StorageScope.Cache ?
                Application.temporaryCachePath :
                GetScopeDefaultLocation(Application.persistentDataPath, scope);
        }
    }
}
