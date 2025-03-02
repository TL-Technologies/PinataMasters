using System.Collections.Generic;
using UnityEditor;


namespace Modules.Hive.Editor.BuildUtilities
{
    internal abstract class BuildPreprocessorContext : BuildProcessorContext, IBuildPreprocessorContext
    {
        /// <inheritdoc/>
        public ProjectSnapshot ProjectSnapshot => ProjectSnapshot.CurrentSnapshot;

        public PluginDisabler PluginDisabler => PluginDisabler.CurrentDisabler;


        public BuildPreprocessorContext(
            IReadOnlyCollection<string> preprocessorDirectives,
            BuildPlayerOptions buildOptions,
            Dictionary<BuildSetting, string> buildSettings) :
            base(preprocessorDirectives, buildOptions, buildSettings)
        {
            if (IsCiBuild)
            {
                PlayerSettings.SplashScreen.show = false;
                PlayerSettings.SplashScreen.showUnityLogo = false;

                if (BuildSettings.TryGetValue(BuildSetting.EnableCrashReportAPI, out string enableCrashReportAPI))
                {
                    PlayerSettings.enableCrashReportAPI = enableCrashReportAPI.Equals("true");
                }
            }
        }
    }
}
