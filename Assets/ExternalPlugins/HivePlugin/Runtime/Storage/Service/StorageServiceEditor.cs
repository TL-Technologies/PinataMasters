#if UNITY_EDITOR
using System.IO;
using System.Threading.Tasks;
using UnityEngine;

namespace Modules.Hive.Storage
{
    internal class StorageServiceEditor : StorageServiceBase
    {
        private string rootPath = null;


        internal override Task RunAsync(StorageServiceConfig config)
        {
            rootPath = Path.Combine(
                Path.GetDirectoryName(Application.dataPath),
                "AppData");

            return base.RunAsync(config);
        }


        public override string GetScopeLocation(StorageScope scope)
        {
            return GetScopeDefaultLocation(rootPath, scope);
        }
    }
}
#endif
