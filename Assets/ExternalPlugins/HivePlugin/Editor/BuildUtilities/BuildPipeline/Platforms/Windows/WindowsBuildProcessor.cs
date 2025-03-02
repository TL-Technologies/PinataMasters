using Modules.Hive.Editor.Pipeline;
using UnityEditor;


namespace Modules.Hive.Editor.BuildUtilities
{
    [EditorPipelineOptions(AppHostLayer = AppHostLayer.Internal)]
    internal class WindowsBuildProcessor : IBuildPreprocessor<IWindowsBuildPreprocessorContext>
    {
        public void OnPreprocessBuild(IWindowsBuildPreprocessorContext context)
        {
            // Override settings according custom build settings
            if (context.BuildSettings.TryGetValue(BuildSetting.BuildNumber, out string buildNumber))
            {
                // It's unusual, but windows target doesn't have its own field for version
                PlayerSettings.macOS.buildNumber = buildNumber;
            }
        }
    }
}
