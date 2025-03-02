using Modules.Hive.Editor.BuildUtilities;
using UnityEditor;


namespace Modules.Hive.Editor
{
    public static class UnityPluginUtilities
    {
        public static bool SetPluginAssetEnabled(string assetPath, bool enable, BuildTarget target)
        {
            UnityPath.AssertPathLocatedInsideProject(assetPath);

            bool result = false;
            PluginImporter importer = AssetImporter.GetAtPath(assetPath) as PluginImporter;
            if (importer != null)
            {
                if (enable != importer.IsPluginAssetEnabled(target))
                {
                    // Keep current meta-file in stash if it's created
                    ProjectSnapshot.CurrentSnapshot?.CopyFileToStash(assetPath + ".meta");
                
                    importer.SetCompatibleWithAnyPlatform(false);
                    importer.SetCompatibleWithPlatform(target, enable);
                    importer.SaveAndReimport();
                }
                
                result = true;
            }

            return result;
        }


        public static bool IsPluginAssetEnabled(string assetPath, BuildTarget target)
        {
            UnityPath.AssertPathLocatedInsideProject(assetPath);

            PluginImporter importer = AssetImporter.GetAtPath(assetPath) as PluginImporter;
            return importer != null && importer.IsPluginAssetEnabled(target);
        }


        public static bool EnablePluginAsset(string assetPath, BuildTarget target)
        {
            return SetPluginAssetEnabled(assetPath, true, target);
        }


        public static bool DisablePluginAsset(string assetPath, BuildTarget target)
        {
            return SetPluginAssetEnabled(assetPath, false, target);
        }


        private static bool IsPluginAssetEnabled(this PluginImporter importer, BuildTarget target)
        {
            return importer.GetCompatibleWithAnyPlatform() || importer.GetCompatibleWithPlatform(target);
        }
    }
}