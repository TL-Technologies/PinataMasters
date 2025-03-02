using DG.DemiEditor;
using DG.DOTweenEditor;
using Modules.Hive.Editor;
using System;
using System.IO;
using System.Reflection;
using UnityEditor;


namespace Modules.DoTween.Editor
{
    internal static class FixDoTweenPaths
    {
        [InitializeOnLoadMethod]
        private static void FixPaths()
        {
            // It's important to trigger default calculation
            if (string.IsNullOrEmpty(EditorUtils.dotweenDir))
            {
                return;
            }
            
            string pluginPath = DoTweenPluginHierarchy.Instance.RootPath;
            string runtimePath = UnityPath.Combine(pluginPath, "Runtime");
            string editorPath = UnityPath.Combine(pluginPath, "Editor");
            
            SetPrivateField(typeof(EditorUtils), "_dotweenDir", $"{runtimePath}/DOTween/");
            SetPrivateField(typeof(EditorUtils), "_dotweenProDir", $"{runtimePath}/DOTweenPro/");
            SetPrivateField(typeof(EditorUtils), "_demigiantDir", runtimePath);
            SetPrivateField(typeof(EditorUtils), "_dotweenProEditorDir", $"{editorPath}/DOTweenPro/");
            SetPrivateField(typeof(EditorUtils), "_dotweenModulesDir", $"{runtimePath}/DOTween/Modules/");
            
            if (DoTweenPluginHierarchy.Instance.PackageInfo != null)
            {
                // Default plugin logic for determining this variable is incorrect if the plugin is used as a package
                SetPrivateField(
                    typeof(DeStylePalette),
                    "_fooAdbImgsDir",
                    $"{UnityPath.GetAssetPathFromFullPath(editorPath)}/DemiLib/Core/Imgs/");
                
                // Variable _editorADBDir in EditorUtils is used as part of the path, which is started from "Assets".
                // That's why we should move resources, which are accessed through this variable, from package to project itself.
                string doTweenSettingsPath = UnityPath.GetFullPathFromAssetPath(DoTweenPluginHierarchy.DoTweenSettingsDirectoryAssetPath);
                if (!Directory.Exists(UnityPath.Combine(doTweenSettingsPath, "Imgs")))
                {
                    string archivePath = UnityPath.Combine(editorPath, "DOTween", "Imgs.zip");
                    FileSystemUtilities.ExtractArchive(archivePath, doTweenSettingsPath);
                }
                string editorAdbDir = UnityPath.RemoveStart(
                    DoTweenPluginHierarchy.DoTweenSettingsDirectoryAssetPath,
                    UnityPath.AssetsDirectoryName);
                editorAdbDir = $"{editorAdbDir}/";
                SetPrivateField(typeof(EditorUtils), "_editorADBDir", editorAdbDir);
            }
            else
            {
                string editorAdbDir = UnityPath.RemoveStart(
                    UnityPath.GetAssetPathFromFullPath(pluginPath), 
                    UnityPath.AssetsDirectoryName);
                editorAdbDir = $"{editorAdbDir}/Editor/DOTween/";
                SetPrivateField(typeof(EditorUtils), "_editorADBDir", editorAdbDir);
            }
            

            void SetPrivateField(Type type, string fieldName, string fieldValue)
            {
                fieldValue = fieldValue.Replace(EditorUtils.pathSlashToReplace, EditorUtils.pathSlash);
                
                FieldInfo fieldInfo = type.GetField(fieldName, BindingFlags.Static | BindingFlags.NonPublic);
                if (fieldInfo != null)
                {
                    fieldInfo.SetValue(null, fieldValue);
                }
                else
                {
                    CustomDebug.LogWarning($"Can't file field {fieldName} in type {type}!");
                }
            }
        }
    }
}
