using Modules.Hive.Editor;
using Modules.Hive.Editor.BuildUtilities;
using Sirenix.OdinInspector.Editor;
using Sirenix.Serialization.Utilities.Editor;
using System.IO;
using UnityEngine;


namespace Modules.Odin.Editor
{
    internal class CustomAssemblyImportSettingsAutomation : IBuildPreprocessor<IBuildPreprocessorContext>
    {
        public void OnPreprocessBuild(IBuildPreprocessorContext context)
        {
            if (EditorOnlyModeConfig.Instance.IsEditorOnlyModeEnabled())
            {
                Debug.LogWarning("Odin: Editor only mode currently not supported!");
            }
            
            if (ImportSettingsConfig.Instance.AutomateBeforeBuild)
            {
                string rootAssetPath = OdinPluginHierarchy.Instance.RootAssetPath;
                string rootPath = OdinPluginHierarchy.Instance.RootPath;
            
                bool isJitSupported = AssemblyImportSettingsUtilities.IsJITSupported(
                    context.BuildTarget,
                    AssemblyImportSettingsUtilities.GetCurrentScriptingBackend(),
                    AssemblyImportSettingsUtilities.GetCurrentApiCompatibilityLevel());
                string aotAssembliesDirectoryLocalPath = UnityPath.Combine(
                    "Runtime", 
                    "Sirenix", 
                    "Assemblies", 
                    "NoEmitAndNoEditor");
                string jitAssembliesDirectoryLocalPath = UnityPath.Combine(
                    "Runtime",
                    "Sirenix",
                    "Assemblies",
                    "NoEditor");
            
                ChangeAssembliesImportSettings(aotAssembliesDirectoryLocalPath, !isJitSupported);
                ChangeAssembliesImportSettings(jitAssembliesDirectoryLocalPath, isJitSupported);
    
    
                void ChangeAssembliesImportSettings(string directoryLocalPath, bool shouldEnable)
                {
                    foreach (FileInfo fileInfo in FileSystemUtilities.EnumerateFiles(
                        UnityPath.Combine(rootPath, directoryLocalPath), 
                        "*.dll", 
                        SearchOption.TopDirectoryOnly))
                    {
                        if (shouldEnable)
                        {
                            UnityPluginUtilities.EnablePluginAsset(
                                UnityPath.Combine(rootAssetPath, directoryLocalPath, fileInfo.Name),
                                context.BuildTarget);
                        }
                        else
                        {
                            UnityPluginUtilities.DisablePluginAsset(
                                UnityPath.Combine(rootAssetPath, directoryLocalPath, fileInfo.Name),
                                context.BuildTarget);
                        }
                    }
                }
            }
        }
    }
}
