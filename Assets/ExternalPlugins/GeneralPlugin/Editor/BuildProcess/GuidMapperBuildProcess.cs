using Modules.Hive.Editor;
using Modules.Hive.Editor.BuildUtilities;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using UnityEditor;


namespace Modules.General.Editor.BuildProcess
{
    internal class GuidMapperBuildProcess : IBuildPreprocessor<IBuildPreprocessorContext>, IBuildFinalizer
    {
        private const string EditorDirectoryName = "Editor";
        private const string ScriptFileExtension = ".cs";
        private static readonly string GuidsAssetPath = UnityPath.Combine(
            UnityPath.StreamingAssetsAssetPath,
            "guids");
        private static readonly string GuidsPath = UnityPath.GetFullPathFromAssetPath(GuidsAssetPath);
        private static readonly string StreamingAssetsPath = UnityPath.GetFullPathFromAssetPath(UnityPath.StreamingAssetsAssetPath);
        
        
        public void OnPreprocessBuild(IBuildPreprocessorContext context)
        {
            if (!Directory.Exists(StreamingAssetsPath))
            {
                Directory.CreateDirectory(StreamingAssetsPath);
            }
            FileSystemUtilities.Delete(GuidsPath);
    
            string[] paths = AssetDatabase.GetAllAssetPaths();
            Dictionary<string, string> result = new Dictionary<string, string>();
            for (int i = 0; i < paths.Length; i++)
            {
                string path = paths[i];
                if (path.StartsWith(UnityPath.AssetsDirectoryName, System.StringComparison.Ordinal) &&
                    !path.Contains(EditorDirectoryName) &&
                    !path.Contains(ScriptFileExtension) &&
                    (path.Contains(UnityPath.ResourcesDirectoryName) || path.Contains(UnityPath.StreamingAssetsDirectoryName)))
                {
                    result.Add(AssetDatabase.AssetPathToGUID(path), path);
                }
            }
    
            File.WriteAllText(GuidsPath, JsonConvert.SerializeObject(result));
        }
    
    
        public void OnFinalizeBuild(BuildPipelineContext context)
        {
            if (Directory.Exists(StreamingAssetsPath))
            {
                FileSystemUtilities.Delete(GuidsPath);
                
                if (FileSystemUtilities.IsDirectoryEmpty(StreamingAssetsPath))
                {
                    FileSystemUtilities.Delete(StreamingAssetsPath);
                }
            }
        }
    }
}
