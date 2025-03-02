using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;


namespace Modules.Hive.Editor
{
    // Class for packages-lock.json file serialization and deserialization purposes
    [Serializable]
    internal class UnityPackagesManifestLock
    {
        #region Nested types

        [Serializable]
        private class PackagesLockEntry
        {
            public string version = null;
            public int depth = 0;
            public string source = null;
            public Dictionary<string, string> dependencies = null;
            public string url = null;
        }
        
        #endregion
        
        
        
        #region Fields

        private static readonly string PackagesLockPath = UnityPath.Combine(
            UnityPath.ProjectPath,
            "Packages",
            "packages-lock.json");
        
        [JsonProperty] private Dictionary<string, PackagesLockEntry> dependencies = null;

        #endregion
        
        
        
        #region Instancing

        public static UnityPackagesManifestLock Open()
        {
            return JsonConvert.DeserializeObject<UnityPackagesManifestLock>(
                File.ReadAllText(PackagesLockPath),
                GetSerializerSettings());
        }
        
        
        private UnityPackagesManifestLock() { }

        #endregion



        #region Methods
        
        public List<string> GetDependencies()
        {
            return dependencies.Keys.ToList();
        }
        

        private static JsonSerializerSettings GetSerializerSettings()
        {
            return new JsonSerializerSettings
            {
                Formatting = Formatting.Indented,
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };
        }

        #endregion
    }
}
