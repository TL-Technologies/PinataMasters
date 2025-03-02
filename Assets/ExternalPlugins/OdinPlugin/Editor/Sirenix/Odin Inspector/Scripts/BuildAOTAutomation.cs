//-----------------------------------------------------------------------
// <copyright file="BuildAOTAutomation.cs" company="Sirenix IVS">
// Copyright (c) Sirenix IVS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using Modules.Hive.Editor;
using Modules.Odin.Editor;

#if UNITY_EDITOR && UNITY_5_6_OR_NEWER

namespace Sirenix.Serialization.Internal
{
    using Sirenix.Serialization;
    using UnityEditor;
    using UnityEditor.Build;
    using System.IO;
    using System;

#if UNITY_2018_1_OR_NEWER

    using UnityEditor.Build.Reporting;

#endif

#if UNITY_2018_1_OR_NEWER
    public class PreBuildAOTAutomation : IPreprocessBuildWithReport
#else
    public class PreBuildAOTAutomation : IPreprocessBuild
#endif
    {
        private string GeneratedDllPath = $"{OdinPluginHierarchy.OdinSettingsDirectoryAssetPath}/AOTAssemblies/";
        
        public int callbackOrder { get { return -1000; } }

        public void OnPreprocessBuild(BuildTarget target, string path)
        {
            if (AOTGenerationConfig.Instance.ShouldAutomationGeneration(target))
            {
                string previouslyGeneratedDll = FileSystemUtilities.FindFileEndsWith("Sirenix.Serialization.AOTGenerated.dll");
                if (!string.IsNullOrEmpty(previouslyGeneratedDll))
                {
                    File.Delete(previouslyGeneratedDll);
                    string metaName = $"{previouslyGeneratedDll}.meta";
                    if (File.Exists(metaName))
                    {
                        File.Delete(metaName);
                    }
                }
                
                AOTGenerationConfig.Instance.ScanProject();
                AOTGenerationConfig.Instance.GenerateDLL(GeneratedDllPath);
            }
        }

#if UNITY_2018_1_OR_NEWER

        public void OnPreprocessBuild(BuildReport report)
        {
            this.OnPreprocessBuild(report.summary.platform, report.summary.outputPath);
        }

#endif
    }

#if UNITY_2018_1_OR_NEWER
    public class PostBuildAOTAutomation : IPostprocessBuildWithReport
#else
    public class PostBuildAOTAutomation : IPostprocessBuild
#endif
    {
        private string GeneratedDllPath = $"{OdinPluginHierarchy.OdinSettingsDirectoryAssetPath}/AOTAssemblies/";
        
        public int callbackOrder { get { return -1000; } }

        public void OnPostprocessBuild(BuildTarget target, string path)
        {
            if (AOTGenerationConfig.Instance.DeleteDllAfterBuilds && AOTGenerationConfig.Instance.ShouldAutomationGeneration(target))
            {
                Directory.Delete(GeneratedDllPath, true);
                File.Delete(GeneratedDllPath.TrimEnd('/', '\\') + ".meta");
                AssetDatabase.Refresh();
            }
        }

#if UNITY_2018_1_OR_NEWER

        public void OnPostprocessBuild(BuildReport report)
        {
            this.OnPostprocessBuild(report.summary.platform, report.summary.outputPath);
        }

#endif
    }
}

#endif // UNITY_EDITOR && UNITY_5_6_OR_NEWER