using Modules.Hive.Editor;
using Sirenix.Serialization;
using Sirenix.Utilities;
using System;
using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEngine;


namespace Modules.Odin.Editor
{
    internal static class ExtractConfigsToProject
    {
        [InitializeOnLoadMethod]
        private static void ExtractConfigs()
        {
            string oldConfigPath = UnityPath.GetFullPathFromAssetPath(
                UnityPath.Combine(
                    "Assets",
                    "Plugins",
                    "Sirenix"));
            string newConfigAssetPath = OdinPluginHierarchy.OdinSettingsDirectoryAssetPath;
            string newConfigPath = UnityPath.GetFullPathFromAssetPath(newConfigAssetPath);

            if (Directory.Exists(oldConfigPath))
            {
                FileSystemUtilities.Move(
                    oldConfigPath,
                    newConfigPath,
                    FileSystemOperationOptions.Override);
                
                string oldConfigParentDirectoryPath = UnityPath.GetDirectoryName(oldConfigPath);
                if (FileSystemUtilities.IsDirectoryEmpty(oldConfigParentDirectoryPath))
                {
                    FileSystemUtilities.Delete(oldConfigParentDirectoryPath);
                }
            }

            string configAssetPath = $"{newConfigAssetPath}/Odin Inspector/Config/Editor/";
            string configResourcesAssetPath = $"{newConfigAssetPath}/Odin Inspector/Config/Resources/Sirenix/";
            // It's important to trigger static constructor
            if (!SirenixAssetPaths.OdinEditorConfigsPath.Equals(configAssetPath))
            {
                string configPath = UnityPath.GetFullPathFromAssetPath(configAssetPath);
                string configResourcesPath = UnityPath.GetFullPathFromAssetPath(configResourcesAssetPath);
                if (!Directory.Exists(configPath))
                {
                    Directory.CreateDirectory(configPath);
                }
                if (!Directory.Exists(configResourcesPath))
                {
                    Directory.CreateDirectory(configResourcesPath);
                }

                SetStaticReadonlyField(typeof(SirenixAssetPaths), "OdinEditorConfigsPath", configAssetPath);
                SetStaticReadonlyField(typeof(SirenixAssetPaths), "OdinResourcesPath", configResourcesAssetPath);
                SetStaticReadonlyField(typeof(SirenixAssetPaths), "OdinResourcesConfigsPath", configResourcesAssetPath);
                
                SetupGlobalSerializationAssetPath();
            }
            
            
            void SetStaticReadonlyField(Type type, string fieldName, string fieldValue)
            {
                FieldInfo fieldInfo = type.GetField(fieldName,BindingFlags.Static | BindingFlags.Public);
                if (fieldInfo != null)
                {
                    fieldInfo.SetValue(null, fieldValue);
                }
                else
                {
                    Debug.LogWarning($"Can't file field {fieldName} in type {type}!");
                }
            }
            
            
            void SetupGlobalSerializationAssetPath()
            {
                PropertyInfo propertyInfo = typeof(GlobalConfig<GlobalSerializationConfig>).GetProperty("ConfigAttribute", BindingFlags.Static | BindingFlags.NonPublic);
                GlobalConfigAttribute attribute = propertyInfo.GetValue(null) as GlobalConfigAttribute;
                
                FieldInfo fieldInfo = typeof(GlobalConfigAttribute).GetField("assetPath",BindingFlags.Instance | BindingFlags.NonPublic);
                fieldInfo.SetValue(attribute, configResourcesAssetPath);
            }
        }
    }
}