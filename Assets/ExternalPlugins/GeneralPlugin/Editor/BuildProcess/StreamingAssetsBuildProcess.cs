using Modules.Hive.Editor;
using Modules.Hive.Editor.BuildUtilities;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;


namespace Modules.General.Editor.BuildProcess
{
    internal class StreamingAssetsBuildProcess : IBuildPreprocessor<IBuildPreprocessorContext>
    {
        private static readonly HashSet<string> PlatformFolders = new HashSet<string>
        {
            "iOS", "Android", "MacOS", "Windows", "WebGL"
        };
        
        
        public void OnPreprocessBuild(IBuildPreprocessorContext context)
        {
            string streamingAssetsPath = UnityPath.GetFullPathFromAssetPath(UnityPath.StreamingAssetsAssetPath);

            TrimStreamingFolders(streamingAssetsPath, context.BuildTarget, assetDirectory =>
            {
                ProjectSnapshot.CurrentSnapshot?.MoveAssetToStash(UnityPath.GetAssetPathFromFullPath(assetDirectory));
            });
            
            
            void TrimStreamingFolders(string rootDirectory, BuildTarget currentBuildTarget, Action<string /*src dir*/> trimmingAction)
            {
                string currentFolder = GetCurrentPlatformFolder(currentBuildTarget);

                if (!Directory.Exists(rootDirectory))
                {
                    return;
                }

                HashSet<string> duplicates = new HashSet<string>();

                foreach (string platformFolder in PlatformFolders)
                {
                    if (platformFolder != currentFolder)
                    {
                        string[] directories = Directory.GetDirectories(
                            rootDirectory,
                            platformFolder,
                            SearchOption.AllDirectories);

                        foreach (string directory in directories)
                        {
                            int index = directory.IndexOf(platformFolder);
                            string rootFolder = directory.Substring(0, index + platformFolder.Length); //iOS/anyPath/iOS  -> iOS/

                            if (duplicates.Add(rootFolder)) //check circle references from folder, iOS/Folder/iOS e.g.
                            {
                                trimmingAction(rootFolder);
                            }
                        }
                    }
                }
            }
            
            
            string GetCurrentPlatformFolder(BuildTarget buildTarget)
            {
                switch (buildTarget)
                {
                    case BuildTarget.iOS:
                        return "iOS";

                    case BuildTarget.Android:
                        return "Android";
                
                    case BuildTarget.StandaloneOSX:
                        return "MacOS";
                
                    case BuildTarget.StandaloneWindows64:
                        return "Windows";
                
                    case BuildTarget.WebGL:
                        return "WebGL";
                        
                    default:
                        throw new NotImplementedException($"Unable to determine a platform folder name for build target '{buildTarget}'");
                }
            }
        }
    }
}
