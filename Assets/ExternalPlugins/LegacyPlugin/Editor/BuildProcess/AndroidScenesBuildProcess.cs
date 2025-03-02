using Modules.Hive.Editor.BuildUtilities;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;


namespace Modules.Legacy
{
    internal class AndroidScenesBuildProcess : IBuildPipelineOptionsModifier
    {
        public void OnModifyBuildPipelineOptions(BuildPipelineContext context)
        {
            if (context.BuildOptions.target == BuildTarget.Android)
            {
                const string defaultAndroidSplashSceneName = "AndroidSplashScene";
            
                EditorBuildSettingsScene androidSplashScene = EditorBuildSettings.scenes.FirstOrDefault(
                    p => p.path.Contains(defaultAndroidSplashSceneName));

                if (androidSplashScene != null)
                {
                    BuildPlayerOptions buildOptions = context.BuildOptions;
                        
                    List<string> editorScenes = buildOptions.scenes.ToList();
                    editorScenes.Insert(0, androidSplashScene.path);
                    
                    buildOptions.scenes = editorScenes.ToArray();
                    context.BuildOptions = buildOptions;
                }
            }
        }
    }
}
