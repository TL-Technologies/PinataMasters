using System;

namespace Modules.Hive.Editor
{
    internal sealed class HivePluginHierarchy : PluginHierarchy
    {
        #region Fields
        
        private const string PlatformTemplatesDirectory = "Templates";
        private const string EditorPlatformTemplatesDirectory = "UnityTemplatesBackup";
        private static HivePluginHierarchy instance;
        private static readonly string EditorPlatformsDirectory = UnityPath.Combine("Editor", "Platforms");
        private static string EditorPlatformsAssetPath;

        #endregion



        #region Instancing

        public static HivePluginHierarchy Instance => instance ?? (instance = new HivePluginHierarchy("Modules.Hive"));


        private HivePluginHierarchy(string mainAssemblyName) : base(mainAssemblyName)
        {
            EditorPlatformsAssetPath = UnityPath.Combine(RootAssetPath, EditorPlatformsDirectory);
        }

        #endregion
        
        
        
        #region Methods
        
        internal string GetPlatformTemplatesAssetPath(PlatformAlias platformAlias) => 
            UnityPath.Combine(PlatformsAssetPath, platformAlias.DirectoryName, PlatformTemplatesDirectory);

        internal string GetPlatformAssetsPath(PlatformAlias platformAlias) =>
            UnityPath.Combine(PlatformsAssetPath, platformAlias.DirectoryName);
        
        internal string GetEditorPlatformTemplatesAssetPath(PlatformAlias platformAlias)
        {
            string unityMajorVersion = null;
            #if UNITY_2020_1_OR_NEWER
                unityMajorVersion = "2020";
            #elif UNITY_2019_4_OR_NEWER
                unityMajorVersion = "2019";
            #endif
            
            if (string.IsNullOrEmpty(unityMajorVersion))
            {
                throw new Exception(
                    "Unsupported Unity Version. Please update templates android templates.");
            }

            PlatformAlias targetPlatform;
            if (platformAlias == PlatformAlias.Android ||
                platformAlias == PlatformAlias.Amazon ||
                platformAlias == PlatformAlias.GooglePlay)
            {
                targetPlatform = PlatformAlias.Android;
            }
            else
            {
                throw new ArgumentException("This method is Android only");
            }

            return UnityPath.Combine(EditorPlatformsAssetPath, targetPlatform.DirectoryName,
                EditorPlatformTemplatesDirectory, unityMajorVersion);
        }

        #endregion
    }
}
