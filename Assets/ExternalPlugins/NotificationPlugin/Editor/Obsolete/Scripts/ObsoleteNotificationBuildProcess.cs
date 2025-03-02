using Modules.Hive.Editor.BuildUtilities.Android;
using Modules.Hive.Editor.BuildUtilities;
using Modules.Notification.Obsolete;
using Modules.Hive.Editor;
using System;
using System.IO;
using UnityEngine;
using UnityEditor;


namespace Modules.Notification.Editor.Obsolete
{
    internal class PreprocessBuild : IBuildPreprocessor<IAndroidBuildPreprocessorContext>
    {
        private const string ObsoleteAssemblyName = "Modules.Notification.Obsolete";


        public void OnPreprocessBuild(IAndroidBuildPreprocessorContext context)
        {
            if (UnityAssemblyUtilities.IsAssemblyIncludedInBuild(ObsoleteAssemblyName))
            {
                AddIcon(LLNotificationSettings.Instance.NotificationIconTexture);
                AddIcon(LLNotificationSettings.Instance.ViewIconTexture);
                AddIcon(LLNotificationSettings.Instance.ViewBackgroundTexture);
                
                string obsoleteAarAssetPath = UnityPath.Combine(
                    NotificationPluginHierarchy.Instance.RootAssetPath, 
                    "Runtime", 
                    "Obsolete", 
                    "Platforms", 
                    "Android", 
                    "LLLocalNotifications.aar");
                UnityPluginUtilities.EnablePluginAsset(obsoleteAarAssetPath, context.BuildTarget);
            }
            
            
            void AddIcon(Texture texture)
            {
                if (texture == null)
                {    
                    return;
                }

                string assetPath = AssetDatabase.GetAssetPath(texture);
                Texture2D tex2d = texture as Texture2D ?? throw new InvalidCastException($"Cannot convert file '{assetPath}' to Texture2D.");
                context.SharedLibrary.AddResource(tex2d, Path.GetFileName(assetPath), AndroidResourceType.Drawable);
            }
        }
    }
}
