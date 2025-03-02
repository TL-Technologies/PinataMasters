using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;


namespace Modules.Hive.Editor.BuildUtilities
{
    public class PluginInfo
    {
        #region Fields

        private string name;
        private string version;
        private string rootPath;
        private bool isPackage;

        #endregion

        
        
        #region Properties

        public string Name => name;

        public string Version => version;

        public string RootPath => rootPath;

        public bool IsPackage => isPackage;

        #endregion

        
        
        #region Properties
        
        public PluginInfo(string name, string version, string rootPath, bool isPackage)
        {
            this.name = name;
            this.version = version;
            this.rootPath = rootPath;
            this.isPackage = isPackage;
        }
        
        #endregion
    }

    
    public class PluginDisabler
    {
        #region Fields

        private static readonly string filePath = "Assets/pluginInfos.json"; 
        
        private static List<PluginInfo> pluginInfos = new List<PluginInfo>();
        private static PluginDisabler currentDisabler = null;

        #endregion

        

        #region Instancing

        public static PluginDisabler CurrentDisabler
        {
            get
            {
                if (currentDisabler == null)
                {
                    currentDisabler = new PluginDisabler();
                }

                return currentDisabler;
            }
        }

        #endregion

        
        
        #region Public methods

        public void PreparePluginForRemoval(PluginHierarchy pluginHierarchy)
        {
            var packageInfo = pluginHierarchy.PackageInfo;
            bool isPackage = pluginHierarchy.PackageInfo != null;
            pluginInfos.Add(new PluginInfo(isPackage ? packageInfo.name : String.Empty,
                                                isPackage ? packageInfo.version : String.Empty, 
                                                       pluginHierarchy.RootAssetPath,
                                                       isPackage));
        }

        #endregion

        

        #region Private methods

        public static void RemovePlugins()
        {
            string jsonString = JsonConvert.SerializeObject(pluginInfos);
            File.WriteAllText(filePath, jsonString);
            
            for (int index = 0; index < pluginInfos.Count; index++)
            {
                PluginInfo pluginInfo = pluginInfos[index];

                if (!pluginInfo.IsPackage)
                {
                    ProjectSnapshot.CurrentSnapshot.SaveDirectoryStructure(pluginInfo.RootPath);
                    DeleteDirectory(pluginInfo.RootPath);
                    
                    Debug.Log("Deleted submodule - " + pluginInfo.RootPath);
                }
                else
                {
                    var manifest = UnityPackagesManifest.Open();
                    manifest.RemoveDependency(pluginInfo.Name);
                    manifest.Save();

                    Debug.Log("Deleted package - " + pluginInfo.Name + " " + pluginInfo.Version);
                }
            }
        }


        public static void RestorePlugins()
        {
            if (!File.Exists(filePath)) return;
            
            string jsonString = File.ReadAllText(filePath);
            File.Delete(filePath);
            List<PluginInfo> pluginInfosDeserialized = JsonConvert.DeserializeObject<List<PluginInfo>>(jsonString);
            pluginInfos.AddRange(pluginInfosDeserialized);

            for (int index = 0; index < pluginInfos.Count; index++)
            {
                PluginInfo pluginInfo = pluginInfos[index];

                if (pluginInfo.IsPackage)
                {
                    var manifest = UnityPackagesManifest.Open();
                    manifest.AddDependency(pluginInfo.Name, pluginInfo.Version);
                    manifest.Save();

                    Debug.Log("Restored package - " + pluginInfo.Name + " " + pluginInfo.Version);
                }
            }
        }


        private static void DeleteDirectory(string target_dir)
        {
            string[] files = Directory.GetFiles(target_dir);
            string[] dirs = Directory.GetDirectories(target_dir);

            foreach (string file in files)
            {
                File.SetAttributes(file, FileAttributes.Normal);
                File.Delete(file);
            }

            foreach (string dir in dirs)
            {
                DeleteDirectory(dir);
            }
            
            Directory.Delete(target_dir, false);
        }

        #endregion
    }
}