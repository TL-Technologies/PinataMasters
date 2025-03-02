using System.Collections.Generic;
using UnityEditor;


namespace Modules.Hive.Editor.BuildUtilities
{
    internal class AndroidBuildPostprocessorContext : BuildPostprocessorContext, IAndroidBuildPostprocessorContext
    {
        public AndroidBuildPostprocessorContext(
            IReadOnlyCollection<string> preprocessorDirectives, 
            BuildPlayerOptions buildOptions,
            Dictionary<BuildSetting, string> buildSettings,
            HiveBuildSummary buildProjectSummary) : 
            base(preprocessorDirectives, buildOptions, buildSettings, buildProjectSummary) { }
    }
}
