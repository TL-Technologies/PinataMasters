using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;


namespace Modules.InAppPurchase.InternalRestore
{
    [Serializable]
    public class InternalRestorePrefs
    {
        #region Fields

        private const string AssetsDirectoryName = "Assets";

        private const string PluginsDirectoryName = "Plugins";
        
        private const string InternalRestorePrefsFileName = "InternalRestorePrefs.json";

        #endregion


        #region Properties

        /// <summary>
        /// ATTENTION!
        /// Do not use the UnityPath as it is not available at Runtime.
        /// </summary>
        private static string InternalRestorePrefsPath =>
            Path.Combine(AssetsDirectoryName, PluginsDirectoryName, InternalRestorePrefsFileName);


        [JsonProperty]
        public List<string> StoreItems { get; private set; } = new List<string>();

        #endregion



        #region Instancing

        public static InternalRestorePrefs Open()
        {
            if (!File.Exists(InternalRestorePrefsPath))
            {
                var restorePlayerPrefs = new InternalRestorePrefs();
                restorePlayerPrefs.Save();
            }

            return JsonConvert.DeserializeObject<InternalRestorePrefs>(
                File.ReadAllText(InternalRestorePrefsPath),
                GetSerializerSettings());
        }


        private InternalRestorePrefs() { }

        #endregion



        #region Methods

        public bool AddStoreItem(string storeItemId)
        {
            if (StoreItems.Contains(storeItemId))
            {
                return false;
            }

            StoreItems.Add(storeItemId);
            return true;
        }


        public bool RemoveStoreItem(string storeItemId) => StoreItems.Remove(storeItemId);


        public void RemoveAllStoreItem() => StoreItems.Clear();


        public List<string> GetStoreItems()
        {
            return StoreItems.ToList();
        }


        public void Save()
        {
            JsonSerializer serializer = JsonSerializer.Create(GetSerializerSettings());

            using (var file = File.CreateText(InternalRestorePrefsPath))
            using (var writer = new JsonTextWriter(file))
            {
                writer.Indentation = 2;
                writer.IndentChar = ' ';
                serializer.Serialize(writer, this);
            }
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
