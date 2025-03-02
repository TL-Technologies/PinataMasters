using System;
using System.Collections.Generic;


namespace Modules.Hive.Editor
{
    // Class for manifest.json file serialization and deserialization purposes
    [Serializable]
    public class UnityRegistryInfo
    {
        public string name = null;
        public string url = null;
        public List<string> scopes = null;
        
        
        public UnityRegistryInfo(string registryName, string registryUrl, List<string> registryScopes)
        {
            name = registryName;
            url = registryUrl;
            scopes = registryScopes;
        }
    }
}
