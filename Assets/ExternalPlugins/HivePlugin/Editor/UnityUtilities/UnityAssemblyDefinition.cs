using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Compilation;
using UnityEditorInternal;


namespace Modules.Hive.Editor
{
    [Serializable]
    public class UnityAssemblyDefinition
    {
        [JsonIgnore] private bool isGuidReferences = true;
        
        [JsonIgnore] public string OutputPath { get; private set; }
        [JsonIgnore] public bool IsEditorOnly => includePlatforms.Count == 1 && includePlatforms.Contains("Editor");
        [JsonIgnore] public List<string> ReferencesList
        {
            get
            {
                List<string> result = new List<string>(References);
                if (isGuidReferences)
                {
                    result = result.Select(reference =>
                    {
                        UnityAssemblyDefinition asmdef = OpenAtPath(
                            AssetDatabase.GUIDToAssetPath(
                                CompilationPipeline.AssemblyDefinitionReferenceGUIDToGUID(reference)));
                        return asmdef != null ? asmdef.Name : string.Empty;
                    }).ToList();
                }
                
                return result;
            }
        }


        #region DTO

        [JsonProperty] public string Name { get; private set; }
        [JsonProperty] public bool AllowUnsafeCode { get; set; }
        [JsonProperty] public bool OverrideReferences { get; set; }
        [JsonProperty] public bool AutoReferenced { get; set; } = true;
        [JsonProperty] public List<string> DefineConstraints { get; private set; } = new List<string>();
        [JsonProperty] public HashSet<string> OptionalUnityReferences { get; private set; } = new HashSet<string>();
        [JsonProperty] private HashSet<string> References { get; set; } = new HashSet<string>();
        [JsonProperty] private HashSet<string> includePlatforms = new HashSet<string>();
        [JsonProperty] private HashSet<string> excludePlatforms = new HashSet<string>();

        [JsonExtensionData] private IDictionary<string, JToken> additionalData;

        #endregion



        #region Instancing

        /// <summary>
        /// Opens an asmdef-file in the specified path.
        /// </summary>
        /// <param name="assetPath">The asmdef-file to open.</param>
        /// <returns></returns>
        public static UnityAssemblyDefinition OpenAtPath(string assetPath)
        {
            if (string.IsNullOrEmpty(assetPath))
            {
                return null;
            }
            
            AssemblyDefinitionAsset asset = AssetDatabase.LoadAssetAtPath<AssemblyDefinitionAsset>(assetPath);
            var rs = JsonConvert.DeserializeObject<UnityAssemblyDefinition>(asset.text, GetSerializerSettings());
            rs.OutputPath = UnityPath.GetFullPathFromAssetPath(assetPath);
            
            using (HashSet<string>.Enumerator enumerator = rs.References.GetEnumerator())
            {
                // By default Unity assumes that empty references list use guids
                rs.isGuidReferences = !enumerator.MoveNext() || enumerator.Current.StartsWith("GUID");
            }
            
            return rs;
        }


        /// <summary>
        /// Opens an asmdef-file that describes assembly with specified name.
        /// </summary>
        /// <param name="name">The name of assembly to open.</param>
        /// <returns></returns>
        public static UnityAssemblyDefinition OpenForAssemblyName(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return null;
            }
            
            string assetPath = CompilationPipeline.GetAssemblyDefinitionFilePathFromAssemblyName(name);
            return OpenAtPath(assetPath);
        }


        private UnityAssemblyDefinition() { }

        #endregion


        
        #region Methods
        
        public bool AddReferenceToAssembly(string assemblyName)
        {
            bool result = false;
            string reference = GetAssemblyReferenceFormat(assemblyName);
            if (!string.IsNullOrEmpty(reference))
            {
                result = References.Add(reference);
            }

            return result;
        }
        
        
        public bool RemoveReferenceToAssembly(string assemblyName)
        {
            bool result = false;
            string reference = GetAssemblyReferenceFormat(assemblyName);
            if (!string.IsNullOrEmpty(reference))
            {
                result = References.Remove(reference);
            }

            return result;
        }
        
        
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this, GetSerializerSettings());
        }


        public void Save()
        {
            JsonSerializer serializer = JsonSerializer.Create(GetSerializerSettings());

            using (var file = File.CreateText(OutputPath))
            using (var writer = new JsonTextWriter(file))
            {
                writer.Indentation = 4;
                writer.IndentChar = ' ';
                serializer.Serialize(writer, this);
            }

            AssetDatabase.Refresh();
        }
        
        
        private string GetAssemblyReferenceFormat(string assemblyName)
        {
            string reference = assemblyName;
            if (isGuidReferences)
            {
                string assetPath = CompilationPipeline.GetAssemblyDefinitionFilePathFromAssemblyName(assemblyName);
                reference = string.IsNullOrEmpty(assetPath) ? 
                    string.Empty : 
                    CompilationPipeline.GUIDToAssemblyDefinitionReferenceGUID(AssetDatabase.AssetPathToGUID(assetPath));
            }
            
            return reference;
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
