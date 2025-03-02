using System;
using System.Collections.Generic;


namespace Modules.Hive.Editor.BuildUtilities.Android
{
    public static class AndroidManifestUtilities
    {
        #region Fields

        public static readonly string AndroidManifestTemplatesUnityDirectory =
            HivePluginHierarchy.Instance.GetEditorPlatformTemplatesAssetPath(PlatformAlias.Android);

        private static readonly string HiveTemplatesDirectoryPath = 
            UnityPath.GetFullPathFromAssetPath(HivePluginHierarchy.Instance.GetPlatformTemplatesAssetPath(PlatformAlias.Android));
        private static readonly string ProjectTemplatesDirectoryPath = UnityPath.GetFullPathFromAssetPath(UnityPath.AndroidPluginsAssetPath);
        
        private static readonly Dictionary<ManifestType, string> ManifestFileNameByType = new Dictionary<ManifestType, string>()
        {
            { ManifestType.Launcher, "LauncherManifest.xml" },
            { ManifestType.Main, "AndroidManifest.xml" }
        };
        
        #endregion
        
        
        
        #region Methods

        public static IEnumerable<ManifestType> EnumerateManifestTemplateTypes()
        {
            return ManifestFileNameByType.Keys;
        }
        
        
        public static string GetManifestTemplateName(ManifestType manifestType)
        {
            if (!ManifestFileNameByType.TryGetValue(manifestType, out string result))
            {
                throw new ArgumentException($"Can't get template name for manifest type {manifestType}!");
            }
            
            return result;
        }
        
        
        /// <summary>
        /// Gets path to existed manifest template of the project.
        /// </summary>
        public static string GetProjectTemplatePath(ManifestType manifestType) => 
            UnityPath.Combine(
                ProjectTemplatesDirectoryPath, 
                GetManifestTemplateName(manifestType));
        
        
        /// <summary>
        /// Gets path to existed manifest template of Hive plugin.
        /// </summary>
        public static string GetHiveTemplatePath(ManifestType manifestType) => 
            UnityPath.Combine(
                HiveTemplatesDirectoryPath, 
                GetManifestTemplateName(manifestType));
        
        
        /// <summary>
        /// Get path to an application android manifest template that Unity uses by default.
        /// </summary>
        public static string GetUnityTemplatePath(ManifestType manifestType)
        {
            string manifestName = GetManifestTemplateName(manifestType);
            #if UNITY_2019_3_OR_NEWER
                if (manifestType == ManifestType.Main)
                {
                    manifestName = "UnityManifest.xml";
                }
            #endif
            return UnityPath.Combine(
                AndroidManifestTemplatesUnityDirectory,
                manifestName);
        }
        
        
        #endregion
    }
}
