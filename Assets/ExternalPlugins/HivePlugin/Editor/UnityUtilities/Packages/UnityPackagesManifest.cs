using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;


namespace Modules.Hive.Editor
{
    // Class for manifest.json file serialization and deserialization purposes
    [Serializable]
    public class UnityPackagesManifest
    {
        #region Fields

        private static readonly string PackagesManifestPath = UnityPath.Combine(
            UnityPath.ProjectPath,
            "Packages",
            "manifest.json");
        private static UnityPackagesManifestLock packagesLock;
        

        #endregion
        
        
        #region Properties

        [JsonProperty] public List<UnityRegistryInfo> ScopedRegistries { get; private set; } = new List<UnityRegistryInfo>();
        [JsonProperty] public Dictionary<string, string> Dependencies { get; private set; } = new Dictionary<string, string>();
        
        #endregion



        #region Instancing

        public static UnityPackagesManifest Open()
        {
            packagesLock = UnityPackagesManifestLock.Open();
            
            return JsonConvert.DeserializeObject<UnityPackagesManifest>(
                File.ReadAllText(PackagesManifestPath),
                GetSerializerSettings());
        }
        
        
        private UnityPackagesManifest() { }

        #endregion



        #region Methods
        
        public bool AddDependency(string packageName, string packageVersion)
        {
            if (Dependencies.ContainsKey(packageName))
            {
                return false;
            }
            
            Dependencies[packageName] = packageVersion;
            return true;
        }
        
        
        public bool RemoveDependency(string packageName) => Dependencies.Remove(packageName);
        
        
        public bool AddScopedRegistry(string name, string url, List<string> scopes)
        {
            if (ScopedRegistries.Exists(info => info.name.Equals(name)))
            {
                Debug.LogWarning($"Scoped registries already contains registry with name {name}");
                return false;
            }
            
            ScopedRegistries.Add(new UnityRegistryInfo(name, url, scopes));
            return true;
        }
        
        
        public bool RemoveScopedRegistry(string name) => 
            ScopedRegistries.RemoveAll(info => info.name.Equals(name)) > 0;
        
        
        public List<string> GetDependencies()
        {
            return packagesLock.GetDependencies();
        }
        
        
        public void Save()
        {
            packagesLock = null;
            
            JsonSerializer serializer = JsonSerializer.Create(GetSerializerSettings());

            using (var file = File.CreateText(PackagesManifestPath))
            using (var writer = new JsonTextWriter(file))
            {
                writer.Indentation = 2;
                writer.IndentChar = ' ';
                serializer.Serialize(writer, this);
            }
            
            AssetDatabase.Refresh();
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
