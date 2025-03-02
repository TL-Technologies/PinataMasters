using Modules.Hive.Editor.Pipeline;
using UnityEditor;


namespace Modules.Hive.Editor.BuildUtilities
{
    [EditorPipelineOptions(AppHostLayer = AppHostLayer.Internal)]
    internal class MacOsBuildProcessor : IBuildPreprocessor<IMacOsBuildPreprocessorContext>
    {
        public void OnPreprocessBuild(IMacOsBuildPreprocessorContext context)
        {
            // Override settings according custom build settings
            if (context.BuildSettings.TryGetValue(BuildSetting.BuildNumber, out string buildNumber))
            {
                PlayerSettings.macOS.buildNumber = buildNumber;
            }
        }
    }
}
